using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.Tof
{
    public class DistanceSensor : IDistanceSensor
    {
        private readonly I2cDevice _i2cDevice;

        private ushort _fastOscFrequency = 0;
        private ushort _oscCalibrateVal = 0;
        private DistanceMode _distanceMode;
        private const ushort TargetRate = 0x0A00;

        private DistanceSensor()
        {
            _i2cDevice = I2cDevice.Create(new I2cConnectionSettings(0x1, 0x29));

            Init(false);
            StartContinuous(100);
        }

        public double GetDistanceMillimeters()
        {
            var bytes = ReadRegVariable(regAddr.RESULT__RANGE_STATUS, 17);

            var status = bytes[0];

            //Console.WriteLine($"Status: {status:X}");

            var distance = bytes[13] << 8 | bytes[14];

            //distance = (distance * 2011 + 0x0400) / 0x0800;

            //Console.WriteLine($"Status {status:X} Distance: {distance}mm ({distance / 1000d}m)");

            return distance;
        }


        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDistanceSensor>(new DistanceSensor());
        }

        private bool Init(bool io2V8)
        {
            var modelIdAndType = ReadReg16Bit(regAddr.IDENTIFICATION__MODEL_ID);

            Console.WriteLine($"modelIdAndType: {modelIdAndType:X}");

            if (modelIdAndType != 0xEACC)
            {
                return false;
            }

            WriteReg(regAddr.SOFT_RESET, 0x00);
            WriteReg(regAddr.SOFT_RESET, 0x01);
            Thread.Sleep(1);

            Console.WriteLine("check firmware system status");
            while ((ReadReg(regAddr.FIRMWARE__SYSTEM_STATUS) & 0x01) == 0)
            {
                Console.WriteLine("readReg == 0, going to wait and try again");
                Thread.Sleep(10);
            }


            //// store oscillator info for later use
            _fastOscFrequency = ReadReg16Bit(regAddr.OSC_MEASURED__FAST_OSC__FREQUENCY);
            _oscCalibrateVal = ReadReg16Bit(regAddr.RESULT__OSC_CALIBRATE_VAL);

            Console.WriteLine($"fast_osc_frequency: {_fastOscFrequency:X} osc_calibrate_val: {_oscCalibrateVal:X}");

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
            WriteReg16Bit(regAddr.DSS_CONFIG__TARGET_TOTAL_RATE_MCPS, TargetRate); // should already be this value after reset
            WriteReg(regAddr.GPIO__TIO_HV_STATUS, 0x02);
            WriteReg(regAddr.SIGMA_ESTIMATOR__EFFECTIVE_PULSE_WIDTH_NS, 8); // tuning parm default
            WriteReg(regAddr.SIGMA_ESTIMATOR__EFFECTIVE_AMBIENT_WIDTH_NS, 16); // tuning parm default
            WriteReg(regAddr.ALGO__CROSSTALK_COMPENSATION_VALID_HEIGHT_MM, 0x01);
            WriteReg(regAddr.ALGO__RANGE_IGNORE_VALID_HEIGHT_MM, 0xFF);
            WriteReg(regAddr.ALGO__RANGE_MIN_CLIP, 0); // tuning parm default
            WriteReg(regAddr.ALGO__CONSISTENCY_CHECK__TOLERANCE, 2); // tuning parm default

            //// general config
            WriteReg16Bit(regAddr.SYSTEM__THRESH_RATE_HIGH, 0x0000);
            WriteReg16Bit(regAddr.SYSTEM__THRESH_RATE_LOW, 0x0000);
            WriteReg(regAddr.DSS_CONFIG__APERTURE_ATTENUATION, 0x38);

            //// timing config
            //// most of these settings will be determined later by distance and timing
            //// budget configuration
            WriteReg16Bit(regAddr.RANGE_CONFIG__SIGMA_THRESH, 360); // tuning parm default
            WriteReg16Bit(regAddr.RANGE_CONFIG__MIN_COUNT_RATE_RTN_LIMIT_MCPS, 192); // tuning parm default

            //// dynamic config

            WriteReg(regAddr.SYSTEM__GROUPED_PARAMETER_HOLD_0, 0x01);
            WriteReg(regAddr.SYSTEM__GROUPED_PARAMETER_HOLD_1, 0x01);
            WriteReg(regAddr.SD_CONFIG__QUANTIFIER, 2); // tuning parm default

            //// VL53L1_preset_mode_standard_ranging() end

            //// from VL53L1_preset_mode_timed_ranging_*
            //// GPH is 0 after reset, but writing GPH0 and GPH1 above seem to set GPH to 1,
            //// and things don't seem to work if we don't set GPH back to 0 (which the API
            //// does here).
            WriteReg(regAddr.SYSTEM__GROUPED_PARAMETER_HOLD, 0x00);
            WriteReg(regAddr.SYSTEM__SEED_CONFIG, 1); // tuning parm default

            //// from VL53L1_config_low_power_auto_mode
            WriteReg(regAddr.SYSTEM__SEQUENCE_CONFIG, 0x8B); // VHV, PHASECAL, DSS1, RANGE
            WriteReg16Bit(regAddr.DSS_CONFIG__MANUAL_EFFECTIVE_SPADS_SELECT, 200 << 8);
            WriteReg(regAddr.DSS_CONFIG__ROI_MODE_CONTROL, 2); // REQUESTED_EFFFECTIVE_SPADS

            //// VL53L1_set_preset_mode() end

            //// default to long range, 50 ms timing budget
            //// note that this is different than what the API defaults to

            SetDistanceMode(DistanceMode.Short);
            //setMeasurementTimingBudget(50000);

            //// VL53L1_StaticInit() end

            //// the API triggers this change in VL53L1_init_and_start_range() once a
            //// measurement is started; assumes MM1 and MM2 are disabled
            WriteReg16Bit(regAddr.ALGO__PART_TO_PART_RANGE_OFFSET_MM,
              ReadReg16Bit((regAddr)((ushort)regAddr.MM_CONFIG__OUTER_OFFSET_MM * 4)));


            return true;
        }


        private void StartContinuous(UInt32 periodMs)
        {
            // from VL53L1_set_inter_measurement_period_ms()
            WriteReg32Bit(regAddr.SYSTEM__INTERMEASUREMENT_PERIOD, periodMs * _oscCalibrateVal);

            WriteReg(regAddr.SYSTEM__INTERRUPT_CLEAR, 0x01); // sys_interrupt_clear_range
            WriteReg(regAddr.SYSTEM__MODE_START, 0x40); // mode_range__timed
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
                    WriteReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_A, 0x07);
                    WriteReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_B, 0x05);
                    WriteReg(regAddr.RANGE_CONFIG__VALID_PHASE_HIGH, 0x38);

                    // dynamic config
                    WriteReg(regAddr.SD_CONFIG__WOI_SD0, 0x07);
                    WriteReg(regAddr.SD_CONFIG__WOI_SD1, 0x05);
                    WriteReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD0, 6); // tuning parm default
                    WriteReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD1, 6); // tuning parm default

                    break;

                case DistanceMode.Medium:
                    // from VL53L1_preset_mode_standard_ranging()

                    // timing config
                    WriteReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_A, 0x0B);
                    WriteReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_B, 0x09);
                    WriteReg(regAddr.RANGE_CONFIG__VALID_PHASE_HIGH, 0x78);

                    // dynamic config
                    WriteReg(regAddr.SD_CONFIG__WOI_SD0, 0x0B);
                    WriteReg(regAddr.SD_CONFIG__WOI_SD1, 0x09);
                    WriteReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD0, 10); // tuning parm default
                    WriteReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD1, 10); // tuning parm default

                    break;

                case DistanceMode.Long: // long
                                        // from VL53L1_preset_mode_standard_ranging_long_range()

                    // timing config
                    WriteReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_A, 0x0F);
                    WriteReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_B, 0x0D);
                    WriteReg(regAddr.RANGE_CONFIG__VALID_PHASE_HIGH, 0xB8);

                    // dynamic config
                    WriteReg(regAddr.SD_CONFIG__WOI_SD0, 0x0F);
                    WriteReg(regAddr.SD_CONFIG__WOI_SD1, 0x0D);
                    WriteReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD0, 14); // tuning parm default
                    WriteReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD1, 14); // tuning parm default

                    break;

                default:
                    // unrecognized mode - do nothing
                    return false;
            }

            // save mode so it can be returned by getDistanceMode()
            _distanceMode = mode;

            return true;
        }

        private byte[] ReadRegVariable(regAddr regAddr, int n)
        {
            Thread.Sleep(1);

            Span<byte> outArray = stackalloc byte[n];
            Span<byte> regAddrBytes = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(regAddrBytes, (ushort)regAddr);

            _i2cDevice.WriteRead(regAddrBytes, outArray);

            return outArray.ToArray();
        }

        // Read an 8-bit register
        private byte ReadReg(regAddr regAddr)
        {
            return ReadRegVariable(regAddr, 1)[0];
        }

        private UInt16 ReadReg16Bit(regAddr regAddr)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(ReadRegVariable(regAddr, 2));
        }

        private void WriteReg(regAddr regAddr, byte[] values)
        {
            Thread.Sleep(1);
            Span<byte> regAddrBytes = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(regAddrBytes, (ushort)regAddr);

            _i2cDevice.Write(regAddrBytes.ToArray().Concat(values).ToArray());
        }

        private void WriteReg(regAddr reg, byte value)
        {
            WriteReg(reg, new[] { value });
        }

        private void WriteReg16Bit(regAddr reg, ushort value)
        {
            Span<byte> valueArray = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(valueArray, value);

            WriteReg(reg, valueArray.ToArray());
        }

        private void WriteReg32Bit(regAddr reg, UInt32 value)
        {
            Span<byte> valueArray = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(valueArray, value);

            WriteReg(reg, valueArray.ToArray());
        }
    }
}
