using System;

namespace HRMS.View.Modules
{
    public partial class Adduser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string targetUrl = "AddUserMain.aspx";
            string query = Request.Url.Query;
            if (!string.IsNullOrWhiteSpace(query))
            {
                targetUrl += query;
            }

            Response.Redirect(targetUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
