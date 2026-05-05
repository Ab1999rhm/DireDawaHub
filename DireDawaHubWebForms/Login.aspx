<%@ Page Title="Login" Language="C#" MasterPageFile="~/Auth.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DireDawaHubWebForms.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- MAIN CANVAS -->
    <asp:Panel ID="pnlLoginCanvas" runat="server" Style="position: relative; width: 1200px; height: 1000px; margin: 0 auto; font-family: 'Segoe UI', Arial, sans-serif;">
        
        <!-- AUTH CARD -->
        <asp:Panel ID="pnlAuthCard" runat="server" BackColor="White" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 200px; top: 150px; width: 800px; height: 550px; border-radius: 24px; box-shadow: 0 25px 50px rgba(0,0,0,0.3); overflow: hidden;">
            
            <!-- LEFT SIDEBAR -->
            <asp:Panel ID="pnlAuthSidebar" runat="server" BackColor="#F0F4F8" Style="position: absolute; left: 0px; top: 0px; width: 300px; height: 550px; border-right: 1px solid #E2E8F0;">
                <asp:Label ID="lblStatus" runat="server" Text="LIVE SYSTEM" BackColor="#DCFCE7" ForeColor="#166534" Font-Bold="true" Font-Size="12px" Style="position: absolute; left: 30px; top: 40px; padding: 5px 10px; border-radius: 15px;"></asp:Label>
                
                <asp:Label ID="lblWelcome" runat="server" Text="Welcome to the Portal" Font-Bold="true" Font-Size="24px" ForeColor="#1E3A8A" Style="position: absolute; left: 30px; top: 100px; width: 240px;"></asp:Label>
                <asp:Label ID="lblDesc" runat="server" Text="Access the Dire Dawa Community Hub management system to update resources and insights." Font-Size="14px" ForeColor="#64748B" Style="position: absolute; left: 30px; top: 180px; width: 240px;"></asp:Label>
                
                <!-- Stat Box 1 -->
                <asp:Panel ID="pnlStat1" runat="server" BackColor="White" BorderStyle="Solid" BorderColor="#E2E8F0" BorderWidth="1px" Style="position: absolute; left: 30px; top: 300px; width: 240px; height: 80px; border-radius: 12px;">
                    <asp:Label ID="lblStat1Header" runat="server" Text="LIVE STATISTICS" Font-Bold="true" Font-Size="11px" ForeColor="#3B82F6" Style="position: absolute; left: 15px; top: 10px;"></asp:Label>
                    <asp:Label ID="lblStat1Val" runat="server" Text="Community Driven" Font-Bold="true" Font-Size="16px" ForeColor="#0F172A" Style="position: absolute; left: 15px; top: 30px;"></asp:Label>
                    <asp:Label ID="lblStat1Sub" runat="server" Text="Join 50+ contributors today" Font-Size="12px" ForeColor="#64748B" Style="position: absolute; left: 15px; top: 55px;"></asp:Label>
                </asp:Panel>

                <!-- Stat Box 2 -->
                <asp:Panel ID="pnlStat2" runat="server" BackColor="White" BorderStyle="Solid" BorderColor="#E2E8F0" BorderWidth="1px" Style="position: absolute; left: 30px; top: 400px; width: 240px; height: 80px; border-radius: 12px;">
                    <asp:Label ID="lblStat2Header" runat="server" Text="DATA INTEGRITY" Font-Bold="true" Font-Size="11px" ForeColor="#10B981" Style="position: absolute; left: 15px; top: 10px;"></asp:Label>
                    <asp:Label ID="lblStat2Val" runat="server" Text="Verified Updates" Font-Bold="true" Font-Size="16px" ForeColor="#0F172A" Style="position: absolute; left: 15px; top: 30px;"></asp:Label>
                    <asp:Label ID="lblStat2Sub" runat="server" Text="100% Secure communication" Font-Size="12px" ForeColor="#64748B" Style="position: absolute; left: 15px; top: 55px;"></asp:Label>
                </asp:Panel>
            </asp:Panel>

            <!-- RIGHT FORM SECTION -->
            <asp:Panel ID="pnlAuthForm" runat="server" BackColor="White" Style="position: absolute; left: 300px; top: 0px; width: 500px; height: 550px;">
                <asp:Image ID="imgBanner" runat="server" ImageUrl="~/images/banner.png" Style="position: absolute; left: 50px; top: 40px; width: 400px; height: 100px; object-fit: cover; border-radius: 12px;" />
                
                <asp:Label ID="lblSignIn" runat="server" Text="Sign In" Font-Bold="true" Font-Size="24px" ForeColor="#0F172A" Style="position: absolute; left: 50px; top: 160px;"></asp:Label>
                <asp:Label ID="lblSignInDesc" runat="server" Text="Enter your credentials to manage the hub" Font-Size="14px" ForeColor="#64748B" Style="position: absolute; left: 50px; top: 195px;"></asp:Label>

                <!-- Email Input -->
                <asp:Label ID="lblEmail" runat="server" Text="Email Address" Font-Bold="true" Font-Size="12px" ForeColor="#0F172A" Style="position: absolute; left: 50px; top: 240px;"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" Width="380px" Height="30px" BackColor="#F8FAFC" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 50px; top: 260px; border-radius: 8px; padding: 5px 10px;"></asp:TextBox>

                <!-- Password Input -->
                <asp:Label ID="lblPassword" runat="server" Text="Password" Font-Bold="true" Font-Size="12px" ForeColor="#0F172A" Style="position: absolute; left: 50px; top: 320px;"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Width="380px" Height="30px" BackColor="#F8FAFC" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 50px; top: 340px; border-radius: 8px; padding: 5px 10px;"></asp:TextBox>

                <!-- Remember Me -->
                <asp:CheckBox ID="chkRemember" runat="server" Text="Remember me" Font-Size="12px" ForeColor="#64748B" Style="position: absolute; left: 50px; top: 395px;" />
                <asp:LinkButton ID="lnkForgot" runat="server" Text="Forgot?" Font-Bold="true" Font-Size="12px" ForeColor="#3B82F6" Style="position: absolute; left: 405px; top: 395px; text-decoration: none;"></asp:LinkButton>

                <!-- Login Button -->
                <asp:Button ID="btnLoginSubmit" runat="server" Text="Sign Into Dashboard" BackColor="#1E40AF" ForeColor="White" Font-Bold="true" BorderStyle="None" Width="400px" Height="45px" Style="position: absolute; left: 50px; top: 435px; border-radius: 12px; cursor: pointer;" OnClick="btnLoginSubmit_Click" />

                <!-- Register Link -->
                <asp:Label ID="lblRegisterText" runat="server" Text="Need to share information?" Font-Size="12px" ForeColor="#64748B" Style="position: absolute; left: 140px; top: 500px;"></asp:Label>
                <asp:LinkButton ID="lnkRegister" runat="server" Text="Register as Contributor" Font-Bold="true" Font-Size="12px" ForeColor="#3B82F6" Style="position: absolute; left: 300px; top: 500px; text-decoration: none;" OnClick="lnkRegister_Click"></asp:LinkButton>

            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server"></asp:Content>
