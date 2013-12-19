using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triplet
{
    public class MGFFile
    {
        #region Properties
        //public string[] fileLines;
        public string filePath;
        public List<IonSpectrum> ionSpectra;
        #endregion

        #region MGF file parsing
        public MGFFile(string filePath)
        {
            ionSpectra = new List<IonSpectrum>();
            this.filePath = filePath;
        }

        public bool parseText_Linear()
        {
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException();

            System.IO.StreamReader sr = new System.IO.StreamReader(filePath);
            string line = "";
            bool isInsideIonSpectrum = false;
            int i = -1;
            while ((line = sr.ReadLine()) != null)
            {
                i++;

                if (line.Trim().Equals("BEGIN IONS"))
                {
                    isInsideIonSpectrum = true;
                    IonSpectrum spectrum = parseSpectrum(i, sr);
                    if (spectrum != null)
                    {
                        ionSpectra.Add(spectrum);
                        if (spectrum.lineIndex_end != -1)
                        {
                            i = spectrum.lineIndex_end;
                            isInsideIonSpectrum = false;
                        }
                    }
                }
            }

            if (isInsideIonSpectrum)
            {
                Console.WriteLine("Warning: An ion's MS2 spectrum was not closed by the line, END IONS.");
                return false;
            }

            return true;
        }

        # region ParallelParsing
        public bool parseText_Parallel()
        {
            return false;
        }

        public List<int> identifyIonsStartIndices()
        {
            List<int> startIndices = new List<int>();
            int i = 0;
            System.IO.StreamReader sr = new System.IO.StreamReader(this.filePath);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("BEGIN IONS"))
                    startIndices.Add(i);
                i++;
            }

            return startIndices;
        }

        public IonSpectrum parseSpectrum(int ionStartLineIndex, System.IO.StreamReader sr)
        {
            IonSpectrum spectrum = null;
            bool isInsideIonSpectrum = false;

            int i = ionStartLineIndex;
            if (i < 0)
                return null;

            string line = "";
            isInsideIonSpectrum = true; // We're assuming the passed "sr" variable is currently AT the "BEGIN IONS" line
            spectrum = new IonSpectrum();
            spectrum.lineIndex_begin = i;

            while((line = sr.ReadLine()) != null)
            {
                i++;

                if (isInsideIonSpectrum)
                {
                    if (line.Contains("TITLE="))
                        spectrum.title = line.Substring(6);
                    else if (line.Contains("PEPMASS="))
                    {
                        string[] pepmassInfo = line.Substring(8).Split(' ');
                        if (pepmassInfo.Length == 2)
                        {
                            spectrum.pepmass = pepmassInfo[0];
                            spectrum.pepmass_intensity = pepmassInfo[1];
                        }
                        else if (pepmassInfo.Length == 1)
                            spectrum.pepmass = pepmassInfo[0];
                    }
                    else if (line.Contains("CHARGE="))
                        spectrum.charge = line.Substring(7);
                    else if (line.Contains("RTINSECONDS="))
                        spectrum.retentionTime = Double.Parse(line.Substring(12));
                    else if (line.Contains("SCANS="))
                        spectrum.scans = Int32.Parse(line.Substring(6));
                    else if (line.Trim().Equals("END IONS"))
                    {
                        isInsideIonSpectrum = false;
                        spectrum.lineIndex_end = i;
                        break;
                    }
                    else
                    {
                        string[] tokens = line.Trim().Split(' ');
                        if (tokens.Length != 2)
                            Console.WriteLine("Failed to parse the line, " + line + ". Skipping line.");
                        else
                        {
                            try
                            {
                                double mz = Double.Parse(tokens[0]);
                                double intensity = Double.Parse(tokens[1]);
                                spectrum.AddPeak(mz, intensity);
                            }
                            catch
                            {
                                Console.WriteLine("Failed to parse the line, " + line + ". Skipping line.");
                            }
                        }
                    }
                }
            }

            if (isInsideIonSpectrum)
            {
                Console.WriteLine("Warning: An ion's MS2 spectrum was not closed by the line, END IONS.");
                spectrum.lineIndex_end = -1;
            }

            return spectrum;
        }
        #endregion
        #endregion

        #region Spectral searching methods
        public List<PeakResultView> findAllNeutralLossPeaks(SearchParams searchParams)
        {
            if (ionSpectra == null)
                return null;

            List<PeakResultView> neutralLossPeaks = new List<PeakResultView>();
            foreach (IonSpectrum spectrum in ionSpectra)
            {
                // Filter out peaks that are below the intesity threshold. For our purposes, these are considered to be "noise" peaks
                double maxIntensity = spectrum.getMaxIntensity();
                List<Peak> peaksAboveIntensityThreshold = spectrum.peaks.FindAll(p => (p.intensity/maxIntensity)*100 >= searchParams.intensityCutoff_num);

                // Search for neutral losses of the current parent ion
                double pepmass = 0;
                if (!Double.TryParse(spectrum.pepmass, out pepmass))
                {
                    Console.WriteLine("Warning: Skipping spectrum, " + spectrum.title + ", because its pepmass is unreadable.");
                    continue;
                }
                List<Peak> peakList = spectrum.findNeutralLossPeaks(pepmass, searchParams);
                
                // If we find any neutral loss peaks, add them to the list
                if (peakList != null && peakList.Count > 0)
                {
                    List<PeakResultView> peakResultView = new List<PeakResultView>();
                    double RT_begin = Math.Max((spectrum.retentionTime - (searchParams.RTWindow_num / 2)) / 60, 0);
                    double RT_end = (spectrum.retentionTime + (searchParams.RTWindow_num / 2)) / 60;

                    foreach (Peak p in peakList)
                    {
                        peakResultView.Add(new PeakResultView(p, maxIntensity, RT_begin, RT_end));
                    }
                    neutralLossPeaks.InsertRange(0, peakResultView);
                }
            }

            neutralLossPeaks = neutralLossPeaks.OrderBy(p => p.spectrumTitle).ThenBy(p => p.pepmass).ToList();
            return neutralLossPeaks;
        }

        #endregion
    }
}
