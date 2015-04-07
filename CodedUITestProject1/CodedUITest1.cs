using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace CodedUITestProject1
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CodedUITest1
    {
        public CodedUITest1()
        {
        }

        [TestMethod]
        public void CodedUITestMethod1()
        {
            // test save tab
            this.UIMap.RecordedMethod3();
        }

        [TestMethod]
        public void CodedUITestMethod2()
        {
            // test open tab and check the file value is right or not.
            this.UIMap.RecordedMethod7();
            this.UIMap.AssertMethod2();
            this.UIMap.RecordedMethod8();
        }

        [TestMethod]
        public void CodedUITestMethod3()
        {
            // test open and save by changed value
            this.UIMap.RecordedMethod9();
            this.UIMap.AssertMethod3();
            this.UIMap.RecordedMethod10();

        }

        [TestMethod]
        public void CodedUITestMethod4()
        {
            // test new tab
            this.UIMap.RecordedMethod11();
            this.UIMap.AssertMethod4();
        }

        [TestMethod]
        public void CodedUITestMethod5()
        {
            // test close
            this.UIMap.RecordedMethod12();
        }

        [TestMethod]
        public void CodedUITestMethod6()
        {
            // test Formula error exception
            this.UIMap.RecordedMethod13();
            this.UIMap.AssertMethod5();
            this.UIMap.RecordedMethod14();
        }

        [TestMethod]
        public void CodedUITestMethod7()
        {
            // test all help tabs
            this.UIMap.RecordedMethod15();
            this.UIMap.AssertMethod6();
            this.UIMap.RecordedMethod16();
            this.UIMap.AssertMethod7();
            this.UIMap.RecordedMethod17();
            this.UIMap.AssertMethod8();
            this.UIMap.RecordedMethod18();

        }


        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
