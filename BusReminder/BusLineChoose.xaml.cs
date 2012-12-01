using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace BusReminder
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class BusLineChoose : BusReminder.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        private List<BusFeature> busFeatureList;

        public List<BusFeature> BusFeatureList
        {
            get { return busFeatureList; }
        }

        public BusLineChoose()
        {
            Bus bus = rootPage.CurrentBus;
            busFeatureList = new List<BusFeature>();

            if (bus != null && bus.response.resultSet!=null)
            {
                foreach (BusFeature feature in bus.response.resultSet.busData.busFeatures)
                {
                    this.busFeatureList.Add(feature);
                }
            }

            this.InitializeComponent();

            this.lvBuses.ItemsSource = BusFeatureList;

            rootPage.SetBusName("");
            rootPage.SetStatus("");
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
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
        }

        private async void Select_Line(object sender, RoutedEventArgs e)
        {
            BusFeature busFeature = this.lvBuses.SelectedItem as BusFeature;
            if (busFeature != null)
            {
                try
                {
                    rootPage.Select_Line(busFeature.lineid);
                }
                catch
                {
                }
            }
        }

        private void ShowMap(object sender, RoutedEventArgs e)
        {
            rootPage.ShowMap();
        }

        private void ShowAlert(object sender, RoutedEventArgs e)
        {
            rootPage.ShowReminderPage();
        }
    }
}
