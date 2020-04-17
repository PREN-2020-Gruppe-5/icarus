using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.Linq;
using System.Threading;

// ReSharper disable InconsistentNaming

namespace Icarus.Sensors.Tof
{
    public class VL53L1X
    {
        private readonly I2cDevice _i2C;

        private ushort fast_osc_frequency = 0;
        private ushort osc_calibrate_val = 0;
        private DistanceMode distance_mode;
        private const ushort TargetRate = 0x0A00;

        public VL53L1X(I2cDevice i2c)
        {
            _i2C = i2c;
        }

        public bool init(bool io_2v8)
        {
            var modelIdAndType = readReg16Bit(regAddr.IDENTIFICATION__MODEL_ID);

            Console.WriteLine($"modelIdAndType: {modelIdAndType:X}");

            if (modelIdAndType != 0xEACC)
            {
                return false;
            }
            
            writeReg(regAddr.SOFT_RESET, 0x00);
            writeReg(regAddr.SOFT_RESET, 0x01);
            Thread.Sleep(1);

            Console.WriteLine("check firmware system status");
            while ((readReg(regAddr.FIRMWARE__SYSTEM_STATUS) & 0x01) == 0)
            {
                Console.WriteLine("readReg == 0, going to wait and try again");
                Thread.Sleep(10);
            }


            //// store oscillator info for later use
            fast_osc_frequency = readReg16Bit(regAddr.OSC_MEASURED__FAST_OSC__FREQUENCY);
            osc_calibrate_val = readReg16Bit(regAddr.RESULT__OSC_CALIBRATE_VAL);

            Console.WriteLine($"fast_osc_frequency: {fast_osc_frequency:X} osc_calibrate_val: {osc_calibrate_val:X}");

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
            writeReg16Bit(regAddr.DSS_CONFIG__TARGET_TOTAL_RATE_MCPS, TargetRate); // should already be this value after reset
            writeReg(regAddr.GPIO__TIO_HV_STATUS, 0x02);
            writeReg(regAddr.SIGMA_ESTIMATOR__EFFECTIVE_PULSE_WIDTH_NS, 8); // tuning parm default
            writeReg(regAddr.SIGMA_ESTIMATOR__EFFECTIVE_AMBIENT_WIDTH_NS, 16); // tuning parm default
            writeReg(regAddr.ALGO__CROSSTALK_COMPENSATION_VALID_HEIGHT_MM, 0x01);
            writeReg(regAddr.ALGO__RANGE_IGNORE_VALID_HEIGHT_MM, 0xFF);
            writeReg(regAddr.ALGO__RANGE_MIN_CLIP, 0); // tuning parm default
            writeReg(regAddr.ALGO__CONSISTENCY_CHECK__TOLERANCE, 2); // tuning parm default

            //// general config
            writeReg16Bit(regAddr.SYSTEM__THRESH_RATE_HIGH, 0x0000);
            writeReg16Bit(regAddr.SYSTEM__THRESH_RATE_LOW, 0x0000);
            writeReg(regAddr.DSS_CONFIG__APERTURE_ATTENUATION, 0x38);

            //// timing config
            //// most of these settings will be determined later by distance and timing
            //// budget configuration
            writeReg16Bit(regAddr.RANGE_CONFIG__SIGMA_THRESH, 360); // tuning parm default
            writeReg16Bit(regAddr.RANGE_CONFIG__MIN_COUNT_RATE_RTN_LIMIT_MCPS, 192); // tuning parm default

            //// dynamic config

            writeReg(regAddr.SYSTEM__GROUPED_PARAMETER_HOLD_0, 0x01);
            writeReg(regAddr.SYSTEM__GROUPED_PARAMETER_HOLD_1, 0x01);
            writeReg(regAddr.SD_CONFIG__QUANTIFIER, 2); // tuning parm default

            //// VL53L1_preset_mode_standard_ranging() end

            //// from VL53L1_preset_mode_timed_ranging_*
            //// GPH is 0 after reset, but writing GPH0 and GPH1 above seem to set GPH to 1,
            //// and things don't seem to work if we don't set GPH back to 0 (which the API
            //// does here).
            writeReg(regAddr.SYSTEM__GROUPED_PARAMETER_HOLD, 0x00);
            writeReg(regAddr.SYSTEM__SEED_CONFIG, 1); // tuning parm default

            //// from VL53L1_config_low_power_auto_mode
            writeReg(regAddr.SYSTEM__SEQUENCE_CONFIG, 0x8B); // VHV, PHASECAL, DSS1, RANGE
            writeReg16Bit(regAddr.DSS_CONFIG__MANUAL_EFFECTIVE_SPADS_SELECT, 200 << 8);
            writeReg(regAddr.DSS_CONFIG__ROI_MODE_CONTROL, 2); // REQUESTED_EFFFECTIVE_SPADS

            //// VL53L1_set_preset_mode() end

            //// default to long range, 50 ms timing budget
            //// note that this is different than what the API defaults to

            setDistanceMode(DistanceMode.Short);
            //setMeasurementTimingBudget(50000);

            //// VL53L1_StaticInit() end

            //// the API triggers this change in VL53L1_init_and_start_range() once a
            //// measurement is started; assumes MM1 and MM2 are disabled
            writeReg16Bit(regAddr.ALGO__PART_TO_PART_RANGE_OFFSET_MM,
              readReg16Bit((regAddr)((ushort)regAddr.MM_CONFIG__OUTER_OFFSET_MM * 4)));


            return true;
        }

