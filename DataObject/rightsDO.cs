using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class rightsDO
    {
        public int? roleId { get; set; }
        public int? menuId { get; set; }
        public int? submenuId { get; set; }
        public int Insertedby { get; set; }
        public int? Updatedby { get; set; }
        public int? userrightsid { get; set; }
        public int? roleid { get; set; }
        public string roledescription { get; set; }
        public int? menuid { get; set; }
        public int? submenuid { get; set; }
        public string menu { get; set; }
        public string submenu { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string searchbyType { get; set; }

        public string searchValue { get; set; }
        public int? subsubmenuid { get; set; }
        public string subsubmenu { get; set; }
        public string userrightid { get; set; }
    }
}
