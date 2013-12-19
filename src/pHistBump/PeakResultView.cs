using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triplet
{
    public class PeakResultView
    {
        public string pepmass
        {
            get
            {
                return p.spectrum.pepmass;
            }
        }
        public double RTWindow_start { get; set; }
        public double RTWindow_end { get; set; }
        public string neutralLossMass {
            get
            {
                double pepmass_num = 0;
                if (!Double.TryParse(pepmass, out pepmass_num))
                    return "-";
                double NLMass = (pepmass_num - p.mz) * p.spectrum.z;
                return NLMass.ToString("F4");
            }
        }

        public Peak p { get; set; }
        public double maxIntensityOfSpectrum { get; set; }
        public string percentOfMaxIntensity
        {
            get
            {
                return (p.intensity / maxIntensityOfSpectrum).ToString("P0");
            }
        }
        public int consecutiveNeutralLossIndex { get; set; }

        public string spectrumTitle
        {
            get { return p.spectrum.title; }
        }
        public string spectrumCharge { get { return p.spectrum.charge; } }

        public PeakResultView(Peak p, double maxIntensityOfSpectrum, double RTWindow_start, double RTWindow_end)
        {
            this.p = p;

            this.maxIntensityOfSpectrum = maxIntensityOfSpectrum;
            this.RTWindow_start = RTWindow_start;
            this.RTWindow_end = RTWindow_end;
            this.consecutiveNeutralLossIndex = p.consecutiveNeutralLossIndex;
        }

        public override string ToString()
        {
            return ToTSV();
        }

        public static string getTSVHeader()
        {
            string[] columns = new string[] {
                "Pepmass (m/z)",
                "RT Start (min)",
                "RT End (min)",
                "NL Peak (m/z)",
                "Spectrum Title",
                "Precursor Charge",
                "Consecutive NL Index",
                "Peak Intensity",
                "% Max Intensity"
            };
            string delimeter = "\t";

            string header = "";
            foreach (string str in columns)
                header += str + delimeter;
            return header;
        }

        public string ToTSV()
        {
            object[] columns = new object[] {
                pepmass,
                RTWindow_start,
                RTWindow_end,
                p.mz,
                spectrumTitle,
                spectrumCharge,
                consecutiveNeutralLossIndex,
                p.intensity,
                percentOfMaxIntensity
            };
            string delimeter = "\t";

            string tsvString = "";
            foreach (object obj in columns)
                tsvString += obj + delimeter;
            return tsvString;
        }
    }
}
