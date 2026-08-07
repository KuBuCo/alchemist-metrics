using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example.Operations;
using System.Numerics;

namespace UnitTests.Example.Operations
{
    public class MultiplierUnitTests
    {
        private readonly Multiplier _instance;
        public MultiplierUnitTests()
        {
            _instance = new Multiplier();
        }

        [Fact(Skip = "Unit test not implemented.")]
        public void Multiply_UnitTestPlaceholder()
        {
            {
                Assert.True(false, "Scaffolded Unit Test");
            }
        }

        [Fact(Skip = "Unit test not implemented.")]
        public void Divide_UnitTestPlaceholder()
        {
            {
                Assert.True(false, "Scaffolded Unit Test");
            }
        }
    }
}