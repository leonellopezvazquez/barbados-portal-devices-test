using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace barbados_portal_test
{
    class PIPSEvent
    {
      
        public long id { get; set; }
        public string idDevice { get; set; }
        public string LPN { get; set; }
        public int lane { get; set; }
        public string TollingId { get; set; }
        public string timestamp { get; set; }
        public string overview { get; set; }
        public string patch { get; set; }
        public string overview_path { get; set; }
        public string patch_path { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public bool misread { get; set; }
        public bool synced { get; set; }
        public bool hit { get; set; }
        public int conf { get; set; }
        public string IdCamera { get; set; }

        public byte[] Patch { get; set; }

        public byte[] IR { get; set; }

        public byte[] Overview { get; set; }
    }


    public class Mantis
    {
        public string status { get; set; }
        public int car { get; set; }
        public int day { get; set; }
        public int night { get; set; }
        public string brand { get; set; }
        public int brand_percent { get; set; }
        public string type { get; set; }
        public int type_percent { get; set; }
        public string color { get; set; }
        public int color_percent { get; set; }
        public string others { get; set; }

        
    }

}
