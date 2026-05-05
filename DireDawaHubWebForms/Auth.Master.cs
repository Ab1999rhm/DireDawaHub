using System;
using System.Web.UI;

namespace DireDawaHubWebForms
{
    public partial class AuthMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void BtnLogin_Click(object sender, EventArgs e) { Response.Redirect("~/Login.aspx"); }
        protected void BtnContributor_Click(object sender, EventArgs e) { Response.Redirect("~/Register.aspx"); }
    }
}
