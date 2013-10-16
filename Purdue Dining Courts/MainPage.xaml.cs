﻿using System;
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

namespace Purdue_Dining_Courts
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // todo get the hours of operation for each dining court

            //DateTime testTime = new DateTime(2013, 10, 10);
            PurdueMenu earhartMenu = new PurdueMenu("Earhart");
            PurdueMenu windsorMenu = new PurdueMenu("Windsor");
            PurdueMenu hillenbrandMenu = new PurdueMenu("Hillenbrand");
            PurdueMenu wileyMenu = new PurdueMenu("Wiley");
            PurdueMenu fordMenu = new PurdueMenu("Ford");

            Dictionary<string, List<MenuItem>> dict = earhartMenu.BreakfastMenu();
            foreach (var station in dict)
            {
                ListView stationListView = new ListView();
                TextBlock tb = new TextBlock();
                tb.FontSize = 24.0;
                tb.Text = station.Key;
                EarhartPanel.Children.Add(tb);

                foreach (MenuItem item in station.Value)
                {
                    stationListView.Items.Add(item.Name);
                }

                EarhartPanel.Children.Add(stationListView);
            }
            EarhartPanel.UpdateLayout();
        }
    }
}
