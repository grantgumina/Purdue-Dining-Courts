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
        private string[] menus = { "Best", "Breakfast", "Lunch", "Dinner" };
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
                dateComboBox.SelectedIndex = 0;

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
                        stationListView.IsEnabled = false;

                        // apply custom style to listview
                        Style st = new Style();
                        st.TargetType = typeof(ListView);
                        Setter backGroundSetter = new Setter();
                        backGroundSetter.Property = ListViewItem.BackgroundProperty;
                        backGroundSetter.Value = new SolidColorBrush(Colors.LightBlue);
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
                    subTitleList[i].Text = string.Format("{0} - {1}", menuList[i].HoursOfOperation[menuList[i].ChosenMenu].startTime.ToString("hh:mm"), menuList[i].HoursOfOperation[menuList[i].ChosenMenu].endTime.ToString("hh:mm"));
                    panelList[i].UpdateLayout();
                }
                catch (NullReferenceException exception)
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

        private int DaysToAdd(DayOfWeek current, DayOfWeek desired)
        {
            int c = (int)current;
            int d = (int)desired;
            int n = (7 - c + d);
            return (n > 7) ? n % 7 : n;
        }

        private void autoDateComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                menu.ChangeDate(DateTime.Now);
                UpdateMenus(menus[menuComboBox.SelectedIndex]);
            }
        }

        private void sundayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Sunday));
                menu.ChangeDate(newDate);
                UpdateMenus(menus[menuComboBox.SelectedIndex]);
            }
        }

        private void mondayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Monday));
                menu.ChangeDate(newDate);
                UpdateMenus(menus[menuComboBox.SelectedIndex]);
            }
        }

        private void tuesdayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Tuesday));
                menu.ChangeDate(newDate);
                UpdateMenus(menus[menuComboBox.SelectedIndex]);
            }
        }

        private void wednesdayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Wednesday));
                menu.ChangeDate(newDate);
                UpdateMenus(menus[menuComboBox.SelectedIndex]);
            }
        }

        private void thursdayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Thursday));
                menu.ChangeDate(newDate);
                UpdateMenus(menus[menuComboBox.SelectedIndex]);
            }
        }

        private void fridayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Friday));
                menu.ChangeDate(newDate);
                UpdateMenus(menus[menuComboBox.SelectedIndex]);
            }
        }

        private void saturdayComboBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (PurdueMenu menu in menuList)
            {
                DateTime newDate = DateTime.Now;
                newDate = newDate.AddDays(DaysToAdd(newDate.DayOfWeek, DayOfWeek.Saturday));
                menu.ChangeDate(newDate);
                UpdateMenus(menus[menuComboBox.SelectedIndex]);
            }
        }
    }
}
