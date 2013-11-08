using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Diagnostics;

namespace Purdue_Dining_Courts
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            this.InitializeComponent();
            downloadingRing.IsActive = true;
        }

        private PurdueMenu[] menuArray = new PurdueMenu[5];
        private async void ShowNetworkFailurePopup()
        {
            MessageDialog msg = new MessageDialog("Could not connect to Purdue's server. Please make sure you have a strong internet connection.", "Failure to Connect");
            await msg.ShowAsync();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => doshit());
            Frame.Navigate(typeof(MainPage), menuArray);
        }

        private void doshit()
        {
            try
            {
                menuArray[0] = new PurdueMenu("Earhart");
                menuArray[1] = new PurdueMenu("Windsor");
                menuArray[2] = new PurdueMenu("Hillenbrand");
                menuArray[3] = new PurdueMenu("Wiley");
                menuArray[4] = new PurdueMenu("Ford");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("====> " + ex.Message);
                ShowNetworkFailurePopup();
            }      
        }
    }
}
