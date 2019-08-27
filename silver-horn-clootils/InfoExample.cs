using System;
using System.IO;
using Cloo;

namespace Clootils
{
    public class InfoExample : IExample
    {
        public string Name => "System info";

        public string Description => "Prints some information about the current platform and its devices.";

        public void Run(ComputeContext context, TextWriter log)
        {
            log.WriteLine("[HOST]");
            log.WriteLine(Environment.OSVersion);

            log.WriteLine();
            log.WriteLine("[OPENCL PLATFORM]");

            var platform = context.Platform;

            log.WriteLine("Name: " + platform.Name);
            log.WriteLine("Vendor: " + platform.Vendor);
            log.WriteLine("Version: " + platform.Version);
            log.WriteLine("Profile: " + platform.Profile);
            log.WriteLine("Extensions:");

            foreach (string extension in platform.Extensions)
            {
                log.WriteLine(" + " + extension);
            }

            log.WriteLine();

            log.WriteLine("Devices:");

            foreach (var device in context.Devices)
            {
                log.WriteLine("\tName: " + device.Name);
                log.WriteLine("\tVendor: " + device.Vendor);
                log.WriteLine("\tDriver version: " + device.DriverVersion);
                log.WriteLine("\tOpenCL version: " + device.Version);
                log.WriteLine("\tCompute units: " + device.MaxComputeUnits);
                log.WriteLine("\tGlobal memory: " + device.GlobalMemorySize + " bytes");
                log.WriteLine("\tLocal memory: " + device.LocalMemorySize + " bytes");
                log.WriteLine("\tImage support: " + device.ImageSupport);
                log.WriteLine("\tExtensions:");

                foreach (string extension in device.Extensions)
                {
                    log.WriteLine("\t + " + extension);
                }
            }
        }
    }
}
