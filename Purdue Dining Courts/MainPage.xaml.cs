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
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.ApplicationSettings;

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
        private string[] menus = { "Best", "Breakfast", "Lunch", "Dinner" };
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += ShowPrivacyPolicy;

            // todo - allow user to favorite items
            // todo - push notifications for favorites
            // todo - live tile of latest menu

            PurdueMenu[] pmArray = (PurdueMenu[])e.Parameter;
            PurdueMenu earhartMenu = pmArray[0];
            PurdueMenu windsorMenu = pmArray[1];
            PurdueMenu hillenbrandMenu = pmArray[2];
            PurdueMenu wileyMenu = pmArray[3];
            PurdueMenu fordMenu = pmArray[4];

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
            dateComboBox.SelectedIndex = 0;
        }

        // PRIVACY POLICY
        // Method to add the privacy policy to the settings charm
        private void ShowPrivacyPolicy(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand privacyPolicyCommand = new SettingsCommand("privacyPolicy", "Privacy Policy", (uiCommand) => { LaunchPrivacyPolicyUrl(); });
            args.Request.ApplicationCommands.Add(privacyPolicyCommand);
        }

        // Method to launch the url of the privacy policy
        async void LaunchPrivacyPolicyUrl()
        {
            Uri privacyPolicyUrl = new Uri("http://grantgumina.com/purduediningcourts/privacypolicy.html");
            var result = await Windows.System.Launcher.LaunchUriAsync(privacyPolicyUrl);
        }

        private void UpdateMenus(string desiredMenu = "Best")
        {
            int i = 0;
            Dictionary<string, List<MenuItem>> dict;
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
                        stationListView.IsEnabled = false;

                        // apply custom style to listview
                        Style st = new Style();
                        st.TargetType = typeof(ListView);
                        Setter backGroundSetter = new Setter();
                        backGroundSetter.Property = ListViewItem.BackgroundProperty;
                        backGroundSetter.Value = new SolidColorBrush(Color.FromArgb(100, 126, 208, 224));
                        st.Setters.Add(backGroundSetter);

                        Setter foregroundSetter = new Setter();
                        foregroundSetter.Property = ListViewItem.ForegroundProperty;
                        foregroundSetter.Value = new SolidColorBrush(Colors.Black);
                        st.Setters.Add(foregroundSetter);
                        stationListView.Style = st;

                        TextBlock tb = new TextBlock();
                        tb.FontSize = 18.0;
                        tb.Foreground = new SolidColorBrush(Colors.DarkRed);
                        tb.Margin = new Thickness(0, 10, 0, 0);
                        tb.Text = station.Key;
                        panelList[i].Children.Add(tb);

                        foreach (MenuItem item in station.Value)
                        {
                            stationListView.Items.Add(item.Name);
                        }

                        panelList[i].Children.Add(stationListView);
                    }
                    if (menuList[i].HoursOfOperation[menuList[i].ChosenMenu].startTime.Hour - menuList[i].HoursOfOperation[menuList[i].ChosenMenu].endTime.Hour == 0)
                    //if (menuList[i])
                    {
                        NoFoodServed(i);
                    }
                    else
                    {
                        subTitleList[i].Text = string.Format("{0} - {1}", menuList[i].HoursOfOperation[menuList[i].ChosenMenu].startTime.ToString("hh:mm"), menuList[i].HoursOfOperation[menuList[i].ChosenMenu].endTime.ToString("hh:mm"));
                    }
                    panelList[i].UpdateLayout();
                }
                catch (NullReferenceException exception)
                {
                    NoFoodServed(i);
                }
                i++;
            }

        }

        private void NoFoodServed(int i)
        {
            panelList[i].Children.Clear();
            // handle when there's no food being served...
            TextBlock noFoodTextBlock = new TextBlock();
            noFoodTextBlock.Foreground = new SolidColorBrush(Colors.Black);
            noFoodTextBlock.FontSize = 14;
            noFoodTextBlock.TextWrapping = TextWrapping.Wrap;
            noFoodTextBlock.TextAlignment = TextAlignment.Center;
            noFoodTextBlock.Text = "Food is currently not being served at this location.";
            noFoodTextBlock.Margin = new Thickness(15, 0, 0, 0);
            panelList[i].Children.Add(noFoodTextBlock);
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

        private int DaysToAdd(DayOfWeek current, DayOfWeek desired)
        {
            int c = (int)current;
            int d = (int)desired;
            int n = (7 - c + d);
            return (n > 7) ? n % 7 : n;
        }

        private async void autoDateComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                await Task.Run(() => menu.ChangeDate(DateTime.Now));
            }
            UpdateMenus(menus[menuComboBox.SelectedIndex]);
        }

        private async void sundayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Sunday));
                await Task.Run(() => menu.ChangeDate(newDate));
            }
            UpdateMenus(menus[menuComboBox.SelectedIndex]);
        }

        private async void mondayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Monday));
                await Task.Run(() => menu.ChangeDate(newDate));
            }
            UpdateMenus(menus[menuComboBox.SelectedIndex]);
        }

        private async void tuesdayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Tuesday));
                await Task.Run(() => menu.ChangeDate(newDate));
            }
            UpdateMenus(menus[menuComboBox.SelectedIndex]);
        }

        private async void wednesdayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Wednesday));
                await Task.Run(() => menu.ChangeDate(newDate));
            }
            UpdateMenus(menus[menuComboBox.SelectedIndex]);
        }

        private async void thursdayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Thursday));
                await Task.Run(() => menu.ChangeDate(newDate));
            }
            UpdateMenus(menus[menuComboBox.SelectedIndex]);
        }

        private async void fridayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Friday));
                await Task.Run(() => menu.ChangeDate(newDate));
            }
            UpdateMenus(menus[menuComboBox.SelectedIndex]);
        }

        private async void saturdayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Saturday));
                await Task.Run(() => menu.ChangeDate(newDate));
            }
            UpdateMenus(menus[menuComboBox.SelectedIndex]);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            UpdateMenus();
        }
    }
}
