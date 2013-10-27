using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Purdue_Dining_Courts;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI;

namespace Purdue_Dining_Courts
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private List<PurdueMenu> menuList;
        private List<StackPanel> panelList;
        private List<TextBlock> subTitleList;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // todo - allow user to favorite items
            // todo - push notifications for favorites
            // todo - live tile of latest menu
            try
            {
                PurdueMenu earhartMenu = new PurdueMenu("Earhart");
                PurdueMenu windsorMenu = new PurdueMenu("Windsor");
                PurdueMenu hillenbrandMenu = new PurdueMenu("Hillenbrand");
                PurdueMenu wileyMenu = new PurdueMenu("Wiley");
                PurdueMenu fordMenu = new PurdueMenu("Ford");

                menuList = new List<PurdueMenu>();
                menuList.Add(earhartMenu);
                menuList.Add(windsorMenu);
                menuList.Add(hillenbrandMenu);
                menuList.Add(wileyMenu);
                menuList.Add(fordMenu);

                panelList = new List<StackPanel>();
                panelList.Add(EarhartPanel);
                panelList.Add(WindsorPanel);
                panelList.Add(HillenbrandPanel);
                panelList.Add(WileyPanel);
                panelList.Add(FordPanel);

                subTitleList = new List<TextBlock>();
                subTitleList.Add(earhartSubTitle);
                subTitleList.Add(windsorSubtitle);
                subTitleList.Add(hillenbrandSubtitle);
                subTitleList.Add(wileySubtitle);
                subTitleList.Add(fordSubtitle);

                menuComboBox.SelectedIndex = 0;

                UpdateMenus();
            }
            catch (Exception exception)
            {
                ShowNetworkFailurePopup();
            }
        }

        private void UpdateMenus(string desiredMenu = "Best")
        {
            Dictionary<string, List<MenuItem>> dict;
            int i = 0;
            foreach (PurdueMenu menu in menuList)
            {
                if (desiredMenu == "Breakfast")
                {
                    dict = menu.BreakfastMenu();
                }
                else if (desiredMenu == "Lunch")
                {
                    dict = menu.LunchMenu();
                }
                else if (desiredMenu == "Dinner")
                {
                    dict = menu.DinnerMenu();
                }
                else
                {
                    dict = menu.PickBestMenu();
                }

                try
                {
                    panelList[i].Children.Clear();
                    foreach (var station in dict)
                    {
                        ListView stationListView = new ListView();
                        stationListView.SelectionMode = ListViewSelectionMode.None;
                        TextBlock tb = new TextBlock();
                        tb.FontSize = 18.0;
                        tb.Text = station.Key;
                        panelList[i].Children.Add(tb);

                        foreach (MenuItem item in station.Value)
                        {
                            stationListView.Items.Add(item.Name);
                        }

                        panelList[i].Children.Add(stationListView);
                    }
                    subTitleList[i].Text = string.Format("{0} - {1}", menuList[i].HoursOfOperation[menuList[i].ChosenMenu].startTime.ToString(), menuList[i].HoursOfOperation[menuList[i].ChosenMenu].endTime.ToString());
                    panelList[i].UpdateLayout();
                }
                catch (NullReferenceException exception)
                {
                    panelList[i].Children.Clear();
                    // handle when there's no food being served...
                    TextBlock noFoodTextBlock = new TextBlock();
                    noFoodTextBlock.Foreground = new SolidColorBrush(Colors.Black);
                    noFoodTextBlock.Text = "Food is currently not being served at this location.";
                    noFoodTextBlock.Margin = new Thickness(15, 0, 0, 0);
                    panelList[i].Children.Add(noFoodTextBlock);
                }

                i++;
            }
        }

        private async void ShowNetworkFailurePopup()
        {
            MessageDialog msg = new MessageDialog("Could not connect to Purdue's server. Please make sure you have a strong internet connection.", "Failure to Connect");
            await msg.ShowAsync();
        }

        private void autoComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateMenus();
        }

        private void breakfastComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateMenus("Breakfast");
        }

        private void lunchComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateMenus("Lunch");
        }

        private void dinnerComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateMenus("Dinner");
        }
    }
}
