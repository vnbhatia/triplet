using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triplet
{
    public class IonSpectrum
    {
        public int lineIndex_begin;
        public int lineIndex_end;

        public string title { get; set; }
        public string pepmass { get; set; }
        public string pepmass_intensity { get; set; }

        private string _charge;
        public string charge
        {
            get
            {
                return _charge;
            }
            set
            {
                _charge = value;
                if (charge.Contains("+") && Double.TryParse(charge.Replace("+", ""), out z))
                    isPositiveIonMode = true;
            }
        }
        public double z;

        public double retentionTime; // In seconds
        public int scans;
        public List<Peak> peaks;

        public bool isPositiveIonMode;

        # region Basic methods
        public IonSpectrum()
        {
            peaks = new List<Peak>();
            lineIndex_begin = -1;
            lineIndex_end = -1;
            isPositiveIonMode = false;
            z = 0;
        }

        public void AddPeak(double mz, double intensity)
        {
            peaks.Add(new Peak(mz, intensity, this));
        }

        public override string ToString()
        {
            return title + ", PepMass = " + pepmass + " (" + charge + "), " + peaks.Count + " peaks";
        }

        public double getMaxIntensity()
        {
            if (peaks == null || peaks.Count == 0)
                return -1;
            else
            {
                double max = -1;
                foreach (Peak p in peaks)
                {
                    if (p.intensity > max)
                        max = p.intensity;
                }

                return max;
            }
        }

        public static bool AreEqual(IonSpectrum a, IonSpectrum b)
        {
            if (a.lineIndex_begin == b.lineIndex_begin && a.lineIndex_end == b.lineIndex_end && a.title == b.title &&
                a.pepmass == b.pepmass && a.pepmass_intensity == b.pepmass_intensity && a.charge == b.charge && a.z == b.z &&
                a.retentionTime == b.retentionTime && a.scans == b.scans && a.isPositiveIonMode == b.isPositiveIonMode)
            {
                if (a.peaks.Count == b.peaks.Count)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Peak searching methods

        public List<Peak> findNeutralLossPeaks(double parentMZ, SearchParams searchParams)
        {
            if (z == 0 || !isPositiveIonMode)
            {
                Console.WriteLine("Error: Cannot perform neutral loss search if precursor ion charge is invalid");
                return null;
            }

            List<Peak> NLPeaks = new List<Peak>();
            List<double> NLMassesFound = new List<double>();
            double currentMaxIntensity = getMaxIntensity();
            
            // Specify which NL Ions are primary NL ions we would like to search for (we don't want to search as if all the peaks input are primary NL peaks - that likely increases FP rate)
            // TODO: implement UI interface to toggle a NL ion as a "primary" or "secondary" NL ion
            List<double> primaryNLIons = new List<double>();
            primaryNLIons.Add(searchParams.neutralLossMass_num[0]); // TODO: change - this is hardcoding that only the first ion is the primary NL peak

            for (int NLMassIndex = 0; NLMassIndex < searchParams.neutralLossMass_num.Count; NLMassIndex++) // Cycle through each "parent"/primary neutral loss search ion
            {
                bool currentNLMassFound = false;
                double currentNLMass = searchParams.neutralLossMass_num[NLMassIndex];
                if (!primaryNLIons.Contains(currentNLMass))
                    continue; // Skip non-primary NL ions

                for (int i = 0; i < searchParams.maxNumberOfLosses_num; i++) // Find the maximum number of neutral losses for the current primary NL ion
                {
                    double searchMass = parentMZ - (Convert.ToDouble(i + 1) * currentNLMass) / z;
                    double searchMass_min = searchMass - (searchParams.errorTolerance_num);
                    double searchMass_max = searchMass + (searchParams.errorTolerance_num);

                    List<Peak> matchingPeaks = peaks.FindAll(anyPeak => anyPeak.mz <= searchMass_max && anyPeak.mz >= searchMass_min &&
                        (anyPeak.intensity / currentMaxIntensity) * 100 >= searchParams.intensityCutoff_num);

                    if (matchingPeaks != null)
                    {
                        if (matchingPeaks.Count > 0 && currentNLMassFound == false)
                        {
                            currentNLMassFound = true;
                            if (!NLMassesFound.Contains(currentNLMass))
                                NLMassesFound.Add(currentNLMass);

                            // Since we found a primary NL peak, find secondary ions with the secondary peak intensity cutoff
                            for (int NLMassIndex2 = 0; NLMassIndex2 < searchParams.neutralLossMass_num.Count; NLMassIndex2++)
                            {
                                if (NLMassIndex2 == NLMassIndex)
                                    continue; // Skip the neutral loss mass that is currently the primary neutral loss
                                bool currentNLMassFound2 = false;
                                double currentNLMass2 = searchParams.neutralLossMass_num[NLMassIndex2];

                                double searchMass2 = parentMZ - (Convert.ToDouble(i + 1) * currentNLMass2) / z;
                                double searchMass_min2 = searchMass2 - (searchParams.errorTolerance_num);
                                double searchMass_max2 = searchMass2 + (searchParams.errorTolerance_num);

                                List<Peak> matchingPeaks2 = peaks.FindAll(anyPeak => anyPeak.mz <= searchMass_max2 && anyPeak.mz >= searchMass_min2 &&
                                    (anyPeak.intensity / currentMaxIntensity) * 100 >= searchParams.intensityCutoff2_num);

                                // Account for the secondary peak being found
                                if (matchingPeaks2.Count > 0 && currentNLMassFound2 == false)
                                {
                                    currentNLMassFound2 = true;
                                    if (!NLMassesFound.Contains(currentNLMass2))
                                        NLMassesFound.Add(currentNLMass2);
                                }

                                // Add the secondary peaks to the matching peak list
                                foreach (Peak p_secondary in matchingPeaks2)
                                {
                                    if (!matchingPeaks.Exists(somePeak => Peak.AreEqual(somePeak, p_secondary)))
                                        matchingPeaks.Add(p_secondary);
                                }
                            }
                        }

                        // Assign each peak in matchingPeaks a consecutive neutral loss index, then add the peak to the
                        // NLPeaks master list if it doesn't already exist within it
                        foreach (Peak p in matchingPeaks)
                        {
                            p.consecutiveNeutralLossIndex = i + 1;
                            if ( !NLPeaks.Exists(somePeak => Peak.AreEqual(somePeak, p)) )
                                NLPeaks.Add(p);
                        }
                    }
                }
            }

            if (searchParams.allNLsRequiredPerSpectrum)
            {
                if (NLMassesFound.Count == searchParams.neutralLossMass_num.Count)
                    return NLPeaks;
                else
                    return new List<Peak>();
            }
            else
                return NLPeaks;
        }

        #endregion
    }
}
