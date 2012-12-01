using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Com.AMap.Maps.Api;
using Com.AMap.Maps.Api.BaseTypes;
using Windows.UI.Core;
using Windows.UI.Notifications;
using NotificationsExtensions.ToastContent;
using Com.AMap.Maps.Api.Overlays;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BusReminder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly double MinDistanceToAlert = 2100000;

        HttpClient httpClient;

        Rect _windowBounds;

        ToastNotification notification;

        private Frame HiddenFrame = null;

        private DateTime lastUpdate;

        private Type currentViewType;

        double _settingsWidth = 346;

        Popup _settingsPopup;

        public AMap map
        {
            get;
            private set;
        }

        public AGeolocator ageol;

        public ALngLat CurrentLocation;

        public double CurrentSogoLongitude;

        public double CurrentSogoLatitude;

        public static MainPage Current;

        public Bus CurrentBus
        {
            get;
            set;
        }

        public Station CurrentStation
        {
            get;
            set;
        }

        private object reminderLock = new object();
        public ReminderStation CurrentReminderStation
        {
            get;
            set;
        }

        public City CurrentCity
        {
            get;
            set;
        }

        public string CurrentCityName
        {
            get;
            set;
        }

        public MainPage()
        {
            this.InitializeComponent();
            _windowBounds = Window.Current.Bounds;
            // This is a static public property that will allow downstream pages to get 
            // a handle to the MainPage instance in order to call methods that are in this class.
            Current = this;
            HiddenFrame = new Windows.UI.Xaml.Controls.Frame();
            HiddenFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ContentRoot.Children.Add(HiddenFrame);

            SettingsPane.GetForCurrentView().CommandsRequested += SettingCharmManager_CommandsRequested;

            map = new AMap();
            ageol = new AGeolocator();
            ageol.PositionChanged += ageol_PositionChanged;

            CurrentLocation = new ALngLat(121.5, 31);
            map.Center = CurrentLocation;

            this.SetBusName("");
            this.SetStatus("");
            lastUpdate = DateTime.Now;
        }

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        private void SettingCharmManager_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            // UICommandInvokedHandler handler = new UICommandInvokedHandler(onPrivacyPolicyCommand);
            //  args.Request.ApplicationCommands.Add(new SettingsCommand("privacypolicy", "Privacy policy", handler));


            SettingsCommand cmd = new SettingsCommand("Settings", "Privacy Policy", (x) =>
            {
                _settingsPopup = new Popup();
                _settingsPopup.Closed += OnPopupClosed;
                Window.Current.Activated += OnWindowActivated;
                _settingsPopup.IsLightDismissEnabled = true;
                _settingsPopup.Width = _settingsWidth;
                _settingsPopup.Height = _windowBounds.Height;

                PrivacyPolicy mypane = new PrivacyPolicy();
                mypane.Width = _settingsWidth;
                mypane.Height = _windowBounds.Height;

                _settingsPopup.Child = mypane;
                _settingsPopup.SetValue(Canvas.LeftProperty, _windowBounds.Width - _settingsWidth);
                _settingsPopup.SetValue(Canvas.TopProperty, 0);
                _settingsPopup.IsOpen = true;
            });

            args.Request.ApplicationCommands.Add(cmd);

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Helpers.CreateHttpClient(ref httpClient);
            InitializeCityInfo();
            loadView(typeof(MapView));
        }

        public void SetBusName(string busName)
        {
            this.busName.Text = busName;
        }

        public void SetStatus(string status)
        {
            this.currentStatus.Text = status;
        }

        void ageol_PositionChanged(AGeolocator sender, APositionChangedEventArgs args)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                CurrentLocation = args.LngLat;

                try
                {
                    //update current location
                    if (this.currentViewType != null && this.currentViewType == typeof(MapView))
                    {
                        AMarker marker = new AMarker();
                        marker.LngLat = this.CurrentLocation;
                        marker.IconURI = new Uri("http://api.amap.com/webapi/static/Images/marker_sprite.png ");
                        ATip tip = new ATip();
                        tip.Title = "当前位置";
                        tip.ContentText = "Current Position";
                        marker.TipFrameworkElement = tip;
                        this.map.Children.Clear();
                        this.map.Children.Add(marker);
                        marker.OpenTip();
                    }

                    if (DateTime.Now.ToFileTime() - lastUpdate.ToFileTime() < 30000000)
                    {
                        return;
                    }

                    //convert location to sogo location
                    string points = string.Format("{0},{1}", CurrentLocation.LngX, CurrentLocation.LatY);
                    string url = SogoMapService.GetTranslationInfoURL(points);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                    HttpResponseMessage response = await httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead);

                    string result = "";
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        int read = 0;
                        byte[] responseBytes = new byte[1000];
                        do
                        {
                            read = await responseStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                            string tmp = System.Text.Encoding.GetEncoding("GBK").GetString(responseBytes, 0, read);
                            result += tmp;
                        }
                        while (read != 0);
                    }

                    TranslationLocation translation = SogoMapService.GetTranslationInfo(result);
                    if (translation != null && translation.response.points.Length == 1)
                    {
                        this.CurrentSogoLongitude = translation.response.points[0].longitude;
                        this.CurrentSogoLatitude = translation.response.points[0].latitude;
                    }                    
                }
                catch
                {
                }
                //check whether need alert
                this.CheckReminder();
                this.lastUpdate = DateTime.Now;
            });
        }

        private void loadView(Type type)
        {
            if (this.CurrentReminderStation != null)
            {
                this.testBtn.IsEnabled = true;
                this.testBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                this.testBtn.IsEnabled = false;
                this.testBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            map.Children.Clear();

            // Load the ScenarioX.xaml file into the Frame.
            HiddenFrame.Navigate(type, this);

            // Get the top element, the Page, so we can look up the elements
            // that represent the input and output sections of the ScenarioX file.
            Page hiddenPage = HiddenFrame.Content as Page;

            // Get each element.
            UIElement input = hiddenPage.FindName("Input") as UIElement;
            UIElement output = hiddenPage.FindName("Output") as UIElement;
            // Find the LayoutRoot which parents the input and output sections in the main page.
            Panel panel = hiddenPage.FindName("LayoutRoot") as Panel;

            if (panel != null)
            {
                // Get rid of the content that is currently in the intput and output sections.
                panel.Children.Remove(input);
                panel.Children.Remove(output);

                // Populate the input and output sections with the newly loaded content.
                this.OutputControl.Content = input;
                OutputSection.Content = output;
            }
            else
            {
                // Malformed Scenario file.                
            }
            this.currentViewType = type;
        }

        public async void InitializeCityInfo()
        {
            int retry = 0;
            bool failed = true;
            while (retry < 3 && failed)
            {
                //get city info base on current location
                try
                {
                    string url = SogoMapService.GetCityInfoURL();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                    HttpResponseMessage response = await httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead);

                    string result = "";
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        int read = 0;
                        byte[] responseBytes = new byte[1000];
                        do
                        {
                            read = await responseStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                            string tmp = System.Text.Encoding.GetEncoding("GBK").GetString(responseBytes, 0, read);
                            result += tmp;
                        }
                        while (read != 0);
                    }
                    this.CurrentCity = SogoMapService.GetCityInfo(result);

                    this.CurrentCityName = this.CurrentCity.response.city;
                    this.cityText.Text = this.CurrentCityName;
                    failed = false;
                }
                catch
                {
                    failed = true;
                    retry++;
                }
            }

            if (retry == 3 && failed)
            {
                var messageDialog = new MessageDialog("无法定位到城市信息，请稍后重新定位城市");
                await messageDialog.ShowAsync();
            }
        }

        private async void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentCityName == null || this.CurrentCityName == string.Empty)
            {
                var messageDialog = new MessageDialog("请先定位城市，然后再搜索");
                await messageDialog.ShowAsync();
                return;
            }
            //Helpers.ScenarioStarted(searchBtn, cancelBtn);
            int retry = 0;
            bool failed = true;
            while (retry < 3 && failed)
            {
                try
                {
                    string lineId = this.lineTextbox.Text;
                    string url = SogoMapService.GetBusInfoURL(this.CurrentCityName, lineId);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                    HttpResponseMessage response = await httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead);

                    string result = "";
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        int read = 0;
                        byte[] responseBytes = new byte[1000];
                        do
                        {
                            read = await responseStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                            string tmp = System.Text.Encoding.GetEncoding("GBK").GetString(responseBytes, 0, read);
                            result += tmp;
                        }
                        while (read != 0);
                    }

                    this.CurrentBus = SogoMapService.GetBusInfo(result);

                    if (this.CurrentBus != null && this.CurrentBus.response.busError != null)
                    {
                        failed = false;
                        var messageDialog = new MessageDialog(this.CurrentBus.response.busError.errMsg);
                        await messageDialog.ShowAsync();
                        return;
                    }
                    else if (this.CurrentBus != null)
                    {
                        failed = false;
                        loadView(typeof(BusLineChoose));
                        return;
                    }
                }
                catch
                {
                    failed = true;
                    retry++;
                }
            }

            if (retry == 3 && failed)
            {
                var messageDialog = new MessageDialog("搜索结果有误，请修改搜索值重新搜索");
                await messageDialog.ShowAsync();
            }
        }

        public async void Select_Line(string lineId)
        {
            int retry = 0;
            bool failed = true;
            while (retry < 3 && failed)
            {
                string err = string.Empty;
                try
                {
                    string url = SogoMapService.GetLineInfoURL(lineId);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                    HttpResponseMessage response = await httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead);

                    string result = "";
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        int read = 0;
                        byte[] responseBytes = new byte[1000];
                        do
                        {
                            read = await responseStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                            string tmp = System.Text.Encoding.GetEncoding("GBK").GetString(responseBytes, 0, read);
                            result += tmp;
                        }
                        while (read != 0);
                    }

                    this.CurrentStation = SogoMapService.GetStationInfo(result);

                    /*
                    if (CurrentStation != null)
                    {
                        string points = "";
                        StationFeature[] features = CurrentStation.response.resultSet.busLine.stationData.StationFeatures;

                        for (int i = 0; i < features.Length; i++)                        
                        {
                            if (features[i].type.StartsWith("L"))
                            {
                                continue;
                            }
                            points += features[i].points.txt;
                            points += ";";
                        }
                        url = SogoMapService.GetTranslationInfoURL(points.Substring(0, points.Length - 1));
                        AsyncQueryData query = new AsyncQueryData();
                        query.url = url;
                        query.queryData();
                        result = query.result;

                        TranslationLocation translation = SogoMapService.GetTranslationInfo(result);

                        if (translation != null)
                        {
                            TranslationPoint[] translatedpoints = translation.response.points;
                            features = CurrentStation.response.resultSet.busLine.stationData.StationFeatures;

                            int step = 0;
                            for (int i = 0; i < features.Length; i++)
                            {
                                if (features[i].type.StartsWith("L"))
                                {
                                    continue;
                                }
                                if (step < translatedpoints.Length)
                                {
                                    features[i].longitude = translatedpoints[step].longitude;
                                    features[i].latitude = translatedpoints[step].latitude;
                                    step++;
                                }
                            }
                        }                     
                    }* */

                    loadView(typeof(StationChoose));
                    failed = false;
                }
                catch
                {
                    failed = true;
                    retry++;
                }
            }

            if (retry == 3 && failed)
            {
                var messageDialog = new MessageDialog("搜索结果有误，请稍后重新搜索");
                await messageDialog.ShowAsync();
            }
        }

        public void ResetReminder(ReminderStation alertStation)
        {
            lock (reminderLock)
            {
                this.CurrentReminderStation = alertStation;
            }
        }

        public void CancelReminder()
        {
            lock (reminderLock)
            {
                this.CurrentReminderStation = null;
            }
        }

        public async void CheckReminder()
        {
            lock (reminderLock)
            {
                if (this.CurrentReminderStation != null)
                {
                    bool needAlert = false;
                    StationInfo neareastInfo = null;
                    double distance = double.MaxValue;

                    foreach (StationInfo info in this.CurrentReminderStation.StationInfos)
                    {
                        double len = (info.longitude - this.CurrentSogoLongitude) * (info.longitude - this.CurrentSogoLongitude) +
                            (info.latitude - this.CurrentSogoLatitude) * (info.latitude - this.CurrentSogoLatitude);
                        if (info.isReminder && len < MinDistanceToAlert)
                        {
                            needAlert = true;
                            neareastInfo = info;
                            distance = len;
                            break;
                        }

                        if (len < distance)
                        {
                            distance = len;
                            neareastInfo = info;
                        }
                    }

                    if (neareastInfo != null)
                    {
                        this.CurrentReminderStation.status = "靠近站点" + neareastInfo.caption;
                    }

                    if (needAlert && !this.CurrentReminderStation.alertResponse)
                    {
                        if (this.notification != null && this.CurrentReminderStation.alertDisplaying)
                        {
                            return;
                        }
                        this.CurrentReminderStation.needAlert = true;
                        this.CurrentReminderStation.status = "即将到站！！！" + this.CurrentReminderStation.status;
                        IToastText02 toastContent = ToastContentFactory.CreateToastText02();

                        // Set the launch activation context parameter on the toast.
                        // The context can be recovered through the app Activation event
                        toastContent.Launch = "reminder";

                        toastContent.TextHeading.Text = "即将到站提醒";
                        toastContent.TextBodyWrap.Text = neareastInfo.caption + " 就要到了，请准备下车，以免坐过站";

                        this.notification = toastContent.CreateNotification();
                        this.notification.Dismissed += toast_Dismissed;

                        ToastNotificationManager.CreateToastNotifier().Show(this.notification);
                        this.CurrentReminderStation.alertDisplaying = true;
                        this.loadView(typeof(ReminderPage));
                    }
                }
            }
        }

        private async void testBtn_Click(object sender, RoutedEventArgs e)
        {
            IToastText02 toastContent = ToastContentFactory.CreateToastText02();

            // Set the launch activation context parameter on the toast.
            // The context can be recovered through the app Activation event
            toastContent.Launch = "reminder";

            toastContent.TextHeading.Text = "即将到站提醒";
            string desc = "";
            foreach (StationInfo info in this.CurrentReminderStation.StationInfos)
            {
                if (info.isReminder)
                {
                    desc = info.caption;
                }
            }
            toastContent.TextBodyWrap.Text = desc + "就要到站了，请准备下车，以免坐过站!";

            ToastNotification testNotification = toastContent.CreateNotification();
            ToastNotificationManager.CreateToastNotifier().Show(testNotification);
        }

        async void toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs e)
        {
            switch (e.Reason)
            {
                case ToastDismissalReason.UserCanceled:
                    this.CurrentReminderStation.alertResponse = true;
                    break;
                case ToastDismissalReason.ApplicationHidden:
                case ToastDismissalReason.TimedOut:
                    this.CurrentReminderStation.alertDisplaying = false;
                    break;
            }
        }

        public async void SetAlert(StationFeature selectedFeature)
        {
            string err = string.Empty;
            try
            {
                //set CurrentReminder
                if (this.CurrentStation != null)
                {
                    ReminderStation newAlert = new ReminderStation();
                    List<StationInfo> infoList = new List<StationInfo>();
                    foreach (StationFeature feature in this.CurrentStation.response.resultSet.busLine.stationData.StationFeatures)
                    {
                        if (feature.type.StartsWith("L"))
                        {
                            continue;
                        }
                        StationInfo info = new StationInfo();
                        info.stationId = feature.stationId;
                        info.caption = feature.caption;
                        string[] points = feature.points.txt.Split(',');
                        info.longitude = double.Parse(points[0]);
                        info.latitude = double.Parse(points[1]);
                        if (selectedFeature.stationId.Equals(feature.stationId))
                        {
                            info.isReminder = true;
                            info.displayText = info.caption + "                     (到站提醒)";
                            info.displayStyle = "ReminderTextStyle";
                        }
                        else
                        {
                            info.isReminder = false;
                            info.displayText = info.caption;
                            info.displayStyle = "BasicTextStyle";
                        }
                        infoList.Add(info);
                    }
                    newAlert.name = this.CurrentStation.response.resultSet.busLine.name;
                    newAlert.StationInfos = infoList.ToArray();
                    newAlert.alertResponse = false;
                    newAlert.alertDisplaying = false;

                    this.ResetReminder(newAlert);
                    loadView(typeof(ReminderPage));
                }
                else
                {
                    this.ShowStationChoose();
                    var messageDialog = new MessageDialog("请先选择线路！");
                    await messageDialog.ShowAsync();
                }
            }
            catch (Exception e)
            {
                err = e.Message;

            }
            if (err != null && err != string.Empty)
            {
                this.ShowStationChoose();
                var messageDialog = new MessageDialog("线路数据有误，请重新选择");
                await messageDialog.ShowAsync();
            }
        }

        public void ShowMap()
        {
            this.loadView(typeof(MapView));
        }

        public void ShowBusChoose()
        {
            if (this.CurrentBus != null)
            {
                this.loadView(typeof(BusLineChoose));
            }
            else
            {
                this.loadView(typeof(MapView));
            }
        }

        public void ShowStationChoose()
        {
            if (this.CurrentStation != null)
            {
                this.loadView(typeof(StationChoose));
            }
            else if (this.CurrentBus != null)
            {
                this.loadView(typeof(BusLineChoose));
            }
            else
            {
                this.loadView(typeof(MapView));
            }
        }

        public void ShowReminderPage()
        {
            if (this.CurrentReminderStation != null)
            {
                this.loadView(typeof(ReminderPage));
            }
            else
            {
                this.loadView(typeof(MapView));
            }
        }
    }
}
