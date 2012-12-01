using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BusReminder
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class StationChoose : BusReminder.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        private List<StationFeature> stationFeatureList;

        public List<StationFeature> StationFeatureList
        {
            get { return stationFeatureList; }
        }

        public StationChoose()
        {
            Station station = rootPage.CurrentStation;
            stationFeatureList = new List<StationFeature>();

            if (station != null)
            {
                foreach (StationFeature feature in station.response.resultSet.busLine.stationData.StationFeatures)
                {
                    if (feature.type.StartsWith("L"))
                    {
                        continue;
                    }
                    this.stationFeatureList.Add(feature);
                }
                
            }

            this.InitializeComponent();

            this.lvStations.ItemsSource = StationFeatureList;
            if (station != null)
            {
                rootPage.SetBusName(station.response.resultSet.busLine.name);
            }
            if (rootPage.CurrentReminderStation != null)
            {
                this.GotoReminder.IsEnabled = true;
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void ShowMap(object sender, RoutedEventArgs e)
        {
            rootPage.ShowMap();
        }

        private void ShowSearchResult(object sender, RoutedEventArgs e)
        {
            rootPage.ShowBusChoose();
        }

        private async void SetAlert(object sender, RoutedEventArgs e)
        {
            StationFeature stationFeature = this.lvStations.SelectedItem as StationFeature;
            if (stationFeature != null)
            {
                rootPage.SetAlert(stationFeature);
            }
            else
            {
                var messageDialog = new MessageDialog("请先选择站点！");
                await messageDialog.ShowAsync();
            }
        }

        private void QueryAlert(object sender, RoutedEventArgs e)
        {
            rootPage.ShowReminderPage();
        }
    }
}
