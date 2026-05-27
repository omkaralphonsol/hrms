using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProcessModel;
using System.Configuration;

namespace HRMS.View.Authentication
{
    public partial class Captcha : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            getCaptchaData();
        }
        private void getCaptchaData()
        {
            try
            {
                Bitmap objBitmap = new Bitmap(300, 200); // Increase both width and height
                Graphics objGraphics = Graphics.FromImage(objBitmap);
                objGraphics.Clear(Color.White);

                Random objRandom = new Random();

                // Generate the image for captcha  
                string captchaText = string.Format("{0:X}", objRandom.Next(1000000, 9999999));
                Session["CaptchaVerify"] = captchaText;

                Font objFont = new Font("Arial", 40, FontStyle.Bold); // Increase font size for better visibility

                // Enable anti-aliasing for smoother edges
                objGraphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw the image for captcha  
                objGraphics.DrawString(captchaText, objFont, Brushes.Black, 40, 80); // Adjust X and Y position for centering

                objBitmap.Save(Response.OutputStream, ImageFormat.Gif);
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
               errorlog.fnStoreErrorLog("Captcha", "getCaptchaData", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
    }
}