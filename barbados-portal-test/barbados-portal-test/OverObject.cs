using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace barbados_portal_test
{
    public class OverObject:IDisposable
    {

        public string type { get; set; }

        public string datetime { get; set; }

        public string time { get; set; }

        public string Hex { get; set; }

        public string plate { get; set; }

        public int conf { get; set; }

        public int lane { get; set; }

        public string idCamera { get; set; }

        public string lattitude { get; set; }

        public string longitude { get; set; }
        public void Dispose()
        {

        }

    }
}
