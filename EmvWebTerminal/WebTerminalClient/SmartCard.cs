using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTerminalClient
{
    class SmartCard
    {
        public event EventHandler OnCardIn = null;

        public bool WaitCardIn()
        {
            if (OnCardIn != null)
                OnCardIn(this, new EventArgs());
            return true;
        }
    }
}
