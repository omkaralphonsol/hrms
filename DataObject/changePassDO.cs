using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class changePassDO
    {
        public string userId { get; set; }
        public string user_mail_id { get; set; }
        public string result { get; set; }
        public int UserId { get; set; }
        public string VerifyOTP { get; set; }
        public string user_fullname { get; set; }
        public string generated_otp { get; set; }

        public int? passresetflag { get; set; }
    }
}
