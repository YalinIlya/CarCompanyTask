using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetValue.Code
{
    public class CalculateValue
    {
        public int Calc(int currentValue)
        {
            int hour = DateTime.Now.Hour;
            if (hour > 12) currentValue += 5;
            else if (hour < 12) currentValue -= 5;
            return currentValue;
        }
    }
}