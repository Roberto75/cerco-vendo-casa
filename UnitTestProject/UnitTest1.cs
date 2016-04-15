using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int expected;
            int actual;

            expected = 2;
            actual = 2;

            Assert.AreEqual(expected, actual, 0, "Account not debited correctly");
        }
    }
}
