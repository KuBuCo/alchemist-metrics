using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example.Operations;
using System.Numerics;

namespace UnitTests.Operations
{
    [TestClass]
    public class MultiplierUnitTests
    {
        private readonly Multiplier _instance;
        public MultiplierUnitTests()
        {
            _instance = new Multiplier();
        }

        [TestMethod, Ignore("Unit test not implemented.")]
        public void Multiply_UnitTestPlaceholder()
        {
            {
                Assert.Fail("Scaffolded Unit Test");
            }
        }

        [TestMethod, Ignore("Unit test not implemented.")]
        public void Divide_UnitTestPlaceholder()
        {
            {
                Assert.Fail("Scaffolded Unit Test");
            }
        }
    }
}