using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class roleDO
    {
        public int Insertedby { get; set; }
        public int? Updatedby { get; set; }
        public int Roleid { get; set; }

        public string Roledescription { get; set; }

        public string searchbyType { get; set; }
        public string searchValue { get; set; }
        public bool Isactive { get; set; }

        public DateTime Inserteddate { get; set; }



        public DateTime Updateddate { get; set; }


        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
