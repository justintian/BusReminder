using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.ApplicationSettings;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BusReminder
{
    public sealed partial class PrivacyPolicy : UserControl
    {
        public PrivacyPolicy()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This is the click handler for the back button on the Flyout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MySettingsBackClicked(object sender, RoutedEventArgs e)
        {
            // First close our Flyout.
            Popup parent = this.Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }
    }
}

