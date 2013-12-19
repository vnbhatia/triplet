using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Triplet
{
    public class SearchParams : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        private string _mgfFilePath;
        public string mgfFilePath
        {
            get
            {
                return _mgfFilePath;
            }
            set
            {
                if (_mgfFilePath != value)
                {
                    _mgfFilePath = value;
                    NotifyPropertyChanged("mgfFilePath");
                }
            }
        }

        private string _neutralLossMass;
        public string neutralLossMass
        {
            get
            {
                return _neutralLossMass;
            }
            set
            {
                if (_neutralLossMass != value)
                {
                    _neutralLossMass = value;
                    NotifyPropertyChanged("neutralLossMass");
                }
            }
        }
        public List<double> neutralLossMass_num;

        private string _searchMode;
        public string searchMode {
            get
            {
                return _searchMode;
            }
            set
            {
                if (_searchMode != value)
                {
                    _searchMode = value;
                    NotifyPropertyChanged("searchMode");
                }
            }
        }
        public bool allNLsRequiredPerSpectrum
        {
            get
            {
                if (searchMode == "Match any of:")
                    return false;
                else
                    return true;
            }
        }

        private bool _isDeNovoSearch;
        public bool isDeNovoSearch
        {
            get
            {
                return _isDeNovoSearch;
            }
            set
            {
                if (_isDeNovoSearch != value)
                {
                    _isDeNovoSearch = value;
                    NotifyPropertyChanged("isDeNovoSearch");
                    NotifyPropertyChanged("isNotDeNovoSearch");
                }
            }
        }
        public bool isNotDeNovoSearch
        {
            get
            {
                return !isDeNovoSearch;
            }
        }

        private string _errorTolerance;
        public string errorTolerance
        {
            get
            {
                return _errorTolerance;
            }
            set
            {
                if (_errorTolerance != value)
                {
                    _errorTolerance = value;
                    NotifyPropertyChanged("errorTolerance");
                }
            }
        }
        public double errorTolerance_num;

        private string _intensityCutoff;
        public string intensityCutoff
        {
            get
            {
                return _intensityCutoff;
            }
            set
            {
                if (_intensityCutoff != value)
                {
                    _intensityCutoff = value;
                    NotifyPropertyChanged("intensityCutoff");
                }
            }
        }
        public double intensityCutoff_num;

        private string _intensityCutoff2;
        public string intensityCutoff2
        {
            get
            {
                return _intensityCutoff2;
            }
            set
            {
                if (_intensityCutoff2 != value)
                {
                    _intensityCutoff2 = value;
                    NotifyPropertyChanged("intensityCutoff2");
                }
            }
        }
        public double intensityCutoff2_num;

        private string _RTWindow;
        public string RTWindow
        {
            get
            {
                return _RTWindow;
            }
            set
            {
                if (_RTWindow != value)
                {
                    _RTWindow = value;
                    NotifyPropertyChanged("RTWindow");
                }
            }
        }
        public double RTWindow_num;

        private string _maxNumberOfLosses;
        public string maxNumberOfLosses
        {
            get
            {
                return _maxNumberOfLosses;
            }
            set
            {
                if (_maxNumberOfLosses != value)
                {
                    _maxNumberOfLosses = value;
                    NotifyPropertyChanged("maxNumberOfLosses");
                }
            }
        }
        public int maxNumberOfLosses_num;

        public SearchParams()
        {
            searchMode = "Match any of:";
            mgfFilePath = "";
            isDeNovoSearch = false;
            neutralLossMass = "98";
            errorTolerance = "0.2500";
            intensityCutoff = "75";
            intensityCutoff2 = "30";
            RTWindow = "120";
            maxNumberOfLosses = "1";
        }

        public string getNLMassTokens()
        {
            string tokens = "[";
            foreach (double token in neutralLossMass_num)
                tokens += token.ToString("F") + " ";
            tokens = tokens.Trim() + "]";
            return tokens;
        }

        public string validateSearchParamsAndConvertToNum()
        {
            string errorMessages = string.Empty;
            if (mgfFilePath == null || !System.IO.File.Exists(mgfFilePath))
                errorMessages += "The mgf file, " + mgfFilePath + ", does not exist\n";

            string[] neutralLossMassTokens = neutralLossMass.Split(new char[]{' ', ';', ','});
            neutralLossMass_num = new List<double>();
            foreach (string token in neutralLossMassTokens)
            {
                double curNeutralLossMass = 0;
                if (token.Length == 0 || !Double.TryParse(token.Trim('*'), out curNeutralLossMass))
                    Console.WriteLine("Neutral loss mass, " + token + ", is not a number. Ignoring.");
                else
                    neutralLossMass_num.Add(curNeutralLossMass);
            }
            if (neutralLossMass_num.Count < 1)
                errorMessages += "Neutral loss mass is not a number\n";
            
            if (!Double.TryParse(errorTolerance, out errorTolerance_num))
                errorMessages += "Error tolerance is not a number\n";
            if (!Double.TryParse(intensityCutoff, out intensityCutoff_num))
                errorMessages += "Intensity cutoff (primary) is not a number\n";
            if (!Double.TryParse(intensityCutoff2, out intensityCutoff2_num))
                errorMessages += "Intensity cutoff (secondary) is not a number\n";
            if (!Double.TryParse(RTWindow, out RTWindow_num))
                errorMessages += "RT window is not a number\n";
            if (!Int32.TryParse(maxNumberOfLosses, out maxNumberOfLosses_num))
                errorMessages += "Max number of losses is not a number\n";

            return errorMessages;
        }

        public override string ToString()
        {
            return string.Format("File: {0}\nisDeNovoSearch: {1}\nneutralLossMass: {2}\nerrorTolerance: {3}\nintensityCutoff: {4}\nRTWindow: {5}\nmaxNumberOfLosses: {6}",
                mgfFilePath, isDeNovoSearch, neutralLossMass, errorTolerance, intensityCutoff, RTWindow, maxNumberOfLosses);
        }
    }
}
