using System;
using System.Device.I2c;
using System.Threading;

namespace Icarus.Sensors.Tilt
{
    public class TiltSensor : ITiltSensor
    {
        private static I2cDevice _i2C;
        private const byte PowerMgmt1 = 0x6b;

        private TiltSensor()
        {
            _i2C = I2cDevice.Create(new I2cConnectionSettings(0x1, 0x68));
            _i2C.Write(new ReadOnlySpan<byte>(new[] { PowerMgmt1, (byte)0x00 }));
        }

        public RotationResult GetRotationResult()
        {
            var xAcceleration = ReadWord2C(0x3b);
            var yAcceleration = ReadWord2C(0x3d);
            var zAcceleration = ReadWord2C(0x3f);

            var xAccelerationScaled = xAcceleration / 16384.0;
            var yAccelerationScaled = yAcceleration / 16384.0;
            var zAccelerationScaled = zAcceleration / 16384.0;

            return new RotationResult
            {
                XRotation = Math.Round(GetXRotation(xAccelerationScaled, yAccelerationScaled, zAccelerationScaled), 2),
                YRotation = Math.Round(GetYRotation(xAccelerationScaled, yAccelerationScaled, zAccelerationScaled), 2)
            };
        }

        private static byte ReadByte(byte reg)
        {

            Span<byte> outArray = stackalloc byte[1];
            Span<byte> regAddrBytes = stackalloc byte[1];
            regAddrBytes[0] = reg;

            _i2C.WriteRead(regAddrBytes, outArray);

            Thread.Sleep(1);

            return outArray.ToArray()[0];

        }

        private static ushort ReadWord(byte reg)
        {
            var h = ReadByte(reg);
            reg++;
            var l = ReadByte(reg);
            var value = (h << 8) + l;
            return (ushort)value;
        }

        private static short ReadWord2C(byte reg)
        {
            var value = ReadWord(reg);
            return (short)value;
        }

        private static double Dist(double a, double b)
        {
            return Math.Sqrt((a * a) + (b * b));
        }

        private static double GetYRotation(double x, double y, double z)
        {
            var radians = Math.Atan2(x, Dist(y, z));
            return -((180 / Math.PI) * radians);
        }

        private static double GetXRotation(double x, double y, double z)
        {
            var radians = Math.Atan2(y, Dist(x, z));
            return (180 / Math.PI) * radians;
        }
    }
}
