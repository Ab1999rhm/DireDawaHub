using System;
using System.Web.UI;

namespace DireDawaHubWebForms
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRegSubmit_Click(object sender, EventArgs e)
        {
            // Register Logic
            Response.Redirect("~/Login.aspx");
        }

        protected void lnkLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login.aspx");
        }
    }
}
