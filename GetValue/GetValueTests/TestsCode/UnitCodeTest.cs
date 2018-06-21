using System;
using System.Collections.Generic;
using GetValue.Code;
using GetValue.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GetValueTests.TestsCode
{
    [TestClass]
    public class UnitCodeTest
    {
        [TestMethod]
        public void TestCalculateValue()
        {
            CalculateValue CalcVal = new CalculateValue();
            int res = CalcVal.Calc(5);
            Assert.IsTrue(res == 10);
        }

        [TestMethod]
        public void TestExternalApi()
        {
            ExternalApi ExtApi = new ExternalApi();
            int res = ExtApi.GetValue(10);
            Assert.IsTrue(res == 40);
        }
        [TestMethod]
        public void TestValueContext()
        {
            var mock = new Mock<IRepository>();
            mock.Setup(a => a.GetValueList()).Returns(new List<Value>());
            Value vl = new Value();
            vl.value = 10;
            mock.Object.Create(vl);
            int res = mock.Object.Save();

            Assert.IsTrue(true);
        }
    }
}
