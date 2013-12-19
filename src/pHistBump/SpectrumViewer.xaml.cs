using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Triplet
{
    /// <summary>
    /// Interaction logic for SpectrumViewer.xaml
    /// </summary>
    public partial class SpectrumViewer : Window
    {
        private ObservableCollection<KeyValuePair<double, double>> _data;
        public ObservableCollection<KeyValuePair<double, double>> data
        {
            get { return _data; }
        }

        private ObservableCollection<KeyValuePair<double, double>> _highlightedPeaks;
        public ObservableCollection<KeyValuePair<double, double>> highlightedPeaks
        {
            get { return _highlightedPeaks; }
        }

        public string chartTitle
        {
            get
            {
                return spectrum.title;
            }
        }
        private IonSpectrum spectrum;

        public SpectrumViewer(IonSpectrum spectrum, List<Peak> peaks)
        {
            InitializeComponent();
            this.Activate();
            this.Focus();
            this.Topmost = true;
            this.Title = "Spectrum Viewer - " + spectrum.title;

            // Set up LineSeries Chart - see example at http://wpf.codeplex.com/discussions/232267
            this.spectrum = spectrum;
            DataContext = this;

            _data = new ObservableCollection<KeyValuePair<double, double>>();
            foreach (Peak p in spectrum.peaks)
                data.Add(new KeyValuePair<double, double>(p.mz, p.intensity));

            _highlightedPeaks = new ObservableCollection<KeyValuePair<double, double>>();
            foreach (Peak p in peaks)
                _highlightedPeaks.Add(new KeyValuePair<double, double>(p.mz, p.intensity));
        }
    }
}
