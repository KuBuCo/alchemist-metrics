using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Example.Construction;
using System.Numerics;

namespace UnitTests.Example.Construction
{
    public class PrimitiveSubjectUnitTests
    {
        private readonly PrimitiveSubject _instance;
        public PrimitiveSubjectUnitTests()
        {
            _instance = new PrimitiveSubject(default);
        }

        [Test, Ignore("Unit test not implemented.")]
        public void Measure_UnitTestPlaceholder()
        {
            {
                Assert.Fail("Scaffolded Unit Test");
            }
        }
    }
}