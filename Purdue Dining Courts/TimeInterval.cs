using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purdue_Dining_Courts
{
    /*
     * A stupid simple class that represents a window of time in a day. Uses military time.
     */
    class TimeInterval
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        
        public TimeInterval(int startHour, int startMinute, int endHour, int endMinute)
        {
            startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHour, startMinute, 0);
            endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endHour, endMinute, 0);
        }

        public bool IsWithinRange(DateTime currentTime)
        {
            // Reset year, month, day incase the user has the app opened for multiple days
            startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startTime.Hour, startTime.Minute, 0);
            endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endTime.Hour, endTime.Minute, 0);

            if (currentTime.Ticks > startTime.Ticks && currentTime.Ticks < endTime.Ticks)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
