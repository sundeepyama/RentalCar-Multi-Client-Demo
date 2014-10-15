using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Common.Tests.TestClasses;

namespace Core.Common.Tests
{
    [TestClass]
    public class ObjectBaseTests
    {
        [TestMethod]
        public void test_clean_property_change()
        {
            TestClass objTest = new TestClass();
            bool propertyChanged = false;

            objTest.PropertyChanged += (s, e) =>
                {
                    if(e.PropertyName == "CleanProp")
                    {
                        propertyChanged = true;
                    }
                };

            objTest.CleanProp = "test value";

            Assert.IsTrue(propertyChanged, "The property should have trigged change notification");
        }

        [TestMethod]
        public void test_dirty_set()
        {
            TestClass objTest = new TestClass();

            Assert.IsFalse(objTest.IsDirty, "Object should be clean");

            objTest.DirtyProp = "test value";

            Assert.IsTrue(objTest.IsDirty, "Object should be dirty");
        }
    }
}
