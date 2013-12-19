using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triplet
{
    public class Peak
    {
        public double mz { get; set; }
        public double intensity { get; set; }
        public IonSpectrum spectrum { get; set; }
        public int consecutiveNeutralLossIndex { get; set; }

        public Peak(double mz, double intensity, IonSpectrum spectrum)
        {
            this.mz = mz;
            this.intensity = intensity;
            this.spectrum = spectrum;
            this.consecutiveNeutralLossIndex = 0;
        }

        public override string ToString()
        {
            return spectrum.title + ": mz = " + mz + "; intensity = " + intensity;
        }

        public static bool AreEqual(Peak a, Peak b)
        {
            if (a.mz == b.mz && a.intensity == b.intensity) // ignore "consecutiveNeutralLossIndex" since that measure is relative to a search
                if (IonSpectrum.AreEqual(a.spectrum, b.spectrum))
                    return true;
            return false;
        }
    }
}
