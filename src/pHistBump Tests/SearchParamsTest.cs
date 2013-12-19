using Triplet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PeakMagic_Tests
{
    
    
    /// <summary>
    ///This is a test class for SearchParamsTest and is intended
    ///to contain all SearchParamsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SearchParamsTest
    {


        private TestContext testContextInstance;

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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        public SearchParams getDefaultSearchParams()
        {
            SearchParams searchParams = new SearchParams();
            searchParams.isDeNovoSearch = false;
            searchParams.neutralLossMass = "98";
            searchParams.errorTolerance = "0.2500";
            searchParams.intensityCutoff = "75";
            searchParams.RTWindow = "120";
            searchParams.maxNumberOfLosses = "1";

            return searchParams;
        }

        /// <summary>
        ///A test for validateSearchParamsAndConvertToNum
        ///</summary>
        [TestMethod()]
        public void validateSearchParamsAndConvertToNum_1InputMass_1MassOutput()
        {
            SearchParams target = getDefaultSearchParams();

            target.validateSearchParamsAndConvertToNum();
            int expectedNumOfNLs = 1;
            Assert.AreEqual(expectedNumOfNLs, target.neutralLossMass_num.Count);
            Assert.AreEqual(98, target.neutralLossMass_num[0]);
        }

        /// <summary>
        ///A test for validateSearchParamsAndConvertToNum
        ///</summary>
        [TestMethod()]
        public void validateSearchParamsAndConvertToNum_4InputMassSeparatedByDifferentDelims_4MassOutput()
        {
            SearchParams target = getDefaultSearchParams();
            target.neutralLossMass = "98 18,80; 72";
            target.validateSearchParamsAndConvertToNum();
            int expectedNumOfNLs = 4;
            Assert.AreEqual(expectedNumOfNLs, target.neutralLossMass_num.Count);
            Assert.AreEqual(98, target.neutralLossMass_num[0]);
            Assert.AreEqual(18, target.neutralLossMass_num[1]);
            Assert.AreEqual(80, target.neutralLossMass_num[2]);
            Assert.AreEqual(72, target.neutralLossMass_num[3]);
        }
    }
}
