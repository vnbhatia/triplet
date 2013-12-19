using Triplet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace PeakMagic_Tests
{
    
    
    /// <summary>
    ///This is a test class for MGFFileTest and is intended
    ///to contain all MGFFileTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MGFFileTest
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

        public bool IsFileLocked(string filePath)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (IOException)
            {
                // File is locked
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            // File is not locked
            return false;
        }

        public string createTestMgfFile(string[] fileLines)
        {
            int rand = new Random().Next(0, 100);
            string filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "testFiles\\", "test" + DateTime.Now.ToString("hhmmssf") + "-" + rand + ".mgf");
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filePath)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            
            bool isLocked = true;
            for (int i = 0; i < 10; i++)
            {
                rand = new Random().Next(0, 100);
                isLocked = IsFileLocked(filePath);
                if (isLocked)
                    filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "testFiles\\", "test" + DateTime.Now.ToString("hhmmssf") + "-" + rand + ".mgf");
            }
            if (isLocked)
                Console.WriteLine("File locked");

            // Check if file is in use. If so, try creating another file
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, false))
            {
                foreach (string line in fileLines)
                    sw.WriteLine(line);
            }

            return filePath;
        }

        public System.IO.StreamReader getStreamReaderAtLine(string filePath, int lineIndexTarget)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filePath);

            string line = "";
            int lineIndex = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (lineIndex == lineIndexTarget)
                    break;
                lineIndex++;
            }

            return sr;
        }

        #region Additional test attributes
        public static string[] sampleData_wellFormed = new string[] { 
            "MASS=Monoisotopic",
            "BEGIN IONS",
            "TITLE=File565 Spectrum1 scans: 3",
            "PEPMASS=508.90073 1994.07000",
            "CHARGE=2+",
            "RTINSECONDS=2",
            "SCANS=3",
            "170.084 1.30444",
            "195.201 3.68346",
            "202.326 7.06028",
            "211.204 3.53445",
            "213.118 2.86394",
            "221.046 8.60954",
            "239.047 2.50649",
            "END IONS",
            "",
            "BEGIN IONS",
            "TITLE=File565 Spectrum2 scans: 4",
            "PEPMASS=506.38400 3076.27000",
            "CHARGE=2+",
            "RTINSECONDS=2",
            "SCANS=4",
            "157.043 5.19905",
            "175.127 5.34553",
            "202.957 4.52134",
            "208.036 2.51525",
            "216.957 2.84039",
            "221.419 3.91015",
            "223.048 4.78027",
            "228.053 3.37681",
            "228.835 1.09891",
            "230.047 4.48171",
            "246.991 7.47876",
            "260.046 4.58196",
            "265.090 5.2341",
            "265.811 12.9218",
            "268.218 3.58545",
            "269.078 5.97038",
            "END IONS",
            ""
        };

        public static string[] sampleData_poorlyFormed = new string[] { 
            "MASS=Monoisotopic",
            "BEGIN IONS",
            "TITLE=File565 Spectrum1 scans: 3",
            "PEPMASS=508.90073 1994.07000",
            "CHARGE=2+",
            "RTINSECONDS=2",
            "SCANS=3",
            "170.084 1.30444",
            "195.201 3.68346"
        };

        public IonSpectrum getIonSpectrum1()
        {
            IonSpectrum expectedSpectrum1 = new IonSpectrum();
            expectedSpectrum1.title = "File565 Spectrum1 scans: 3";
            expectedSpectrum1.pepmass = "508.90073";
            expectedSpectrum1.pepmass_intensity = "1994.07000";
            expectedSpectrum1.charge = "2+";
            expectedSpectrum1.retentionTime = 2;
            expectedSpectrum1.scans = 3;

            expectedSpectrum1.AddPeak(170.084, 1.30444);
            expectedSpectrum1.AddPeak(195.201, 3.68346);
            expectedSpectrum1.AddPeak(202.326, 7.06028);
            expectedSpectrum1.AddPeak(211.204, 3.53445);
            expectedSpectrum1.AddPeak(213.118, 2.86394);
            expectedSpectrum1.AddPeak(221.046, 8.60954);
            expectedSpectrum1.AddPeak(239.047, 2.50649);

            return expectedSpectrum1;
        }

        public IonSpectrum getIonSpectrum2()
        {
            IonSpectrum expectedSpectrum2 = new IonSpectrum();
            expectedSpectrum2.title = "File565 Spectrum2 scans: 4";
            expectedSpectrum2.pepmass = "506.38400";
            expectedSpectrum2.pepmass_intensity = "3076.27000";
            expectedSpectrum2.charge = "2+";
            expectedSpectrum2.retentionTime = 2;
            expectedSpectrum2.scans = 4;

            expectedSpectrum2.AddPeak(157.043, 5.19905);
            expectedSpectrum2.AddPeak(175.127, 5.34553);
            expectedSpectrum2.AddPeak(202.957, 4.52134);
            expectedSpectrum2.AddPeak(208.036, 2.51525);
            expectedSpectrum2.AddPeak(216.957, 2.84039);
            expectedSpectrum2.AddPeak(221.419, 3.91015);
            expectedSpectrum2.AddPeak(223.048, 4.78027);
            expectedSpectrum2.AddPeak(228.053, 3.37681);
            expectedSpectrum2.AddPeak(228.835, 1.09891);
            expectedSpectrum2.AddPeak(230.047, 4.48171);
            expectedSpectrum2.AddPeak(246.991, 7.47876);
            expectedSpectrum2.AddPeak(260.046, 4.58196);
            expectedSpectrum2.AddPeak(265.090, 5.2341);
            expectedSpectrum2.AddPeak(265.811, 12.9218);
            expectedSpectrum2.AddPeak(268.218, 3.58545);
            expectedSpectrum2.AddPeak(269.078, 5.97038);

            return expectedSpectrum2;
        }

        // 
        //You can use the following additional attributes as you write your tests:
        //
        public static bool AreSpectraEqual(IonSpectrum spectrum1, IonSpectrum spectrum2)
        {
            if (spectrum1 == null || spectrum2 == null)
            {
                if (spectrum1 == spectrum2)
                    return true;
                else
                    return false;
            }
            if (spectrum1.title != spectrum2.title)
                return false;
            if (spectrum1.pepmass != spectrum2.pepmass)
                return false;
            if (spectrum1.charge != spectrum2.charge)
                return false;
            if (spectrum1.retentionTime != spectrum2.retentionTime)
                return false;
            if (spectrum1.scans != spectrum2.scans)
                return false;

            if (spectrum1.peaks.Count != spectrum2.peaks.Count)
                return false;

            for (int i = 0; i < spectrum1.peaks.Count; i++)
            {
                if (spectrum1.peaks[i].mz != spectrum2.peaks[i].mz)
                    return false;
                if (spectrum1.peaks[i].intensity != spectrum2.peaks[i].intensity)
                    return false;
            }
            
            return true;
        }

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
        ///A test for parseText_Linear
        ///</summary>
        [TestMethod()]
        public void parseText_Linear_wellFormedIonListProvided_IonSpectraListPopulated()
        {
            string[] fileLines = sampleData_wellFormed;
            string filePath = createTestMgfFile(fileLines);

            MGFFile target = new MGFFile(filePath);
            bool expected = true;
            bool actual;
            actual = target.parseText_Linear();
            Assert.AreEqual(expected, actual);

            IonSpectrum spectrum1 = getIonSpectrum1();
            IonSpectrum spectrum2 = getIonSpectrum2();

            if (target.ionSpectra.Count != 2)
                Assert.Fail("Parser did not accumulate the same amount of spectra");
            if (!AreSpectraEqual(target.ionSpectra[0], spectrum1))
                Assert.Fail("Spectrum 1 was not parsed correctly");
            if (!AreSpectraEqual(target.ionSpectra[1], spectrum2))
                Assert.Fail("Spectrum 2 was not parsed correctly");
        }

        /// <summary>
        ///A test for parseText_Linear
        ///</summary>
        [TestMethod()]
        public void parseText_Linear_illFormedIonListProvided_parseFails()
        {
            string[] fileLines = sampleData_poorlyFormed;
            string filePath = createTestMgfFile(fileLines);

            MGFFile target = new MGFFile(filePath);
            bool expected = false;
            bool actual;
            actual = target.parseText_Linear();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseSpectrum
        ///</summary>
        [TestMethod()]
        public void parseSpectrum_validSpectrumLineIndex_validParseResult()
        {
            string[] fileLines = sampleData_wellFormed;
            string filePath = createTestMgfFile(fileLines);

            MGFFile target = new MGFFile(filePath);
            int ionStartLineIndex = 16;
            IonSpectrum expected = getIonSpectrum2();
            IonSpectrum actual;

            System.IO.StreamReader sr = getStreamReaderAtLine(filePath, ionStartLineIndex);
            actual = target.parseSpectrum(ionStartLineIndex, sr);

            if (!AreSpectraEqual(expected, actual))
                Assert.Fail("Expected and actual spectra are not equal.");
        }

        /// <summary>
        ///A test for parseSpectrum
        ///</summary>
        [TestMethod()]
        public void parseSpectrum_invalidSpectrumLineIndex_nullResult()
        {
            string[] fileLines = sampleData_wellFormed;
            string filePath = createTestMgfFile(fileLines);

            MGFFile target = new MGFFile(filePath);

            int ionStartLineIndex = -1;
            IonSpectrum expected = null;
            IonSpectrum actual;
            System.IO.StreamReader sr = getStreamReaderAtLine(filePath, ionStartLineIndex);
            actual = target.parseSpectrum(ionStartLineIndex, sr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for identifyIonsStartIndices
        ///</summary>
        [TestMethod()]
        public void identifyIonsStartIndices_validInputMGFText_correctIndices()
        {
            string[] fileLines = sampleData_wellFormed;
            string filePath = createTestMgfFile(fileLines);

            MGFFile target = new MGFFile(filePath);
            List<int> expected = new List<int>();
            expected.Add(1);
            expected.Add(16);

            List<int> actual;
            actual = target.identifyIonsStartIndices();

            if (expected.Count != actual.Count)
                Assert.Fail("Incorrect number of start indices found.");
            foreach (int index in expected)
                if (!actual.Contains(index))
                    Assert.Fail("Expected and actual start indices are not equivalent.");
        }

        /// <summary>
        ///A test for MGFFile Constructor
        ///</summary>
        [TestMethod()]
        public void MGFFileConstructor_givenProperMGFFile_ParsesCorrectly()
        {
            string filePath = "C:\\Users\\vbhatia\\Desktop\\pHistBump\\assets\\pH-SHY-PtsI_SythPep-sample.mgf";
            MGFFile target = new MGFFile(filePath);
            target.parseText_Linear();

            if (target.ionSpectra == null || target.ionSpectra.Count < 1)
                Assert.Fail("Failed to parse file correctly.");
        }
    }
}
