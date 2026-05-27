using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataObject;
using ProcessModel;

namespace Lean.View.Layout
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected string UserId = null;

        private static bool IsStandaloneDocumentMenu(string menuName, string menuLink)
        {
            string safeName = (menuName ?? string.Empty).Trim();
            string safeLink = (menuLink ?? string.Empty).Trim();
            bool isDocumentByName = safeName.Equals("Document", StringComparison.OrdinalIgnoreCase)
                                    || safeName.Equals("Documents", StringComparison.OrdinalIgnoreCase);
            bool isDocumentByLink = safeLink.IndexOf("/View/Modules/useruploaddocuments.aspx", StringComparison.OrdinalIgnoreCase) >= 0
                                    || safeLink.IndexOf("/View/Modules/viewuserdocuments.aspx", StringComparison.OrdinalIgnoreCase) >= 0;
            return isDocumentByName || isDocumentByLink;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["userid"])))
            {
                int userId = Convert.ToInt32(Session["userid"]);
                if (!IsPostBack)
                {
                    int sessionUserId = Convert.ToInt32(Session["userId"]);
                    string url = HttpContext.Current.Request.Url.ToString();
                    string[] urlSegments = new Uri(url).Segments;
                    string lastSegment = urlSegments[urlSegments.Length - 1].Trim('/');
                    if (!string.IsNullOrWhiteSpace(lastSegment) && !lastSegment.Contains("."))
                    {
                        lastSegment = lastSegment + ".aspx";
                    }
                    UserDetailsBL sessionUserBAL = new UserDetailsBL();
                    int accessStatus = sessionUserBAL.Getpage(sessionUserId, lastSegment);
                    bool skipAccessCheck =
                        string.Equals(lastSegment, "LeaveConfiguration.aspx", StringComparison.OrdinalIgnoreCase);

                    //bool isAddProjectViewMode = url.Contains("Addproject") && url.Contains("mode=view");
                    int roleId = Convert.ToInt32(Session["roleid"]);

                    //var queryParams = HttpUtility.ParseQueryString(new Uri(url).Query);
                    //bool isAddProjectViewMode = url.Contains("Addproject") && queryParams["mode"] == "view";
                    //string projectId = queryParams["projectId"]; // dynamically retrieves the projectId from URL
                    //bool isAlreadyOnTargetUrl = isAddProjectViewMode && queryParams["projectId"] == projectId;

                    //// If in Addproject view mode and projectId is found, redirect using the current projectId
                    //if (isAddProjectViewMode && !string.IsNullOrEmpty(projectId))
                    //{
                    //    // Redirect to Addproject page with the dynamically retrieved projectId
                    //    Response.Redirect($"/View/Modules/Addproject.aspx?projectId={projectId}&mode=view", false);
                    //    return;
                    //}
                    if (!skipAccessCheck && accessStatus != 1)
                    {

                        string status = "Reject";
                        string remark = "You do not have the right to access this page.";

                        if (roleId == 2)
                        {
                            // Redirect to LoginLogout.aspx with status and remark as query string parameters
                            Response.Redirect("/View/Modules/LoginLogout.aspx?status=" + HttpUtility.UrlEncode(status) + "&remark=" + HttpUtility.UrlEncode(remark), false);
                            return;
                        }
                        else
                        {
                            // Redirect to EmployeeLoginDetails.aspx with status and remark as query string parameters
                            Response.Redirect("/View/Modules/Home.aspx?status=" + HttpUtility.UrlEncode(status) + "&remark=" + HttpUtility.UrlEncode(remark), false);
                            return;
                        }

                        //return;


                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "PageAccessSavedScript",
                        //       "showpageAccessSavedMessage('" + status + "', '" + remark + "');" +
                        //       "setTimeout(function(){ window.location.href = 'Viewtask.aspx'; }, 5000);", true);
                        //return;
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "PageAccessSavedScript", "showpageAccessSavedMessage('" + status + "', '" + remark + "');", true);

                        //                    Page.ClientScript.RegisterStartupScript(GetType(), "PageAccessSavedScript", @"showpageAccessSavedMessage('error', 'You do not have the right to access this page.');
                        //    setTimeout(function(){window.location.href = 'Home.aspx';}, 5000);
                        //", true);

                        //Response.Redirect("~/View/Modules/Home.aspx");
                        //Page.ClientScript.RegisterStartupScript(GetType(),"UserSavedScript","showPatientSavedMessage('" + "0" + "', '" + "You do not have the right to access this page." + "'); window.location.replace('Home.aspx');",true);


                    }
                    if (Session["userId"] != null)
                    {
                        CommonBL commonBL = new CommonBL();
                        var logoList = commonBL.GetCompanyLogoByUser(userId); 

                        if (logoList != null && logoList.Count > 0)
                        {
                            string logoPath = logoList[0].LogoPath; // "assets/images/NEW_IMSET_LOGO.png"
                            imgLogoSmall.ImageUrl = ResolveUrl("~/" + logoPath);
                            imgLogoLarge.ImageUrl = ResolveUrl("~/" + logoPath);

                        }
                    }


                    BindMenuList();
                    GetUserName();
                }
            }
            else
            {
                Response.Redirect("~/view/authentication/login.aspx", false);
            }
        }


        public void GetUserName()
        {
            try
            {
                userBAL userBAL = new userBAL();
                MenuDO menuDO = new MenuDO();
                menuDO.userId = Convert.ToString(Session["userId"]);
                List<MenuDO> menuDataList = userBAL.GetUser(menuDO);
                if (menuDataList.Count > 0)
                {
                    MenuDO userDetails = menuDataList[0];
                    txtuserName.Text = userDetails.username;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Site1.Master", "GetUserName", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private void BindMenuList()
        {
            try
            {
                userBAL userBAL = new userBAL();
                MenuDO menuDO = new MenuDO();
                menuDO.type = "GetMenuListDynamic";
                menuDO.userId = Convert.ToString(Session["userid"]);
                List<MenuData> menuDataList = userBAL.GetMenuHierarchy(menuDO);
                menuDataList = menuDataList
                    .Where(x =>
                    {
                        string menuName = (x.Menu ?? string.Empty).Trim();
                        string menuLink = (x.MenuLink ?? string.Empty).Trim();
                        bool isHomeByName = string.Equals(menuName, "Home", StringComparison.OrdinalIgnoreCase);
                        bool isHomeByLink = menuLink.IndexOf("/View/Modules/Home.aspx", StringComparison.OrdinalIgnoreCase) >= 0;
                        bool isStandaloneDocument = IsStandaloneDocumentMenu(menuName, menuLink);
                        return !isHomeByName && !isHomeByLink && !isStandaloneDocument;
                    })
                    .ToList();

                foreach (var menu in menuDataList)
                {
                    if (menu.SubMenus == null) continue;

                    menu.SubMenus = menu.SubMenus
                        .Where(s =>
                        {
                            string subMenuName = (s.SubMenu ?? string.Empty).Trim();
                            string subMenuLink = (s.SubMenuLink ?? string.Empty).Trim();
                            return !IsStandaloneDocumentMenu(subMenuName, subMenuLink);
                        })
                        .ToList();
                }
                rptMainAndSubMenu.DataSource = menuDataList;
                rptMainAndSubMenu.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Site1.Master", "BindMenuList", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void rptMainAndSubMenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    MenuData menuData = (MenuData)e.Item.DataItem;
                    string menuName = (menuData.Menu ?? string.Empty).Trim();
                    string menuLink = (menuData.MenuLink ?? string.Empty).Trim();
                    bool isHomeByName = string.Equals(menuName, "Home", StringComparison.OrdinalIgnoreCase);
                    bool isHomeByLink = menuLink.IndexOf("/View/Modules/Home.aspx", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool isStandaloneDocument = IsStandaloneDocumentMenu(menuName, menuLink);

                    if (isHomeByName || isHomeByLink || isStandaloneDocument)
                    {
                        e.Item.Visible = false;
                        return;
                    }

                    Repeater rptSubMenu = (Repeater)e.Item.FindControl("rptSubMenu");
                    rptSubMenu.DataSource = menuData.SubMenus;
                    rptSubMenu.DataBind();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Site1.Master", "rptMainAndSubMenu_ItemDataBound", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void Logo_Click(object sender, EventArgs e)
        {
            try
            {
                int roleId = Convert.ToInt32(Session["roleid"]);

                if (roleId == 2)
                {
                    Response.Redirect("/View/Modules/LoginLogout.aspx", false);

                }
                else
                {
                    Response.Redirect("/View/Modules/Home.aspx", false);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Site1.Master", "Logo_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                int roleId = Convert.ToInt32(Session["roleid"]);
                if (roleId == 2)
                {
                    txtTimer.Visible = true;
                    Timer1.Enabled = true;
                    if (Session["LoginTime"] != null && Convert.ToString(Session["LogoutTime"]) == "NULL")
                    {
                        DateTime loginTime = Convert.ToDateTime(Session["LoginTime"]);

                        TimeSpan timeElapsed = DateTime.Now - loginTime;

                        txtTimer.Text = "Work Hours: " + timeElapsed.ToString(@"hh\:mm");
                    }
                    else if (Session["LoginTime"] != null && Convert.ToString(Session["LogoutTime"]) != "NULL")
                    {
                        DateTime loginTime = Convert.ToDateTime(Session["LoginTime"]);
                        DateTime logoutTime = Convert.ToDateTime(Session["LogoutTime"]);

                        TimeSpan timeElapsed = logoutTime - loginTime;

                        txtTimer.Text = "Work Hours: " + timeElapsed.ToString(@"hh\:mm");

                        Timer1.Enabled = false;
                    }
                }
                else
                {
                    txtTimer.Visible = false;
                    Timer1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Site1.Master", "Timer1_Tick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

    }
}
