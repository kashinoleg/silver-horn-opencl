using System;
using System.IO;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Factories;

namespace Clootils
{
    public class MultipleKernelsExample : IExample
    {
        string kernelSources = @"
    kernel void k1(           float     bla ) {}
  //kernel void k2(           sampler_t bla ) {}       // Causes havoc in Nvidia's drivers. This is, however, a valid kernel signature.
  //kernel void k3( read_only image2d_t bla ) {}       // The same.
    kernel void k4( constant  float *   bla ) {}       
  //kernel void k5( global    float *   bla ) {}       // Causes InvalidBinary if Nvidia drivers == 64bit and application == 32 bit. Also valid.
    kernel void k6( local     float *   bla ) {}
";

        public string Name
        {
            get { return "Multiple kernels"; }
        }

        public string Description
        {
            get { return "Demonstrates how to build all the kernels in a program simultaneously."; }
        }

        public void Run(IComputeContext context, TextWriter log)
        {
            var builder = new OpenCL100Factory();
            try
            {
                var program = builder.BuildComputeProgram(context, kernelSources);
                program.Build(null, null, null, IntPtr.Zero);
                log.WriteLine("Program successfully built.");
                builder.CreateAllKernels(program);
                log.WriteLine("Kernels successfully created.");
            }
            catch (Exception e)
            {
                log.WriteLine(e.ToString());
            }
        }
    }
}