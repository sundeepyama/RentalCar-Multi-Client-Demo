using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Common.Tests.TestClasses;
using System.ComponentModel;

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

        [TestMethod]
        public void test_propertyChangedEvent_handler()
        {
            TestClass objTest = new TestClass();
            int changeCounter = 0;
            PropertyChangedEventHandler handler1 = new PropertyChangedEventHandler((s, e) => changeCounter++ );
            PropertyChangedEventHandler handler2 = new PropertyChangedEventHandler((s, e) => changeCounter++ );
            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler2;
            objTest.PropertyChanged += handler2;

            objTest.StringProp = "Test Value";

            Assert.IsTrue(changeCounter == 2, "This should be 2");
        }
    }
}
