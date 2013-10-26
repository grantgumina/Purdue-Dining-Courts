using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using System.Diagnostics;

namespace Purdue_Dining_Courts
{
    class PurdueMenu
    {
        public Dictionary<string, TimeInterval> HoursOfOperation { get; set; }
        public string ChosenMenu;
        public DateTime DesiredDate;
        private string diningCourt;
        private XDocument xDoc;

        public PurdueMenu(string dc, DateTime date = new DateTime())
        {
            DesiredDate = date;
            if (DesiredDate == new DateTime())
            {
                DesiredDate = DateTime.Now;
            }

            SetHoursOfOperation(dc, DesiredDate);

            diningCourt = dc;
            DownloadXML();
        }
    
        // based on the current date/time, this function returns the best menu for the user to be presented with automatically
        public Dictionary<string, List<MenuItem>> PickBestMenu()
        {
            // should I reset the date object and redownload the XML?

            // if user passes in a specific date, then it would never make sense to use this option... but I still will need to handle that scenario
            // if DateTime.Now is within a given time frame... display the best menu
            foreach (KeyValuePair<string, TimeInterval> slot in HoursOfOperation)
            { 
                // if service is occuring right now
                if (slot.Value.IsWithinRange(DateTime.Now))
                {
                    Debug.WriteLine("{0}: {1} menu - within range", diningCourt, slot.Key);
                    ChosenMenu = slot.Key;
                    return ParseMenu(slot.Key);
                }
            }

            return null;
        }

        public Dictionary<string, List<MenuItem>> BreakfastMenu()
        {
            ChosenMenu = "Breakfast";
            return ParseMenu("Breakfast");
        }

        public Dictionary<string, List<MenuItem>> LunchMenu()
        {
            ChosenMenu = "Lunch";
            return ParseMenu("Lunch");
        }

        public Dictionary<string, List<MenuItem>> DinnerMenu()
        {
            ChosenMenu = "Dinner";
            return ParseMenu("Dinner");
        }

        private Dictionary<string, List<MenuItem>> ParseMenu(string menuDesired)
        {
            // Set hours of operation for desired menu
            IEnumerable<XElement> stations = xDoc.Descendants(menuDesired).Elements();
            Dictionary<string, List<MenuItem>> genericMenu = new Dictionary<string, List<MenuItem>>();

            foreach (XElement station in stations)
            {
                string stationName = station.Element("Name").Value;
                List<MenuItem> stationItems = (from stationItem in station.Descendants("MenuItem")
                                               select new MenuItem(stationItem)).ToList<MenuItem>();
                genericMenu.Add(stationName, stationItems);
            }
            return genericMenu;
        }

