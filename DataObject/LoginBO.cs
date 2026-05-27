using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class LoginBO
    {
        public string userId { get; set; }
        public string role_id { get; set; }
        public int roleId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string oldpassword { get; set; }
        public string result { get; set; }
        public string message { get; set; }
        public int UserId { get; set; }
        public int Insertedby { get; set; }
        public int? Updatedby { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string roleDescription { get; set; }
        public int isPasswordReset { get; set; }
    }
}
