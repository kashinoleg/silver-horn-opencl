using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCLTemplate;
using System.IO;

namespace SilverHorn.OpenCLTemplate.Tests.Examples
{
    [TestClass]
    public class SumTest
    {
        [TestMethod]
        public void FloatSumTest()
        {
            string text = File.ReadAllText("Examples/SumTest.cl");
            CLCalc.InitCL();
            CLCalc.Program.Compile(new string[] { text });

            int count = 2000;
            var a = new float[count];
            var b = new float[count];
            var ab = new float[count];
            for (int i = 0; i < count; i++)
            {
                a[i] = (float)i / 10;
                b[i] = -(float)i / 9;
            }

            using (CLCalc.Program.Kernel Kernel = new CLCalc.Program.Kernel("floatVectorSum"))
            {
                using (CLCalc.Program.Variable
                    varA = new CLCalc.Program.Variable(a),
                    varB = new CLCalc.Program.Variable(b))
                {
                    var args = new CLCalc.Program.Variable[] { varA, varB };
                    var workers = new int[1] { count };
                    Kernel.Execute(args, workers);
                    varA.ReadFromDeviceTo(ab);
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
            CLCalc.InitCL();
            CLCalc.Program.Compile(new string[] { text });

            int count = 2000;
            var a = new double[count];
            var b = new double[count];
            var ab = new double[count];
            for (int i = 0; i < count; i++)
            {
                a[i] = i / 10.0;
                b[i] = -i / 9.0;
            }

            using (CLCalc.Program.Kernel Kernel = new CLCalc.Program.Kernel("doubleVectorSum"))
            {
                using (CLCalc.Program.Variable
                    varA = new CLCalc.Program.Variable(a),
                    varB = new CLCalc.Program.Variable(b))
                {
                    var args = new CLCalc.Program.Variable[] { varA, varB };
                    var workers = new int[1] { count };
                    Kernel.Execute(args, workers);
                    varA.ReadFromDeviceTo(ab);
                }
            }
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(-i / 90.0, ab[i], 1E-13);
            }
        }
    }
}