        public int GetDistanceMillimeters()
        {
            var bytes = readRegVariable(regAddr.RESULT__RANGE_STATUS, 17);

            var status = bytes[0];

            Console.WriteLine($"Status: {status:X}");
            
            var distance = bytes[13] << 8 | bytes[14];

            //distance = (distance * 2011 + 0x0400) / 0x0800;

            Console.WriteLine($"Status {status:X} Distance: {distance}mm ({distance / 1000d}m)");

            return distance;
        }

        public void startContinuous(UInt32 period_ms)
        {
            // from VL53L1_set_inter_measurement_period_ms()
            writeReg32Bit(regAddr.SYSTEM__INTERMEASUREMENT_PERIOD, period_ms * osc_calibrate_val);

            writeReg(regAddr.SYSTEM__INTERRUPT_CLEAR, 0x01); // sys_interrupt_clear_range
            writeReg(regAddr.SYSTEM__MODE_START, 0x40); // mode_range__timed
        }

        enum DistanceMode { Short, Medium, Long, Unknown };

        // set distance mode to Short, Medium, or Long
        // based on VL53L1_SetDistanceMode()
        bool setDistanceMode(DistanceMode mode)
        {
            switch (mode)
            {
                case DistanceMode.Short:
                    // from VL53L1_preset_mode_standard_ranging_short_range()

                    // timing config
                    writeReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_A, 0x07);
                    writeReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_B, 0x05);
                    writeReg(regAddr.RANGE_CONFIG__VALID_PHASE_HIGH, 0x38);

                    // dynamic config
                    writeReg(regAddr.SD_CONFIG__WOI_SD0, 0x07);
                    writeReg(regAddr.SD_CONFIG__WOI_SD1, 0x05);
                    writeReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD0, 6); // tuning parm default
                    writeReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD1, 6); // tuning parm default

                    break;

                case DistanceMode.Medium:
                    // from VL53L1_preset_mode_standard_ranging()

                    // timing config
                    writeReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_A, 0x0B);
                    writeReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_B, 0x09);
                    writeReg(regAddr.RANGE_CONFIG__VALID_PHASE_HIGH, 0x78);

                    // dynamic config
                    writeReg(regAddr.SD_CONFIG__WOI_SD0, 0x0B);
                    writeReg(regAddr.SD_CONFIG__WOI_SD1, 0x09);
                    writeReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD0, 10); // tuning parm default
                    writeReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD1, 10); // tuning parm default

                    break;

                case DistanceMode.Long: // long
                                        // from VL53L1_preset_mode_standard_ranging_long_range()

                    // timing config
                    writeReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_A, 0x0F);
                    writeReg(regAddr.RANGE_CONFIG__VCSEL_PERIOD_B, 0x0D);
                    writeReg(regAddr.RANGE_CONFIG__VALID_PHASE_HIGH, 0xB8);

                    // dynamic config
                    writeReg(regAddr.SD_CONFIG__WOI_SD0, 0x0F);
                    writeReg(regAddr.SD_CONFIG__WOI_SD1, 0x0D);
                    writeReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD0, 14); // tuning parm default
                    writeReg(regAddr.SD_CONFIG__INITIAL_PHASE_SD1, 14); // tuning parm default

                    break;

                default:
                    // unrecognized mode - do nothing
                    return false;
            }

            // save mode so it can be returned by getDistanceMode()
            distance_mode = mode;

            return true;
        }
        
        byte[] readRegVariable(regAddr regAddr, int n)
        {

            Thread.Sleep(1);

            Span<byte> outArray = stackalloc byte[n];
            Span<byte> regAddrBytes = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(regAddrBytes, (ushort)regAddr);

            _i2C.WriteRead(regAddrBytes, outArray);

            return outArray.ToArray();
        }

        // Read an 8-bit register
        byte readReg(regAddr regAddr)
        {
            return readRegVariable(regAddr, 1)[0];
        }

        UInt16 readReg16Bit(regAddr regAddr)
        {

            return BinaryPrimitives.ReadUInt16BigEndian(readRegVariable(regAddr, 2));
        }

        void writeReg(regAddr regAddr, byte[] values)
        {
            Thread.Sleep(1);
            Span<byte> regAddrBytes = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(regAddrBytes, (ushort)regAddr);

            _i2C.Write(regAddrBytes.ToArray().Concat(values).ToArray());
        }

        void writeReg(regAddr reg, byte value)
        {
            writeReg(reg, new[] { value });
        }

        void writeReg16Bit(regAddr reg, ushort value)
        {
            Span<byte> valueArray = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(valueArray, value);

            writeReg(reg, valueArray.ToArray());
        }

        void writeReg32Bit(regAddr reg, UInt32 value)
        {
            Span<byte> valueArray = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(valueArray, value);

            writeReg(reg, valueArray.ToArray());
        }
    }
}
