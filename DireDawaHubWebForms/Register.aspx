<%@ Page Title="Register" Language="C#" MasterPageFile="~/Auth.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="DireDawaHubWebForms.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- MAIN CANVAS -->
    <asp:Panel ID="pnlRegisterCanvas" runat="server" Style="position: relative; width: 1200px; height: 1222px; margin: 0 auto; font-family: 'Segoe UI', Arial, sans-serif; top: 0px; left: 0px;">
        
        <!-- AUTH CARD -->
        <asp:Panel ID="pnlRegCard" runat="server" BackColor="White" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 253px; top: 50px; width: 698px; height: 957px; border-radius: 20px; box-shadow: 0 15px 35px rgba(0,0,0,0.2); overflow: hidden;">
            
            <asp:Image ID="imgRegBanner" runat="server" ImageUrl="~/images/banner.png" Style="position: absolute; left: 3px; top: 0px; width: 693px; height: 120px; object-fit: cover; border-bottom: 1px solid #E2E8F0;" />
            
            <asp:Label ID="lblRegHeader" runat="server" Text="Contributor Registration" Font-Bold="true" Font-Size="28px" ForeColor="#0F172A" Style="position: absolute; left: 87px; top: 150px; text-align: center; width: 458px;"></asp:Label>
            <asp:Label ID="lblRegSub" runat="server" Text="Join the Dire Dawa Hub community" Font-Size="14px" ForeColor="#475569" Style="position: absolute; left: 150px; top: 190px; text-align: center; width: 300px;"></asp:Label>

            <!-- Document Requirements -->
            <asp:Panel ID="pnlReqs" runat="server" BackColor="#F0F9FF" BorderStyle="Solid" BorderColor="#3B82F6" BorderWidth="4px" Style="position: absolute; left: 50px; top: 240px; width: 492px; height: 90px; border-top: none; border-right: none; border-bottom: none; border-radius: 0 8px 8px 0;">
                <asp:Label ID="lblReqTitle" runat="server" Text="Document Requirements:" Font-Bold="true" Font-Size="14px" ForeColor="#1E40AF" Style="position: absolute; left: 15px; top: 10px;"></asp:Label>
                <asp:Label ID="lblReq1" runat="server" Text="• Upload a clear photo of your Work ID card or Government ID" Font-Size="12px" ForeColor="#475569" Style="position: absolute; left: 15px; top: 35px;"></asp:Label>
                <asp:Label ID="lblReq2" runat="server" Text="• File size: Max 5MB (JPG, PNG, PDF accepted)" Font-Size="12px" ForeColor="#475569" Style="position: absolute; left: 15px; top: 55px;"></asp:Label>
            </asp:Panel>

            <!-- Form Row 1 -->
            <asp:Label ID="lblRegEmail" runat="server" Text="Email Address" Font-Bold="true" Font-Size="12px" ForeColor="#0F172A" Style="position: absolute; left: 50px; top: 360px;"></asp:Label>
            <asp:TextBox ID="txtRegEmail" runat="server" Width="220px" Height="30px" BackColor="#F8FAFC" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 50px; top: 380px; border-radius: 8px; padding: 5px 10px;"></asp:TextBox>

            <asp:Label ID="lblRegWorkId" runat="server" Text="Work ID Number" Font-Bold="true" Font-Size="12px" ForeColor="#0F172A" Style="position: absolute; left: 320px; top: 360px;"></asp:Label>
            <asp:TextBox ID="txtRegWorkId" runat="server" Width="210px" Height="30px" BackColor="#F8FAFC" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 320px; top: 380px; border-radius: 8px; padding: 5px 10px;"></asp:TextBox>

            <!-- Doc Type -->
            <asp:Label ID="lblDocType" runat="server" Text="Document Type" Font-Bold="true" Font-Size="12px" ForeColor="#0F172A" Style="position: absolute; left: 50px; top: 440px;"></asp:Label>
            <asp:DropDownList ID="ddlDocType" runat="server" Width="500px" Height="40px" BackColor="#F8FAFC" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 50px; top: 460px; border-radius: 8px; padding: 5px 10px;">
                <asp:ListItem Text="Municipal Employee Work ID" Value="WorkId"></asp:ListItem>
                <asp:ListItem Text="National Government ID" Value="GovernmentId"></asp:ListItem>
                <asp:ListItem Text="Passport" Value="Passport"></asp:ListItem>
            </asp:DropDownList>

            <!-- File Upload -->
            <asp:Label ID="lblUpload" runat="server" Text="ID Document Photo" Font-Bold="true" Font-Size="12px" ForeColor="#0F172A" Style="position: absolute; left: 50px; top: 520px;"></asp:Label>
            <asp:Panel ID="pnlUpload" runat="server" BackColor="#F8FAFC" BorderStyle="Dashed" BorderColor="#CBD5E1" BorderWidth="2px" Style="position: absolute; left: 50px; top: 540px; width: 496px; height: 80px; border-radius: 12px; text-align: center;">
                <asp:FileUpload ID="fileIdDoc" runat="server" Style="position: absolute; left: 100px; top: 30px; width: 300px;" />
            </asp:Panel>

            <!-- Password Row -->
            <asp:Label ID="lblRegPass" runat="server" Text="Password" Font-Bold="true" Font-Size="12px" ForeColor="#0F172A" Style="position: absolute; left: 50px; top: 650px;"></asp:Label>
            <asp:TextBox ID="txtRegPass" runat="server" TextMode="Password" Width="220px" Height="30px" BackColor="#F8FAFC" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 50px; top: 670px; border-radius: 8px; padding: 5px 10px;"></asp:TextBox>

            <asp:Label ID="lblRegConf" runat="server" Text="Confirm Password" Font-Bold="true" Font-Size="12px" ForeColor="#0F172A" Style="position: absolute; left: 320px; top: 650px;"></asp:Label>
            <asp:TextBox ID="txtRegConf" runat="server" TextMode="Password" Width="210px" Height="30px" BackColor="#F8FAFC" BorderStyle="Solid" BorderColor="#CBD5E1" BorderWidth="1px" Style="position: absolute; left: 320px; top: 670px; border-radius: 8px; padding: 5px 10px;"></asp:TextBox>

            <!-- Submit Button -->
            <asp:Button ID="btnRegSubmit" runat="server" Text="Submit for Verification" BackColor="#1E40AF" ForeColor="White" Font-Bold="true" BorderStyle="None" Width="500px" Height="45px" Style="position: absolute; left: 50px; top: 740px; border-radius: 10px; cursor: pointer;" OnClick="btnRegSubmit_Click" />

            <!-- Login Link -->
            <asp:Label ID="lblLoginText" runat="server" Text="Already have an account?" Font-Size="12px" ForeColor="#64748B" Style="position: absolute; left: 200px; top: 820px;"></asp:Label>
            <asp:LinkButton ID="lnkLogin" runat="server" Text="Sign in here" Font-Bold="true" Font-Size="12px" ForeColor="#3B82F6" Style="position: absolute; left: 350px; top: 820px; text-decoration: none;" OnClick="lnkLogin_Click"></asp:LinkButton>

        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server"></asp:Content>
