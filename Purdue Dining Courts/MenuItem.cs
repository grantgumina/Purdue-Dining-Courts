using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Purdue_Dining_Courts
{
    class MenuItem
    {
        public string Name { get; set; }
        public bool IsVegetarian { get; set; }

        private static string GetElementValue(XContainer element, string name)
        {
            if ((element == null) || (element.Element(name) == null))
            {
                return String.Empty;
            }
            return element.Element(name).Value;
        }

        public MenuItem(XContainer item)
        {
            Name = GetElementValue(item, "Name");
            IsVegetarian = bool.Parse(GetElementValue(item, "IsVegetarian"));
        }
    }
}
