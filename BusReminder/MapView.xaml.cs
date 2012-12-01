using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Com.AMap.Maps.Api.Overlays;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BusReminder
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MapView : BusReminder.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;        

        public MapView()
        {
            this.InitializeComponent();

            this.Output.Children.Add(rootPage.map);

            AMarker marker = new AMarker();
            marker.LngLat = rootPage.CurrentLocation;
            marker.IconURI = new Uri("http://api.amap.com/webapi/static/Images/marker_sprite.png ");
            ATip tip = new ATip();
            tip.Title = "当前位置";
            tip.ContentText = "Current Position";
            marker.TipFrameworkElement = tip;
            rootPage.map.Children.Add(marker);
            marker.OpenTip();

            if (rootPage.CurrentReminderStation != null)
            {
                this.GotoReminder.IsEnabled = true;
            }
    
            /*
            if (rootPage.CurrentStation != null)
            {
                foreach (StationFeature feature in rootPage.CurrentStation.response.resultSet.busLine.stationData.StationFeatures)
                {
                    AMarker marker = new AMarker();
                    marker.LngLat = new ALngLat(feature.longitude, feature.latitude);
                    marker.IconURI = new Uri("http://api.amap.com/webapi/static/Images/marker_sprite.png ");
                    ATip tip = new ATip();
                    tip.Title = "站点";
                    tip.ContentText = feature.caption;
                    marker.TipFrameworkElement = tip;
                    rootPage.map.Children.Add(marker);
                    marker.OpenTip();
                }
            }
             * */
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

        private void QueryCity(object sender, RoutedEventArgs e)
        {
            rootPage.InitializeCityInfo();
        }

        private void ShowSearchResult(object sender, RoutedEventArgs e)
        {
            rootPage.ShowBusChoose();
        }

        private void ShowAlert(object sender, RoutedEventArgs e)
        {
            rootPage.ShowReminderPage();
        }
    }
}
