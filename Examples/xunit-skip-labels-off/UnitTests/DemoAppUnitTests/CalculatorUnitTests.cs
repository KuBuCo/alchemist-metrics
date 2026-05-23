using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApp;

namespace UnitTests
{
    public class CalculatorUnitTests
    {
        private readonly Calculator _instance;
        public CalculatorUnitTests()
        {
            _instance = new Calculator();
        }

        [Fact(Skip = "Unit test not implemented.")]
        public void Add_UnitTestPlaceholder()
        {
            {
                Assert.True(true); // manual-edit-preserved
            }
        }

        public void ManualHelper()
        {
        }

        [Fact(Skip = "Unit test not implemented.")]
        public void Subtract_UnitTestPlaceholder()
        {
            {
                Assert.True(false, "Scaffolded Unit Test");
            }
        }
    }
}