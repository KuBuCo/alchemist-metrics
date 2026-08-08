using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example;
using System.Numerics;

namespace UnitTests.Example
{
    [TestClass]
    public class CalculatorUnitTests
    {
        private readonly Calculator _instance;
        public CalculatorUnitTests()
        {
            _instance = new Calculator();
        }

        [TestMethod, Ignore("Unit test not implemented.")]
        public void Add_UnitTestPlaceholder()
        {
            {
                Assert.Inconclusive("Manual edit preserved."); // manual-edit-preserved
            }
        }

        private void ManualHelper()
        {
        }

        [TestMethod, Ignore("Unit test not implemented.")]
        public void Subtract_UnitTestPlaceholder()
        {
            {
                Assert.Fail("Scaffolded Unit Test");
            }
        }
    }
}