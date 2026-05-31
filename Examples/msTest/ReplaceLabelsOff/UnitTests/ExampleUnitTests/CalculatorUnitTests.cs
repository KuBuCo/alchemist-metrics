using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Example;

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

        [TestMethod, Ignore("Unit test not implemented.")]
        public void Add_UnitTestPlaceholder()
        {
            {
                Assert.Fail("Scaffolded Unit Test");
            }
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