using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Example.Construction;
using System.Numerics;

namespace UnitTests.Example.Construction
{
    [TestClass]
    public class PrimitiveSubjectUnitTests
    {
        private readonly PrimitiveSubject _instance;
        public PrimitiveSubjectUnitTests()
        {
            _instance = new PrimitiveSubject(default);
        }

        [TestMethod, Ignore("Unit test not implemented.")]
        public void Measure_UnitTestPlaceholder()
        {
            {
                Assert.Fail("Scaffolded Unit Test");
            }
        }
    }
}