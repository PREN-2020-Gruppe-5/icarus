using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.Linq;
using System.Threading;

namespace Icarus.Sensors.Tof
{
    public class TofSensor : ITofSensor
    {
        private readonly I2cDevice i2CDevice;

        private ushort fastOscFrequency = 0;
        private ushort oscCalibrateVal = 0;
        private DistanceMode distanceMode;
        private const ushort TargetRate = 0x0A00;

        public TofSensor()
        {
            this.i2CDevice = I2cDevice.Create(new I2cConnectionSettings(0x1, 0x29));

            Init(false);
            StartContinuous(100);
        }

        public double GetDistanceMillimeters()
        {
            var bytes = ReadRegVariable(RegAddr.ResultRangeStatus, 17);

            var status = bytes[0];

            //Console.WriteLine($"Status: {status:X}");

            var distance = bytes[13] << 8 | bytes[14];

            //distance = (distance * 2011 + 0x0400) / 0x0800;

            //Console.WriteLine($"Status {status:X} Distance: {distance}mm ({distance / 1000d}m)");

            return distance;
        }

        private bool Init(bool io2V8)
        {
            var modelIdAndType = ReadReg16Bit(RegAddr.IdentificationModelId);

            Console.WriteLine($"modelIdAndType: {modelIdAndType:X}");

            if (modelIdAndType != 0xEACC)
            {
                return false;
            }

            WriteReg(RegAddr.SoftReset, 0x00);
            WriteReg(RegAddr.SoftReset, 0x01);
            Thread.Sleep(1);

            Console.WriteLine("check firmware system status");
            while ((ReadReg(RegAddr.FirmwareSystemStatus) & 0x01) == 0)
            {
                Console.WriteLine("readReg == 0, going to wait and try again");
                Thread.Sleep(10);
            }


            //// store oscillator info for later use
            this.fastOscFrequency = ReadReg16Bit(RegAddr.OscMeasuredFastOscFrequency);
            this.oscCalibrateVal = ReadReg16Bit(RegAddr.ResultOscCalibrateVal);

            Console.WriteLine($"fast_osc_frequency: {this.fastOscFrequency:X} osc_calibrate_val: {this.oscCalibrateVal:X}");

            //// VL53L1_DataInit() end

            //// VL53L1_StaticInit() begin

            //// Note that the API does not actually apply the configuration settings below
            //// when VL53L1_StaticInit() is called: it keeps a copy of the sensor's
            //// register contents in memory and doesn't actually write them until a
            //// measurement is started. Writing the configuration here means we don't have
            //// to keep it all in memory and avoids a lot of redundant writes later.

            //// the API sets the preset mode to LOWPOWER_AUTONOMOUS here:
            //// VL53L1_set_preset_mode() begin

            //// VL53L1_preset_mode_standard_ranging() begin

            //// values labeled "tuning parm default" are from vl53l1_tuning_parm_defaults.h
            //// (API uses these in VL53L1_init_tuning_parm_storage_struct())

            //// static config
            //// API resets PAD_I2C_HV__EXTSUP_CONFIG here, but maybe we don't want to do
            //// that? (seems like it would disable 2V8 mode)
            WriteReg16Bit(RegAddr.DssConfigTargetTotalRateMcps, TargetRate); // should already be this value after reset
            WriteReg(RegAddr.GpioTioHvStatus, 0x02);
            WriteReg(RegAddr.SigmaEstimatorEffectivePulseWidthNs, 8); // tuning parm default
            WriteReg(RegAddr.SigmaEstimatorEffectiveAmbientWidthNs, 16); // tuning parm default
            WriteReg(RegAddr.AlgoCrosstalkCompensationValidHeightMm, 0x01);
            WriteReg(RegAddr.AlgoRangeIgnoreValidHeightMm, 0xFF);
            WriteReg(RegAddr.AlgoRangeMinClip, 0); // tuning parm default
            WriteReg(RegAddr.AlgoConsistencyCheckTolerance, 2); // tuning parm default

            //// general config
            WriteReg16Bit(RegAddr.SystemThreshRateHigh, 0x0000);
            WriteReg16Bit(RegAddr.SystemThreshRateLow, 0x0000);
            WriteReg(RegAddr.DssConfigApertureAttenuation, 0x38);

            //// timing config
            //// most of these settings will be determined later by distance and timing
            //// budget configuration
            WriteReg16Bit(RegAddr.RangeConfigSigmaThresh, 360); // tuning parm default
            WriteReg16Bit(RegAddr.RangeConfigMinCountRateRtnLimitMcps, 192); // tuning parm default

            //// dynamic config

            WriteReg(RegAddr.SystemGroupedParameterHold0, 0x01);
            WriteReg(RegAddr.SystemGroupedParameterHold1, 0x01);
            WriteReg(RegAddr.SdConfigQuantifier, 2); // tuning parm default

            //// VL53L1_preset_mode_standard_ranging() end

            //// from VL53L1_preset_mode_timed_ranging_*
            //// GPH is 0 after reset, but writing GPH0 and GPH1 above seem to set GPH to 1,
            //// and things don't seem to work if we don't set GPH back to 0 (which the API
            //// does here).
            WriteReg(RegAddr.SystemGroupedParameterHold, 0x00);
            WriteReg(RegAddr.SystemSeedConfig, 1); // tuning parm default

            //// from VL53L1_config_low_power_auto_mode
            WriteReg(RegAddr.SystemSequenceConfig, 0x8B); // VHV, PHASECAL, DSS1, RANGE
            WriteReg16Bit(RegAddr.DssConfigManualEffectiveSpadsSelect, 200 << 8);
            WriteReg(RegAddr.DssConfigRoiModeControl, 2); // REQUESTED_EFFFECTIVE_SPADS

            //// VL53L1_set_preset_mode() end

            //// default to long range, 50 ms timing budget
            //// note that this is different than what the API defaults to

            SetDistanceMode(DistanceMode.Short);
            //setMeasurementTimingBudget(50000);

            //// VL53L1_StaticInit() end

            //// the API triggers this change in VL53L1_init_and_start_range() once a
            //// measurement is started; assumes MM1 and MM2 are disabled
            WriteReg16Bit(RegAddr.AlgoPartToPartRangeOffsetMm,
              ReadReg16Bit((RegAddr)((ushort)RegAddr.MmConfigOuterOffsetMm * 4)));


            return true;
        }


