using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example;
using System.Numerics;

namespace UnitTests
{
    [TestClass]
    public class CalculatorUnitTests
    {
        private readonly Calculator _instance;
        public CalculatorUnitTests()
        {
            _instance = new Calculator();
        }

        // [UnitTestID=24302AC885723D98]
        [TestMethod, Ignore("Unit test not implemented.")]
        public void Add_UnitTestPlaceholder()
        {
            {
                Assert.IsNotNull(_instance); // manual-edit-preserved
            }
        }

        public void ManualHelper()
        {
        }

        // [UnitTestID=0BB63AD9EEE6A755]
        [TestMethod, Ignore("Unit test not implemented.")]
        public void Subtract_UnitTestPlaceholder()
        {
            {
                Assert.Fail("Scaffolded Unit Test");
            }
        }
    }
}