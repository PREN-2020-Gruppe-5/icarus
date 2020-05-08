using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Icarus.Common;
#if !DEBUG
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Icarus.Sensors.ObjectDetection.ManualTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // sudo docker run -it --rm --name objectdetection -v /usr/lib/aarch64-linux-gnu:/usr/lib/aarch64-linux-gnu:ro -v /etc/apt/sources.list.d/cuda-10-0-local-10.0.326.list:/etc/apt/sources.list.d/cuda-10-0-local-10.0.326.list:ro -v /etc/ld.so.conf.d/cuda-10-0.conf:/etc/ld.so.conf.d/cuda-10-0.conf:ro -v /etc/nvidia-container-runtime/host-files-for-container.d/cuda.csv:/etc/nvidia-container-runtime/host-files-for-container.d/cuda.csv:ro -v /etc/alternatives/libcudnn:/etc/alternatives/libcudnn:ro -v /etc/alternatives/libcudnn_so:/etc/alternatives/libcudnn_so:ro -v /etc/alternatives/libcudnn_stlib:/etc/alternatives/libcudnn_stlib:ro -v /etc/nvidia-container-runtime/host-files-for-container.d/cudnn.csv:/etc/nvidia-container-runtime/host-files-for-container.d/cudnn.csv:ro -v /etc/systemd/nvfb.sh:/etc/systemd/nvfb.sh:ro -v /etc/systemd/nv.sh:/etc/systemd/nv.sh:ro -v /etc/systemd/nvwifibt.sh:/etc/systemd/nvwifibt.sh:ro -v /etc/systemd/nvmemwarning.sh:/etc/systemd/nvmemwarning.sh:ro -v /etc/systemd/nvweston.sh:/etc/systemd/nvweston.sh:ro -v /etc/systemd/nvfb-early.sh:/etc/systemd/nvfb-early.sh:ro -v /etc/systemd/resolved.conf.d/nv-fallback-dns.conf:/etc/systemd/resolved.conf.d/nv-fallback-dns.conf:ro -v /etc/systemd/nvwifibt-pre.sh:/etc/systemd/nvwifibt-pre.sh:ro -v /etc/systemd/nvgetty.sh:/etc/systemd/nvgetty.sh:ro -v /etc/systemd/system/nv.service:/etc/systemd/system/nv.service:ro -v /etc/systemd/system/nvpmodel.service:/etc/systemd/system/nvpmodel.service:ro -v /etc/systemd/system/nvfb-early.service:/etc/systemd/system/nvfb-early.service:ro -v /etc/systemd/system/nvzramconfig.service:/etc/systemd/system/nvzramconfig.service:ro -v /etc/systemd/system/nv-l4t-usb-device-mode-runtime.service:/etc/systemd/system/nv-l4t-usb-device-mode-runtime.service:ro -v /etc/systemd/system/nvphs.service:/etc/systemd/system/nvphs.service:ro -v /etc/systemd/system/nv-oem-config.target.wants:/etc/systemd/system/nv-oem-config.target.wants:ro -v /etc/systemd/system/nv-oem-config.target.wants/nvfb-early.service:/etc/systemd/system/nv-oem-config.target.wants/nvfb-early.service:ro -v /etc/systemd/system/nvargus-daemon.service:/etc/systemd/system/nvargus-daemon.service:ro -v /etc/systemd/system/nvgetty.service:/etc/systemd/system/nvgetty.service:ro -v /etc/systemd/system/nvweston.service:/etc/systemd/system/nvweston.service:ro -v /etc/systemd/system/nvwifibt.service:/etc/systemd/system/nvwifibt.service:ro -v /etc/systemd/system/nvfb.service:/etc/systemd/system/nvfb.service:ro -v /etc/systemd/system/multi-user.target.wants/nv.service:/etc/systemd/system/multi-user.target.wants/nv.service:ro -v /etc/systemd/system/multi-user.target.wants/nvpmodel.service:/etc/systemd/system/multi-user.target.wants/nvpmodel.service:ro -v /etc/systemd/system/multi-user.target.wants/nvfb-early.service:/etc/systemd/system/multi-user.target.wants/nvfb-early.service:ro -v /etc/systemd/system/multi-user.target.wants/nvzramconfig.service:/etc/systemd/system/multi-user.target.wants/nvzramconfig.service:ro -v /etc/systemd/system/multi-user.target.wants/nv-l4t-usb-device-mode-runtime.service:/etc/systemd/system/multi-user.target.wants/nv-l4t-usb-device-mode-runtime.service:ro -v /etc/systemd/system/multi-user.target.wants/nvphs.service:/etc/systemd/system/multi-user.target.wants/nvphs.service:ro -v /etc/systemd/system/multi-user.target.wants/nv-l4t-bootloader-config.service:/etc/systemd/system/multi-user.target.wants/nv-l4t-bootloader-config.service:ro -v /etc/systemd/system/multi-user.target.wants/nvargus-daemon.service:/etc/systemd/system/multi-user.target.wants/nvargus-daemon.service:ro -v /etc/systemd/system/multi-user.target.wants/nvgetty.service:/etc/systemd/system/multi-user.target.wants/nvgetty.service:ro -v /etc/systemd/system/multi-user.target.wants/nvweston.service:/etc/systemd/system/multi-user.target.wants/nvweston.service:ro -v /etc/systemd/system/multi-user.target.wants/nvfb.service:/etc/systemd/system/multi-user.target.wants/nvfb.service:ro -v /etc/systemd/system/multi-user.target.wants/nvs-service.service:/etc/systemd/system/multi-user.target.wants/nvs-service.service:ro -v /etc/systemd/system/multi-user.target.wants/nvmemwarning.service:/etc/systemd/system/multi-user.target.wants/nvmemwarning.service:ro -v /etc/systemd/system/multi-user.target.wants/nv-l4t-usb-device-mode.service:/etc/systemd/system/multi-user.target.wants/nv-l4t-usb-device-mode.service:ro -v /etc/systemd/system/nvs-service.service:/etc/systemd/system/nvs-service.service:ro -v /etc/systemd/system/nvmemwarning.service:/etc/systemd/system/nvmemwarning.service:ro -v /etc/systemd/system/nv-l4t-usb-device-mode.service:/etc/systemd/system/nv-l4t-usb-device-mode.service:ro -v /etc/systemd/timesyncd.conf.d/nv-fallback-ntp.conf:/etc/systemd/timesyncd.conf.d/nv-fallback-ntp.conf:ro -v /etc/systemd/nvzramconfig.sh:/etc/systemd/nvzramconfig.sh:ro -v /etc/nvpmodel.conf:/etc/nvpmodel.conf:ro -v /etc/nv:/etc/nv:ro -v /etc/nvpmodel:/etc/nvpmodel:ro -v /etc/nvpmodel/nvpmodel_t210_jetson-nano.conf:/etc/nvpmodel/nvpmodel_t210_jetson-nano.conf:ro -v /etc/nvpmodel/nvpmodel_t210.conf:/etc/nvpmodel/nvpmodel_t210.conf:ro -v /etc/apt/sources.list.d/nvidia-l4t-apt-source.list:/etc/apt/sources.list.d/nvidia-l4t-apt-source.list:ro -v /etc/nv_boot_control.conf:/etc/nv_boot_control.conf:ro -v /etc/nvphsd_common.conf:/etc/nvphsd_common.conf:ro -v /etc/skel/Desktop/nv_forums.desktop:/etc/skel/Desktop/nv_forums.desktop:ro -v /etc/skel/Desktop/nv_devzone.desktop:/etc/skel/Desktop/nv_devzone.desktop:ro -v /etc/skel/Desktop/nv_jetson_zoo.desktop:/etc/skel/Desktop/nv_jetson_zoo.desktop:ro -v /etc/vulkan/icd.d/nvidia_icd.json:/etc/vulkan/icd.d/nvidia_icd.json:ro -v /etc/nvphsd.conf:/etc/nvphsd.conf:ro -v /etc/apparmor.d/abstractions/nvidia:/etc/apparmor.d/abstractions/nvidia:ro -v /etc/xdg/autostart/nvpmodel_indicator.desktop:/etc/xdg/autostart/nvpmodel_indicator.desktop:ro -v /etc/xdg/autostart/nvchrome.sh:/etc/xdg/autostart/nvchrome.sh:ro -v /etc/xdg/autostart/nvchrome.desktop:/etc/xdg/autostart/nvchrome.desktop:ro -v /etc/xdg/autostart/nvbackground.desktop:/etc/xdg/autostart/nvbackground.desktop:ro -v /etc/xdg/autostart/nvbackground.sh:/etc/xdg/autostart/nvbackground.sh:ro -v /etc/ld.so.conf.d/nvidia-tegra.conf:/etc/ld.so.conf.d/nvidia-tegra.conf:ro -v /etc/nvidia:/etc/nvidia:ro -v /etc/nvidia-container-runtime:/etc/nvidia-container-runtime:ro -v /usr/local/cuda-10.0:/usr/local/cuda-10.0:ro -v /usr/local/cuda:/usr/local/cuda:ro -v /var/cuda-repo-10-0-local:/var/cuda-repo-10-0-local:ro -v /var/nvidia:/var/nvidia:ro -v /etc/nvidia:/etc/nvidia:ro -v /etc/alternatives:/etc/alternatives:ro -v /etc/ld.so.cache:/etc/ld.so.cache:ro -v /usr/local/lib:/usr/local/lib:ro --privileged derungsapp/icarus.sensors.objectdetection.manualtests

            // test if this test is run on ARM64 and Linux (Nvidia Jetson Nano)
            Console.WriteLine($"This test runs on {RuntimeInformation.OSDescription} {RuntimeInformation.ProcessArchitecture}");
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            {
                ConsoleHelper.WriteLine("This manual test is not supported on this machine. \n Please make sure to run the test on the actual device with the sensors wired. \n Press 'c' to continue in debug mode", ConsoleColor.Red);
                var key = Console.ReadKey();

                if (key.KeyChar != 'c')
                {
                    return;
                }
            }

            var cancellationToken = new CancellationToken();
