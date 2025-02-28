using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiDesktopApp2.Interfaces;

namespace UiDesktopApp2.Services 
{
    class DateTimeClass : IDateTime
    {
        public DateTime? GetCurrentTime()
        {
            return DateTime.Now;
        }
    }
}
