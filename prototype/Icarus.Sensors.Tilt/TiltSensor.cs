using System;
using System.Device.I2c;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.Tilt
{
    public class TiltResult
    {
        public double GyroscopeX { get; set; }
        public double GyroscopeY { get; set; }
        public double GyroscopeZ { get; set; }

        public double AccelerationX { get; set; }
        public double AccelerationY { get; set; }
        public double AccelerationZ { get; set; }

        public double RotationX { get; set; }
        public double RotationY { get; set; }
    }

    public class TiltSensor
    {
        static I2cDevice _i2C;
        static byte power_mgmt_1 = 0x6b;
        static byte power_mgmt_2 = 0x6c;

        private TiltSensor()
        {
            _i2C = I2cDevice.Create(new I2cConnectionSettings(0x1, 0x68));
            _i2C.Write(new ReadOnlySpan<byte>(new[] { power_mgmt_1, (byte)0x00 }));
        }
        
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(new TiltSensor());
        }

        public TiltResult GetTilt()
        {
            var gyroscopeX = read_word_2c(0x43);
            var gyroscopeY = read_word_2c(0x45);
            var gyroscopeZ = read_word_2c(0x47);

            Console.WriteLine($"x: {Math.Round(gyroscopeX / 131d, 2)}");
            Console.WriteLine($"y: {Math.Round(gyroscopeY / 131d, 2)}");
            Console.WriteLine($"z: {Math.Round(gyroscopeZ / 131d, 2)}");

            var acceleartionX = read_word_2c(0x3b);
            var accelerationY = read_word_2c(0x3d);
            var accelerationZ = read_word_2c(0x3f);

            var accelerationXScaled = acceleartionX / 16384.0;
            var accelerationYScaled = accelerationY / 16384.0;
            var acceleartionZScaled = accelerationZ / 16384.0;

            Console.WriteLine($"x {Math.Round(accelerationXScaled, 2)}");
            Console.WriteLine($"y {Math.Round(accelerationYScaled, 2)}");
            Console.WriteLine($"z {Math.Round(acceleartionZScaled, 2)}");

            Console.WriteLine($"X Rotation {Math.Round(get_x_rotation(accelerationXScaled, accelerationYScaled, acceleartionZScaled), 2)}");
            Console.WriteLine($"Y Rotation {Math.Round(get_y_rotation(accelerationXScaled, accelerationYScaled, acceleartionZScaled), 2)}");

            return new TiltResult
            {
                GyroscopeX = gyroscopeX,
                GyroscopeY = gyroscopeY,
                GyroscopeZ = gyroscopeZ,
                AccelerationX = accelerationXScaled,
                AccelerationY = accelerationYScaled,
                AccelerationZ = acceleartionZScaled,
                RotationX = Math.Round(get_x_rotation(accelerationXScaled, accelerationYScaled, acceleartionZScaled), 2),
                RotationY = Math.Round(get_y_rotation(accelerationXScaled, accelerationYScaled, acceleartionZScaled), 2)
            };
        }

        private static byte read_byte(byte reg)
        {

            Span<byte> outArray = stackalloc byte[1];
            Span<byte> regAddrBytes = stackalloc byte[1];
            regAddrBytes[0] = reg;

            _i2C.WriteRead(regAddrBytes, outArray);

            return outArray.ToArray()[0];

        }

        private static ushort read_word(byte reg)
        {
            var h = read_byte(reg);
            reg++;
            var l = read_byte(reg);
            var value = (h << 8) + l;
            return (ushort)value;
        }

        private static short read_word_2c(byte reg)
        {
            var value = read_word(reg);
            return (short)value;
        }

        private static double dist(double a, double b)
        {
            return Math.Sqrt((a * a) + (b * b));
        }

        private static double get_y_rotation(double x, double y, double z)
        {
            var radians = Math.Atan2(x, dist(y, z));
            return -((180 / Math.PI) * radians);
        }

        private static double get_x_rotation(double x, double y, double z)
        {
            var radians = Math.Atan2(y, dist(x, z));
            return (180 / Math.PI) * radians;
        }

    }
}
