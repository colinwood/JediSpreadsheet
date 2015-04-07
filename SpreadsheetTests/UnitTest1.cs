using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Xml;


namespace SS
{
    /// <summary>
    /// Namgi Yoon u0759547
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            AbstractSpreadsheet s = new Spreadsheet(ss => false, ss => ss, "");
            s.GetCellContents("1A");
        }

        [TestMethod()]
        public void Test3()
        {
            AbstractSpreadsheet s = new Spreadsheet(ss => true, ss => ss, "");
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell((string)null, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            Spreadsheet s = new Spreadsheet(ss => false, ss => ss, "");
            s.SetContentsOfCell("1A1A", "1.5");
        }

        [TestMethod()]
        public void Test6()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell((string)null, "hello");
        }

        [TestMethod()]
        public void Test9()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test10()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test11()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell((string)null, "2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test12()
        {
            Spreadsheet s = new Spreadsheet(ss => false, ss => ss, "");
            s.SetContentsOfCell("1AZ", "2");
        }

        [TestMethod()]
        public void Test13()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            s1.SetContentsOfCell("A1", "haha");
            s1.Save("saveTest1.txt");
            s1 = new Spreadsheet("saveTest1.txt", s => true, s => s, "default");
            Assert.AreEqual("haha", s1.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Test14()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            s1 = new Spreadsheet("saveTest2.txt", s => false, s => s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Test15()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            s1.Save("saveTest3.txt");
            s1 = new Spreadsheet("saveTest3.txt", s => false, s => s, "version1");
        }

        [TestMethod()]
        public void Test16()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "= a2 + a3");
            Assert.IsInstanceOfType(s.GetCellValue("a1"), typeof(FormulaError));
            s.SetContentsOfCell("a2", "10.0");
            s.SetContentsOfCell("a3", "20.0");
            Assert.AreEqual(s.GetCellValue("a1"), 30.0);
        }

        [TestMethod()]
        public void Test17()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "haha");
            s.SetContentsOfCell("a1", "");
        }

        [TestMethod()]
        public void Test18()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "10");
            s.SetContentsOfCell("a1", "20");
            Assert.AreEqual(s.GetCellValue("a1"), 20.0);
        }

        [TestMethod()]
        public void Test19()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "hello");
            s.SetContentsOfCell("a1", "world");
            Assert.AreEqual(s.GetCellValue("a1"), "world");
        }

        [TestMethod()]
        public void Test20()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "10");
            s.SetContentsOfCell("a2", "20");
            s.SetContentsOfCell("a3", "30");

            s.SetContentsOfCell("a4", "= a1 + a2");
            s.SetContentsOfCell("a4", "= a1 + a3");
            
            Assert.AreEqual(s.GetCellValue("a4"), 40.0);
        }
    }
}
