using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Purdue_Dining_Courts
{
    class PurdueMenu
    {
        private string DiningCourt;
        private string MM;
        private string DD;
        private string YYYY;
        private XDocument xDoc;

        public PurdueMenu(string dc, DateTime date = new DateTime())
        {
            if (date == new DateTime())
            {
                date = DateTime.Now;
            }

            DiningCourt = dc;
            MM = date.Month.ToString();
            DD = date.Day.ToString();
            YYYY = date.Year.ToString();
            DownloadXML();
        }

        public Dictionary<string, List<MenuItem>> BreakfastMenu()
        {
            return ParseMenu("Breakfast");
        }

        public Dictionary<string, List<MenuItem>> LunchMenu()
        {
            return ParseMenu("Lunch");
        }

        public Dictionary<string, List<MenuItem>> DinnerMenu()
        {
            return ParseMenu("Dinner");
        }

        private Dictionary<string, List<MenuItem>> ParseMenu(string menuDesired)
        {
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

        private void DownloadXML()
        {
            string url = string.Format("http://api.hfs.purdue.edu/menus/v1/locations/{0}/{1}-{2}-{3}", DiningCourt, MM, DD, YYYY);
            xDoc = XDocument.Load(url);
        }
    }
}