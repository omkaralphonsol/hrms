using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class userDocumentsDO
    {
        public int UserDocDetId { get; set; }
        public int UserId { get; set; }
        public int DocumentMasterId { get; set; }
        public string filepath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime InsertedDate { get; set; }

        public string ReferenceNumber { get; set; }

        public string EmailId { get; set; }

    }

    public class FileAttachment
    {
        public int FileId { get; set; }  // Ensure this exists

        public string FileName { get; set; }
        public string FilePath { get; set; }

        public int DocumentMasterId { get; set; }

        public string ReferenceNumber { get; set; }

        public string EmailId { get; set; }
        //public List<FileAttachment> Attachments { get; set; } = new List<FileAttachment>();

    }
    public class userResponseDataDO
    {
        public bool Success { get; set; }
        public string ResponseMsg { get; set; }
        public string Error { get; set; }
        public List<usersDataDO> UsersList { get; set; }
    }

    public class usersDataDO
    {
        public string EmployeeCode { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string User_fullname { get; set; }
        public string user_mail_id { get; set; }
        public string contact_detail { get; set; }
        public string roledescription { get; set; }


        // add other properties
    }
    public class SearchUserRequest
    {
        public int? UserId { get; set; }
        public int? EmpCodeId { get; set; }
        public string ContactDetail { get; set; }
    }

}
