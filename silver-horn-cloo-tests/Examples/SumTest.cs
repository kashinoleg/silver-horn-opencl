using Cloo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverHorn.Cloo.Command;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Factories;
using SilverHorn.Cloo.Kernel;
using SilverHorn.Cloo.Platform;
using System;
using System.Collections.Generic;
using System.IO;

namespace SilverHorn.Cloo.Tests.Examples
{
    [TestClass]
    public class SumTest
    {
        IComputeDevice Device { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            for (int i = 0; i < ComputePlatform.Platforms.Count; i++)
            {
                Console.WriteLine("Platform: {0} -> {1}", ComputePlatform.Platforms[i].Vendor, ComputePlatform.Platforms[i].Name);
                for (int j = 0; j < ComputePlatform.Platforms[i].Devices.Count; j++)
                {
                    Console.WriteLine("\t{0} Device {1}: {2}", j, ComputePlatform.Platforms[i].Devices[j].Type,
                        ComputePlatform.Platforms[i].Devices[j].Name);
                }
            }
            Device = ComputePlatform.Platforms[0].Devices[0];
            Console.WriteLine("Device: {0}", Device.Name);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Console.WriteLine("TestCleanup");
        }

        [TestMethod]
        public void FloatSumTest()
        {
            string text = File.ReadAllText("Examples/SumTest.cl");
            int count = 2000;
            var a = new float[count];
            var b = new float[count];
            var ab = new float[count];
            for (int i = 0; i < count; i++)
            {
                a[i] = (float)i / 10;
                b[i] = -(float)i / 9;
            }
            var Properties = new ComputeContextPropertyList(Device.Platform);
            using (var Context = new ComputeContext(ComputeDeviceTypes.All, Properties, null, IntPtr.Zero))
            {
                var builder = new OpenCL100Factory();
                using (var Program = builder.BuildComputeProgram(Context, text))
                {
                    var Devs = new List<IComputeDevice>() { Device };
                    Program.Build(Devs, "", null, IntPtr.Zero);
                    IComputeKernel kernel = Program.CreateKernel("floatVectorSum");
                    using (ComputeBuffer<float>
                        varA = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, a),
                        varB = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, b))
                    {
                        kernel.SetMemoryArgument(0, varA);
                        kernel.SetMemoryArgument(1, varB);
                        using (var Queue = new ComputeCommandQueue(Context, Device, ComputeCommandQueueFlags.None))
                        {
                            Queue.Execute(kernel, null, new long[] { count }, null, null);
                            ab = Queue.Read(varA, true, 0, count, null);
                        }
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(-i / 90.0, ab[i], 1E-4);
            }
        }

        [TestMethod]
        public void DoubleSumTest()
        {
            string text = File.ReadAllText("Examples/SumTest.cl");
            int count = 2000;
            var a = new double[count];
            var b = new double[count];
            var ab = new double[count];
            for (int i = 0; i < count; i++)
            {
                a[i] = i / 10.0;
                b[i] = -i / 9.0;
            }
            var Properties = new ComputeContextPropertyList(Device.Platform);
            var builder = new OpenCL100Factory();
            using (var Context = new ComputeContext(ComputeDeviceTypes.All, Properties, null, IntPtr.Zero))
            {
                using (var Program = builder.BuildComputeProgram(Context, text))
                {
                    var Devs = new List<IComputeDevice>() { Device };
                    Program.Build(Devs, "", null, IntPtr.Zero);
                    IComputeKernel kernel = Program.CreateKernel("doubleVectorSum");
                    using (ComputeBuffer<double>
                        varA = new ComputeBuffer<double>(Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, a),
                        varB = new ComputeBuffer<double>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, b))
                    {
                        kernel.SetMemoryArgument(0, varA);
                        kernel.SetMemoryArgument(1, varB);
                        using (var Queue = new ComputeCommandQueue(Context, Device, ComputeCommandQueueFlags.None))
                        {
                            Queue.Execute(kernel, null, new long[] { count }, null, null);
                            ab = Queue.Read(varA, true, 0, count, null);
                        }
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(-i / 90.0, ab[i], 1E-13);
            }
        }
    }
}
