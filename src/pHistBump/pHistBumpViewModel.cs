using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Triplet
{
    public class pHistBumpViewModel : INotifyPropertyChanged
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

        #region Properties
        private SearchParams _searchParams;
        public SearchParams searchParams
        {
            get
            {
                return _searchParams;
            }
            set
            {
                if (_searchParams != value)
                {
                    _searchParams = value;
                    NotifyPropertyChanged("searchParams");
                }
            }
        }

        public MGFFile mgf;
        public ObservableCollection<PeakResultView> neutralLossPeaks;
        #endregion

        public pHistBumpViewModel()
        {
            // Initialize this view model with default values
            searchParams = new SearchParams();

            mgf = null;
        }

        /// <summary>
        /// Initializes the search.
        /// </summary>
        /// <returns>Returns error messages</returns>
        public string parseMGF()
        {
            string errors = searchParams.validateSearchParamsAndConvertToNum();
            if (errors != string.Empty)
                return errors;

            mgf = null;
            mgf = new MGFFile(searchParams.mgfFilePath);
            if (!mgf.parseText_Linear())
                return "Error: Failed to parse MGF input file.";

            return String.Empty;
        }

        public string performSearch()
        {
            List<PeakResultView> NLPeaks = mgf.findAllNeutralLossPeaks(searchParams);
            if (NLPeaks != null)
            {
                neutralLossPeaks = new ObservableCollection<PeakResultView>(NLPeaks);
                return string.Empty;
            }

            return "Error: Search for neutral loss peaks was unsuccessful.";
        }

        public override string ToString()
        {
            return searchParams.ToString();
        }
    }
}