        private void StartContinuous(UInt32 periodMs)
        {
            // from VL53L1_set_inter_measurement_period_ms()
            WriteReg32Bit(RegAddr.SystemIntermeasurementPeriod, periodMs * this.oscCalibrateVal);

            WriteReg(RegAddr.SystemInterruptClear, 0x01); // sys_interrupt_clear_range
            WriteReg(RegAddr.SystemModeStart, 0x40); // mode_range__timed
        }

        private enum DistanceMode { Short, Medium, Long, Unknown };

        // set distance mode to Short, Medium, or Long
        // based on VL53L1_SetDistanceMode()
        private bool SetDistanceMode(DistanceMode mode)
        {
            switch (mode)
            {
                case DistanceMode.Short:
                    // from VL53L1_preset_mode_standard_ranging_short_range()

                    // timing config
                    WriteReg(RegAddr.RangeConfigVcselPeriodA, 0x07);
                    WriteReg(RegAddr.RangeConfigVcselPeriodB, 0x05);
                    WriteReg(RegAddr.RangeConfigValidPhaseHigh, 0x38);

                    // dynamic config
                    WriteReg(RegAddr.SdConfigWoiSd0, 0x07);
                    WriteReg(RegAddr.SdConfigWoiSd1, 0x05);
                    WriteReg(RegAddr.SdConfigInitialPhaseSd0, 6); // tuning parm default
                    WriteReg(RegAddr.SdConfigInitialPhaseSd1, 6); // tuning parm default

                    break;

                case DistanceMode.Medium:
                    // from VL53L1_preset_mode_standard_ranging()

                    // timing config
                    WriteReg(RegAddr.RangeConfigVcselPeriodA, 0x0B);
                    WriteReg(RegAddr.RangeConfigVcselPeriodB, 0x09);
                    WriteReg(RegAddr.RangeConfigValidPhaseHigh, 0x78);

                    // dynamic config
                    WriteReg(RegAddr.SdConfigWoiSd0, 0x0B);
                    WriteReg(RegAddr.SdConfigWoiSd1, 0x09);
                    WriteReg(RegAddr.SdConfigInitialPhaseSd0, 10); // tuning parm default
                    WriteReg(RegAddr.SdConfigInitialPhaseSd1, 10); // tuning parm default

                    break;

                case DistanceMode.Long: // long
                                        // from VL53L1_preset_mode_standard_ranging_long_range()

                    // timing config
                    WriteReg(RegAddr.RangeConfigVcselPeriodA, 0x0F);
                    WriteReg(RegAddr.RangeConfigVcselPeriodB, 0x0D);
                    WriteReg(RegAddr.RangeConfigValidPhaseHigh, 0xB8);

                    // dynamic config
                    WriteReg(RegAddr.SdConfigWoiSd0, 0x0F);
                    WriteReg(RegAddr.SdConfigWoiSd1, 0x0D);
                    WriteReg(RegAddr.SdConfigInitialPhaseSd0, 14); // tuning parm default
                    WriteReg(RegAddr.SdConfigInitialPhaseSd1, 14); // tuning parm default

                    break;

                default:
                    // unrecognized mode - do nothing
                    return false;
            }

            // save mode so it can be returned by getDistanceMode()
            this.distanceMode = mode;

            return true;
        }

        private byte[] ReadRegVariable(RegAddr regAddr, int n)
        {
            Thread.Sleep(1);

            Span<byte> outArray = stackalloc byte[n];
            Span<byte> regAddrBytes = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(regAddrBytes, (ushort)regAddr);

            this.i2CDevice.WriteRead(regAddrBytes, outArray);

            return outArray.ToArray();
        }

        // Read an 8-bit register
        private byte ReadReg(RegAddr regAddr)
        {
            return ReadRegVariable(regAddr, 1)[0];
        }

        private UInt16 ReadReg16Bit(RegAddr regAddr)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(ReadRegVariable(regAddr, 2));
        }

        private void WriteReg(RegAddr regAddr, byte[] values)
        {
            Thread.Sleep(1);
            Span<byte> regAddrBytes = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(regAddrBytes, (ushort)regAddr);

            this.i2CDevice.Write(regAddrBytes.ToArray().Concat(values).ToArray());
        }

        private void WriteReg(RegAddr reg, byte value)
        {
            WriteReg(reg, new[] { value });
        }

        private void WriteReg16Bit(RegAddr reg, ushort value)
        {
            Span<byte> valueArray = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(valueArray, value);

            WriteReg(reg, valueArray.ToArray());
        }

        private void WriteReg32Bit(RegAddr reg, UInt32 value)
        {
            Span<byte> valueArray = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(valueArray, value);

            WriteReg(reg, valueArray.ToArray());
        }
    }
}
