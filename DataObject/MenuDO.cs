using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class MenuDO
    {
        public int Insertedby { get; set; }
        public int? Updatedby { get; set; }
        public string type { get; set; }
        public string userId { get; set; }
        public string menuid { get; set; }
        public string submenuid { get; set; }
        public string menu { get; set; }
        public string menulink { get; set; }
        public string SubMenu { get; set; }
        public bool isactive { get; set; }
        public string username { get; set; }
    }
    public class MenuData
    {
        public string MenuId { get; set; }
        public string Menu { get; set; }
        public string MenuLink { get; set; }
        public string Icon { get; set; }
        public List<SubMenuData> SubMenus { get; set; }






    }
    public class SubMenuData
    {
        //public string MenuId { get; set; }
        public string SubMenuId { get; set; }
        public string SubMenu { get; set; }
        public string SubMenuLink { get; set; }

    }
    public class CompanyLogoDO
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }  
        public string LogoPath { get; set; }
    }


}
