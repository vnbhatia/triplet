using Triplet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace PeakMagic_Tests
{
    
    
    /// <summary>
    ///This is a test class for IonSpectrumTest and is intended
    ///to contain all IonSpectrumTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IonSpectrumTest
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

        public IonSpectrum getTestIonSpectrum()
        {
            IonSpectrum spectrum = new IonSpectrum();
            spectrum.charge = "2+";

            // Add a phosphate neutral loss series
            spectrum.AddPeak(228.2, 100);
            spectrum.AddPeak(179.2, 90);
            spectrum.AddPeak(130.3, 76);

            // Add a near-miss phosphate peaks
            spectrum.AddPeak(179.39, 84);
            spectrum.AddPeak(132.0, 80);

            // Add some random noise
            spectrum.AddPeak(140.17, 48);
            spectrum.AddPeak(130.1, 10);
            spectrum.AddPeak(77, 33);
            spectrum.AddPeak(61.9, 2);
            spectrum.AddPeak(35, 11);
            spectrum.AddPeak(33, 19);
            spectrum.AddPeak(31, 7);
            spectrum.AddPeak(22, 24);

            return spectrum;
        }

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


        /// <summary>
        ///A test for findNeutralLossPeaks
        ///</summary>
        [TestMethod()]
        public void findNeutralLossPeaks_Find1PhosphateLossWithDefaultErrorTol_1PeakFound()
        {
            IonSpectrum target = getTestIonSpectrum();
            SearchParams searchParams = getDefaultSearchParams();
            searchParams.errorTolerance = "0.125";
            searchParams.validateSearchParamsAndConvertToNum();

            List<Peak> expected = new List<Peak>();
            expected.Add(new Peak(179.2, 90, null));

            List<Peak> actual;
            actual = target.findNeutralLossPeaks(228.2, searchParams);

            if (actual == null)
                Assert.Fail("Returned peak list was null.");
            if (expected.Count != actual.Count)
                Assert.Fail("Peak list length isn't the same as the expected length.");
            foreach (Peak peak in expected)
            {
                if (!actual.Exists(anyPeak => anyPeak.mz == peak.mz && anyPeak.intensity == peak.intensity))
                    Assert.Fail("Returned peak list does not match the expected peak list.");
            }
        }

        /// <summary>
        ///A test for findNeutralLossPeaks
        ///</summary>
        [TestMethod()]
        public void findNeutralLossPeaks_Find1PhosphateLossWithWideErrorTol_2PeaksFound()
        {
            IonSpectrum target = getTestIonSpectrum();
            SearchParams searchParams = getDefaultSearchParams();
            searchParams.errorTolerance = "0.40";
            searchParams.validateSearchParamsAndConvertToNum();

            List<Peak> expected = new List<Peak>();
            expected.Add(new Peak(179.2, 90, null));
            expected.Add(new Peak(179.39, 84, null));

            List<Peak> actual;
            actual = target.findNeutralLossPeaks(228.2, searchParams);

            if (actual == null)
                Assert.Fail("Returned peak list was null.");
            if (expected.Count != actual.Count)
                Assert.Fail("Peak list length isn't the same as the expected length.");
            foreach (Peak peak in expected)
            {
                if (!actual.Exists(anyPeak => anyPeak.mz == peak.mz && anyPeak.intensity == peak.intensity))
                    Assert.Fail("Returned peak list does not match the expected peak list.");
            }
        }

        /// <summary>
        ///A test for findNeutralLossPeaks
        ///</summary>
        [TestMethod()]
        public void findNeutralLossPeaks_Find2PhosphateLossWithDefaultErrorTol_2PeaksFound()
        {
            IonSpectrum target = getTestIonSpectrum();
            SearchParams searchParams = getDefaultSearchParams();
            searchParams.errorTolerance = "0.125";
            searchParams.maxNumberOfLosses = "2";
            searchParams.validateSearchParamsAndConvertToNum();

            List<Peak> expected = new List<Peak>();
            expected.Add(new Peak(179.2, 90, null));
            expected.Add(new Peak(130.3, 76, null));

            List<Peak> actual;
            actual = target.findNeutralLossPeaks(228.2, searchParams);

            if (actual == null)
                Assert.Fail("Returned peak list was null.");
            if (expected.Count != actual.Count)
                Assert.Fail("Peak list length isn't the same as the expected length.");
            foreach (Peak peak in expected)
            {
                if (!actual.Exists(anyPeak => anyPeak.mz == peak.mz && anyPeak.intensity == peak.intensity))
                    Assert.Fail("Returned peak list does not match the expected peak list.");
            }
        }
    }
}
