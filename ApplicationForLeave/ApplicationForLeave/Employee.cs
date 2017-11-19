using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationForLeave
{
    class Employee
    {
        public int ID { get; set; }
        public string Fio { get; set; }
        public Chief Chief { get; set; }
    }
}
