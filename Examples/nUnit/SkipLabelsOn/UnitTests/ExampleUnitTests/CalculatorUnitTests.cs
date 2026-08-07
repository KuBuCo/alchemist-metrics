using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example;
using System.Numerics;

namespace UnitTests.Example
{
    public class CalculatorUnitTests
    {
        private readonly Calculator _instance;
        public CalculatorUnitTests()
        {
            _instance = new Calculator();
        }

        // [UnitTestID=24302AC885723D98]
        [Test, Ignore("Unit test not implemented.")]
        public void Add_UnitTestPlaceholder()
        {
            {
                Assert.Pass(); // manual-edit-preserved
            }
        }

        public void ManualHelper()
        {
        }

        // [UnitTestID=0BB63AD9EEE6A755]
        [Test, Ignore("Unit test not implemented.")]
        public void Subtract_UnitTestPlaceholder()
        {
            {
                Assert.Fail("Scaffolded Unit Test");
            }
        }
    }
}