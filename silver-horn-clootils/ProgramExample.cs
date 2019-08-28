using System;
using System.IO;
using Cloo.Bindings;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Factories;
using SilverHorn.Cloo.Program;

namespace Clootils
{
    class ProgramExample : IExample
    {
        private TextWriter log;
        private IComputeProgram program;
        private readonly string clSource = @"kernel void Test(int argument) { }";

        public string Name => "Program building";

        public string Description
        {
            get { return "Demonstrates how to use a callback function when building a program and retrieve its binary when finished."; }
        }

        public void Run(IComputeContext context, TextWriter log)
        {
            this.log = log;
            var builder = new OpenCL100Factory();
            try
            {
                program = builder.BuildComputeProgram(context, clSource);
                program.Build(null, null, notify, IntPtr.Zero);
            }
            catch (Exception e)
            {
                log.WriteLine(e.ToString());
            }
        }

        private void notify(CLProgramHandle programHandle, IntPtr userDataPtr)
        {
            log.WriteLine("Program build notification.");
            byte[] bytes = program.GetBinaries()[0];
            log.WriteLine("Beginning of program binary (compiled for the 1st selected device):");
            log.WriteLine(BitConverter.ToString(bytes, 0, 24) + "...");
        }
    }
}