        private void SetHoursOfOperation(string diningCourt, DateTime DesiredDate)
        {
            // Breakfast hour
            TimeInterval earheartBreakfastHours, fordBreakfastHours, hillenbrandBreakfastHours, wileyBreakfastHours, windsorBreakfastHours;

            // Lunch hours
            TimeInterval earheartLunchHours, fordLunchHours, hillenbrandLunchHours, wileyLunchHours, windsorLunchHours;

            // Dinner hours
            TimeInterval earheartDinnerHours, fordDinnerHours, hillenbrandDinnerHours, wileyDinnerHours, windsorDinnerHours;

            if (DesiredDate.DayOfWeek != DayOfWeek.Saturday && DesiredDate.DayOfWeek != DayOfWeek.Sunday)
            {
                // m-f breakfast, Earhart & Ford & Wiley
                earheartBreakfastHours = new TimeInterval(6, 30, 9, 30);
                fordBreakfastHours = new TimeInterval(6, 30, 9, 30);
                wileyBreakfastHours = new TimeInterval(6, 30, 9, 30);
                // m-f breakfast, Hillenbrand (none)
                hillenbrandBreakfastHours = new TimeInterval(0, 0,  0, 0);
                // m-f breakfast, Windsor
                windsorBreakfastHours = new TimeInterval(9, 30, 17, 00);

                // m-f lunch, Earhart & Ford & Wiley
                earheartLunchHours = new TimeInterval(11, 00, 14, 00);
                fordLunchHours = new TimeInterval(11, 00, 14, 00);
                wileyLunchHours = new TimeInterval(11, 00, 14, 00);
                // m-f lunch, Hillenbrand
                hillenbrandLunchHours = new TimeInterval(10, 00, 14, 00);
                // m-f lunch, Windsor
                windsorLunchHours = new TimeInterval(9, 30, 17, 00);

                // m-f dinner, Earhart & Ford
                earheartDinnerHours = new TimeInterval(17, 00, 20, 30);
                fordDinnerHours = new TimeInterval(17, 00, 20, 30);
                // m-f dinner, Hillenbrand & Windsor
                hillenbrandDinnerHours = new TimeInterval(17, 30, 19, 30);
                windsorDinnerHours = new TimeInterval(17, 30, 19, 30);
                // m-f dinner, Wiley
                wileyDinnerHours = new TimeInterval(17, 00, 20, 00);
            }
            else if (DesiredDate.DayOfWeek == DayOfWeek.Saturday)
            {
                // sat breakfast, Earhart & Ford
                earheartBreakfastHours = new TimeInterval(0, 0,  0, 0);
                fordBreakfastHours = new TimeInterval(0, 0,  0, 0);
                // sat breakfast, Hillenbrand (none)
                hillenbrandBreakfastHours = new TimeInterval(0, 0,  0, 0);
                // sat breakfast, Wiley & Windsor
                wileyBreakfastHours = new TimeInterval(8, 0, 9, 30);
                windsorBreakfastHours = new TimeInterval(0, 0,  0, 0);

                // sat lunch, Earhart & Ford & Wiley
                earheartLunchHours = new TimeInterval(11, 00, 14, 00);
                fordLunchHours = new TimeInterval(11, 00, 14, 00);
                wileyLunchHours = new TimeInterval(11, 00, 14, 00);
                // sat lunch, Hillenbrand
                hillenbrandLunchHours = new TimeInterval(0, 0, 0, 0);
                // sat lunch, Windsor
                windsorLunchHours = new TimeInterval(0, 0,  0, 0);

                // sat dinner, Earhart & Ford
                earheartDinnerHours = new TimeInterval(0, 0,  0, 0);
                fordDinnerHours = new TimeInterval(17, 00, 20, 30);
                // sat dinner, Hillenbrand & Windsor
                hillenbrandDinnerHours = new TimeInterval(0, 0,  0, 0);
                windsorDinnerHours = new TimeInterval(0, 0,  0, 0);
                // sat dinner, Wiley
                wileyDinnerHours = new TimeInterval(17, 00,  20, 00);
            }
            else
            {
                // sun breakfast, Earhart & Ford
                earheartBreakfastHours = new TimeInterval(0, 0,  0, 0);
                fordBreakfastHours = new TimeInterval(0, 0,  0, 0);
                // sun breakfast, Hillenbrand (none)
                hillenbrandBreakfastHours = new TimeInterval(0, 0,  0, 0);
                // sun breakfast, Wiley & Windsor
                wileyBreakfastHours = new TimeInterval(8, 0, 9, 30);
                windsorBreakfastHours = new TimeInterval(0, 0,  0, 0);

                // sun lunch, Earhart & Ford & Wiley
                earheartLunchHours = new TimeInterval(11, 00, 14, 00);
                fordLunchHours = new TimeInterval(11, 00, 14, 00);
                wileyLunchHours = new TimeInterval(11, 00, 14, 00);
                // sun lunch, Hillenbrand
                hillenbrandLunchHours = new TimeInterval(11, 00, 14, 00);
                // sun lunch, Windsor
                windsorLunchHours = new TimeInterval(0, 0,  0, 0);

                // sun dinner, Earhart & Ford
                earheartDinnerHours = new TimeInterval(17, 00, 20, 30);
                fordDinnerHours = new TimeInterval(0, 0,  0, 0);
                // sun dinner, Hillenbrand & Windsor
                hillenbrandDinnerHours = new TimeInterval(0, 0,  0, 0);
                windsorDinnerHours = new TimeInterval(0, 0,  0, 0);
                // sun dinner, Wiley
                wileyDinnerHours = new TimeInterval(17, 00, 20, 00);
            }
            switch (diningCourt)
            {
                case "Earhart":
                    HoursOfOperation = new Dictionary<string, TimeInterval>();
                    HoursOfOperation["Breakfast"] = earheartBreakfastHours;
                    HoursOfOperation["Lunch"] = earheartLunchHours;
                    HoursOfOperation["Dinner"] = earheartDinnerHours;
                    break;
                case "Ford":
                    HoursOfOperation = new Dictionary<string, TimeInterval>();
                    HoursOfOperation["Breakfast"] = fordBreakfastHours;
                    HoursOfOperation["Lunch"] = fordLunchHours;
                    HoursOfOperation["Dinner"] = fordDinnerHours;
                    break;
                case "Hillenbrand":
                    HoursOfOperation = new Dictionary<string, TimeInterval>();
                    HoursOfOperation["Breakfast"] = hillenbrandBreakfastHours;
                    HoursOfOperation["Lunch"] = hillenbrandLunchHours;
                    HoursOfOperation["Dinner"] = hillenbrandDinnerHours;
                    break;
                case "Wiley":
                    HoursOfOperation = new Dictionary<string, TimeInterval>();
                    HoursOfOperation["Breakfast"] = wileyBreakfastHours;
                    HoursOfOperation["Lunch"] = wileyLunchHours;
                    HoursOfOperation["Dinner"] = wileyDinnerHours;
                    break;
                case "Windsor":
                    HoursOfOperation = new Dictionary<string, TimeInterval>();
                    HoursOfOperation["Breakfast"] = windsorBreakfastHours;
                    HoursOfOperation["Lunch"] = windsorLunchHours;
                    HoursOfOperation["Dinner"] = windsorDinnerHours;
                    break;
            }
        }

        private void DownloadXML()
        {
            string MM = DesiredDate.Month.ToString();
            string DD = DesiredDate.Day.ToString();
            string YYYY = DesiredDate.Year.ToString();
            string url = string.Format("http://api.hfs.purdue.edu/menus/v1/locations/{0}/{1}-{2}-{3}", diningCourt, MM, DD, YYYY);
            xDoc = XDocument.Load(url);
        }
    }
}