#if !DEBUG
            var serviceCollection = new ServiceCollection();
            ObjectDetectionModule.Initialize(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var objectDetectionSensor = serviceProvider.GetService<IObjectDetectionSensor>();
#else
            IObjectDetectionSensor objectDetectionSensor = null;
#endif
            await objectDetectionSensor.RunDetectionFromImage("traffic_cone_1.png");
            var result = objectDetectionSensor.GetDetectedObjects();

            if (result != null && result.Count == 1 && result.All(p => p.Confidence >= 0.8))
            {
                ConsoleHelper.WriteLine("Test 1 passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine("Test 1 failed. Expected 1 Object with >= 0.8 confidence", ConsoleColor.Red);
            }

            await objectDetectionSensor.RunDetectionFromImage("traffic_cone_2.png");
            result = objectDetectionSensor.GetDetectedObjects();

            if (result.Count == 8 && result.All(p => p.Confidence >= 0.3))
            {
                ConsoleHelper.WriteLine("Test 2 passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine("Test 2 failed. Expected 8 Objects with >= 0.3 confidence", ConsoleColor.Red);
            }

            await objectDetectionSensor.RunDetectionFromImage("traffic_cone_3.png");
            result = objectDetectionSensor.GetDetectedObjects();

            if (result.Count == 1 && result.All(p => p.Confidence >= 0.9))
            {
                ConsoleHelper.WriteLine("Test 3 passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine("Test 3 failed. Expected 1 Object with >= 0.9 confidence", ConsoleColor.Red);
            }

            await objectDetectionSensor.RunDetectionFromImage("traffic_cone_4.png");
            result = objectDetectionSensor.GetDetectedObjects();
            
            if (result.Count == 1 && result.All(p => p.Confidence >= 0.3))
            {
                ConsoleHelper.WriteLine("Test 4 passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine("Test 4 failed. Expected 1 Object with >= 0.3 confidence", ConsoleColor.Red);
            }
        }
    }
}
