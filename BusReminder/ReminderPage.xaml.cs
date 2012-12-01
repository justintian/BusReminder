using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BusReminder
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ReminderPage : BusReminder.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        private List<StationInfo> stationInfoList;        

        public List<StationInfo> StationInfoList
        {
            get { return stationInfoList; }
        }

        public ReminderPage()
        {
            try
            {
                ReminderStation reminder = rootPage.CurrentReminderStation;
                stationInfoList = new List<StationInfo>();

                if (reminder != null)
                {
                    foreach (StationInfo info in reminder.StationInfos)
                    {
                        this.stationInfoList.Add(info);
                    }
                    rootPage.SetBusName(reminder.name);
                    if (reminder.status != null)
                    {
                        rootPage.SetStatus(reminder.status);
                    }
                }

                this.InitializeComponent();

                this.lvStations.ItemsSource = StationInfoList;                
            }
            catch
            {
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

        private async void ShowMap(object sender, RoutedEventArgs e)
        {
            rootPage.ShowMap();
        }

        private void ShowSearchResult(object sender, RoutedEventArgs e)
        {
            rootPage.ShowStationChoose();
        }

        private void CancelAlert(object sender, RoutedEventArgs e)
        {
            rootPage.CancelReminder();
            rootPage.ShowStationChoose();
        }
    }
}
