using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class LoginDO
    {
        public int userId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string result { get; set; }
        public string message { get; set; }
        public string remark { get; set; }
        public string passresetflag { get; set; }

        public string UserRole { get; set; }
    }
}
