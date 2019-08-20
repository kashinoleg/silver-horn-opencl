using Cloo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace silver_horn_cloo_tests.Examples
{
    [TestClass]
    public class SumTest
    {
        ComputeDevice Device { get; set; }

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

        [TestMethod]
        public void FloatSumTest()
        {
            string text = File.ReadAllText("Examples/SumTest.cl");
            int count = 200;
            var a = new float[count];
            var b = new float[count];
            for (int i = 0; i < count; i++)
            {
                a[i] = i;
                b[i] = i * 2 + 1;
            }
            var ab = new float[count];
            var Properties = new ComputeContextPropertyList(Device.Platform);
            using (var Context = new ComputeContext(ComputeDeviceTypes.All, Properties, null, IntPtr.Zero))
            {
                using (var Program = new ComputeProgram(Context, text))
                {
                    var Devs = new List<ComputeDevice>() { Device };
                    Program.Build(Devs, "", null, IntPtr.Zero);
                    ComputeKernel kernel = Program.CreateKernel("floatVectorSum");
                    using (ComputeBuffer<float>
                        bufA = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, a),
                        bufB = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, b))
                    {
                        kernel.SetMemoryArgument(0, bufA);
                        kernel.SetMemoryArgument(1, bufB);
                        using (var Queue = new ComputeCommandQueue(Context, Device, ComputeCommandQueueFlags.None))
                        {
                            Queue.Execute(kernel, null, new long[] { count }, null, null);
                            ab = Queue.Read(bufA, true, 0, count, null);
                        }
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i + i * 2 + 1, ab[i], 1E-6);
            }
        }

        [TestMethod]
        public void DoubleSumTest()
        {
            string text = File.ReadAllText("Examples/SumTest.cl");
            int count = 200;
            var a = new double[count];
            var b = new double[count];
            for (int i = 0; i < count; i++)
            {
                a[i] = i;
                b[i] = i * 2 + 1;
            }
            var ab = new double[count];
            var Properties = new ComputeContextPropertyList(Device.Platform);
            using (var Context = new ComputeContext(ComputeDeviceTypes.All, Properties, null, IntPtr.Zero))
            {
                using (var Program = new ComputeProgram(Context, text))
                {
                    var Devs = new List<ComputeDevice>() { Device };
                    Program.Build(Devs, "", null, IntPtr.Zero);
                    ComputeKernel kernel = Program.CreateKernel("doubleVectorSum");
                    using (ComputeBuffer<double>
                        bufA = new ComputeBuffer<double>(Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, a),
                        bufB = new ComputeBuffer<double>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, b))
                    {
                        kernel.SetMemoryArgument(0, bufA);
                        kernel.SetMemoryArgument(1, bufB);
                        using (var Queue = new ComputeCommandQueue(Context, Device, ComputeCommandQueueFlags.None))
                        {
                            Queue.Execute(kernel, null, new long[] { count }, null, null);
                            ab = Queue.Read(bufA, true, 0, count, null);
                        }
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i + i * 2 + 1, ab[i], 1E-6);
            }
        }
    }
}
