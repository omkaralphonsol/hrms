using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Authentication
{
    public partial class logout : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                int userId = Convert.ToInt32(Session["userid"]);

                Session.Clear();
                Session.Abandon();
                Response.Redirect("/View/Authentication/Login.aspx", false);
            }
        }
    }
}