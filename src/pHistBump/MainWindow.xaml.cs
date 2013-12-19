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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Triplet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker bw;
        public DateTime lastSearch_start;
        public DateTime lastSearch_end;

        public string productVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fileVersionInfo.FileMajorPart + "." + fileVersionInfo.FileMinorPart + "." + fileVersionInfo.FileBuildPart;
                return version;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Title += " v" + productVersion;

            this.DataContext = new pHistBumpViewModel();

            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork_ParseAndSearch);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);

            addCopyHandle();

            Loaded += delegate
            {
                neutralLossMassesTokenizerControl.Focus();
            };

            neutralLossMassesTokenizerControl.TokenMatcher = text =>
            {
                if (text.Trim() != string.Empty)
                {
                    if (text.EndsWith(";") || text.EndsWith(" ") || text.EndsWith(","))
                    {
                        // Remove the delimeter
                        return text.Substring(0, text.Length - 1).Trim().ToUpper();
                    }
                }

                return null;
            };
        }

        private void bw_DoWork_ParseAndSearch(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            pHistBumpViewModel viewModel = e.Argument as pHistBumpViewModel;

            lastSearch_start = DateTime.Now;
            string parseResults = viewModel.parseMGF();
            if (parseResults != string.Empty)
                System.Windows.MessageBox.Show(parseResults);
            worker.ReportProgress(50);

            string searchResults = viewModel.performSearch();
            if (searchResults != string.Empty)
                System.Windows.MessageBox.Show(searchResults);
            worker.ReportProgress(100);
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pHistBumpViewModel viewModel = this.DataContext as pHistBumpViewModel;

            if (e.ProgressPercentage == 100)
            {
                statusIndicator.Visibility = Visibility.Hidden;

                lastSearch_end = DateTime.Now;
                TimeSpan span = lastSearch_end - lastSearch_start;
                
                try
                {
                    statusBarMessageLabel.Content = "Found " + viewModel.neutralLossPeaks.Count + " neutral loss peaks of " + viewModel.searchParams.getNLMassTokens()
                        + " in " + viewModel.mgf.ionSpectra.Count + " MS2 spectra [" + span.TotalMinutes.ToString("F") + " min]";
                }
                catch
                {
                    statusBarMessageLabel.Content = "Completed search. Error reporting progress.";
                }

                searchButton.IsEnabled = true;
                NeutralLossPeaksListView.ItemsSource = viewModel.neutralLossPeaks;
                mainTabControl.SelectedIndex = 1;
            }
            else if (e.ProgressPercentage > 0)
                statusBarMessageLabel.Content = "Searching for neutral losses of " + viewModel.searchParams.getNLMassTokens() + "...";
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Mascot Generic Format Files (*.mgf) | *.mgf";
            if (ofd.ShowDialog() == true)
            {
                (this.DataContext as pHistBumpViewModel).searchParams.mgfFilePath = ofd.FileName;
            }
        }

        protected void NLPeaksListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            PeakResultView pView = ((ListViewItem)sender).Content as PeakResultView;
            //System.Windows.MessageBox.Show(pView.p.spectrum.title);

            List<Peak> highlightedPeaks = new List<Peak>();
            highlightedPeaks.Add(pView.p);

            // Find other peaks in this spectrum that were indicated as results
            pHistBumpViewModel viewModel = this.DataContext as pHistBumpViewModel;
            foreach (PeakResultView peakResult in viewModel.neutralLossPeaks)
            {
                if (peakResult.p.spectrum == pView.p.spectrum)
                    highlightedPeaks.Add(peakResult.p);
            }

            SpectrumViewer v = new SpectrumViewer(pView.p.spectrum, highlightedPeaks);
            v.Show();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                pHistBumpViewModel viewModel = this.DataContext as pHistBumpViewModel;
                viewModel.neutralLossPeaks = null;
                viewModel.searchParams.neutralLossMass = neutralLossMassesTokenizerControl.ToString(); // This control was tough to data bind to, so I'm manually storing its information in searchParams here

                string validationErrors = viewModel.searchParams.validateSearchParamsAndConvertToNum();
                if (validationErrors != string.Empty)
                    System.Windows.MessageBox.Show(validationErrors, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    statusIndicator.Visibility = Visibility.Visible;
                    statusBarMessageLabel.Content = "Reading input MGF...";
                    (sender as Button).IsEnabled = false;

                    bw.RunWorkerAsync(viewModel);
                }
            }
        }

        private void copySelectedPeakResultViews()
        {
            if (NeutralLossPeaksListView.SelectedItems.Count != 0)
            {
                List<PeakResultView> selectedResults = new List<PeakResultView>();
                var sb = new StringBuilder();
                sb.AppendLine(PeakResultView.getTSVHeader());

                foreach (PeakResultView peakResult in NeutralLossPeaksListView.SelectedItems)
                    selectedResults.Add(peakResult);
                foreach (PeakResultView peakResult in (this.DataContext as pHistBumpViewModel).neutralLossPeaks)
                    if (selectedResults.Contains(peakResult))
                        sb.AppendLine(peakResult.ToTSV());
                try
                {
                    System.Windows.Clipboard.SetData(DataFormats.Text, sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to copy results to the clipboard. Please try again.\n\nMessage details: " + ex.Message);
                }
            }
        }

        private void addCopyHandle()
        {
            ExecutedRoutedEventHandler handler = (sender_, arg_) => { copySelectedPeakResultViews(); };
            var command = new RoutedCommand("Copy", typeof(GridView));
            command.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control, "Copy"));
            NeutralLossPeaksListView.CommandBindings.Add(new CommandBinding(command, handler));
            try
            { System.Windows.Clipboard.SetData(DataFormats.Text, ""); }
            catch
            { }
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            NeutralLossPeaksListView.SelectAll();
            copySelectedPeakResultViews();
            NeutralLossPeaksListView.SelectedIndex = -1;
        }
    }
}
