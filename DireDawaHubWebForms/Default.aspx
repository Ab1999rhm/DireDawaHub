<%@ Page Title="Dire Dawa Hub" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DireDawaHubWebForms._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- No external CSS logic used here to prevent breaking Visual Studio Designer -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- MAIN CANVAS FOR DRAG AND DROP -->
    <asp:Panel ID="pnlDashboardCanvas" runat="server" Style="position: relative; width: 1200px; height: 2300px; margin: 0 auto; font-family: 'Segoe UI', Arial, sans-serif;">
        
        <!-- HERO SECTION -->
        <asp:Panel ID="pnlHero" runat="server" Style="position: absolute; left: 0px; top: 0px; width: 1200px; height: 300px; text-align: center;">
            <asp:Label ID="lblHeroTitle" runat="server" Text="Dire Dawa Hub" Font-Bold="True" Font-Size="64px" ForeColor="White" Style="position: absolute; left: 350px; top: 50px;"></asp:Label>
            <asp:Label ID="lblHeroSubtitle" runat="server" Text="The Smart City Gateway for Residents & Businesses" Font-Size="24px" ForeColor="#94A3B8" Style="position: absolute; left: 320px; top: 140px;"></asp:Label>
            
            <asp:Panel ID="pnlSearchBox" runat="server" BackColor="#0F172A" BorderStyle="Solid" BorderColor="#3B82F6" BorderWidth="2px" Style="position: absolute; left: 300px; top: 200px; width: 600px; height: 60px; border-radius: 30px;">
                <asp:TextBox ID="txtGlobalSearch" runat="server" Height="40px" BackColor="Transparent" ForeColor="White" BorderStyle="None" Font-Size="18px" Style="position: absolute; left: 20px; top: 10px; width: 409px;"></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="Search" BackColor="#3B82F6" ForeColor="White" BorderStyle="None" Width="120px" Height="46px" Font-Bold="true" Font-Size="16px" Style="position: absolute; left: 470px; top: 5px; border-radius: 25px;" OnClick="btnSearch_Click" />
            </asp:Panel>
        </asp:Panel>

        <!-- QUICK NAV PILLS -->
        <asp:Panel ID="pnlQuickNav" runat="server" Style="position: absolute; left: 50px; top: 320px; width: 1100px; height: 50px;">
            <asp:Button ID="btnNavWater" runat="server" Text="Water Services" BackColor="#1E293B" ForeColor="White" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Height="40px" Width="150px" Font-Bold="true" Style="position: absolute; left: 100px; top: 0px; border-radius: 20px;" />
            <asp:Button ID="btnNavHealth" runat="server" Text="Health Clinics" BackColor="#1E293B" ForeColor="White" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Height="40px" Width="150px" Font-Bold="true" Style="position: absolute; left: 280px; top: 0px; border-radius: 20px;" />
            <asp:Button ID="btnNavAgri" runat="server" Text="Agriculture" BackColor="#1E293B" ForeColor="White" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Height="40px" Width="150px" Font-Bold="true" Style="position: absolute; left: 460px; top: 0px; border-radius: 20px;" />
            <asp:Button ID="btnNavJobs" runat="server" Text="Job Portal" BackColor="#1E293B" ForeColor="White" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Height="40px" Width="150px" Font-Bold="true" Style="position: absolute; left: 640px; top: 0px; border-radius: 20px;" />
            <asp:Button ID="btnNavEdu" runat="server" Text="Education" BackColor="#1E293B" ForeColor="White" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Height="40px" Width="150px" Font-Bold="true" Style="position: absolute; left: 820px; top: 0px; border-radius: 20px;" />
        </asp:Panel>

        <!-- STATS GRID -->
        <asp:Panel ID="pnlStatWater" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#3B82F6" BorderWidth="1px" Style="position: absolute; left: 50px; top: 400px; width: 250px; height: 120px; border-radius: 16px; text-align: center;">
            <asp:Label ID="lblWaterCount" runat="server" Text="12" Font-Size="40px" Font-Bold="true" ForeColor="#60A5FA" Style="position: absolute; left: 0px; top: 20px; width: 100%;"></asp:Label>
            <asp:Label ID="lblWaterLabel" runat="server" Text="Active Water Routes" Font-Size="14px" Font-Bold="true" ForeColor="#94A3B8" Style="position: absolute; left: 0px; top: 75px; width: 100%; text-transform: uppercase;"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlStatClinic" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#10B981" BorderWidth="1px" Style="position: absolute; left: 330px; top: 400px; width: 250px; height: 120px; border-radius: 16px; text-align: center;">
            <asp:Label ID="lblClinicCount" runat="server" Text="8" Font-Size="40px" Font-Bold="true" ForeColor="#34D399" Style="position: absolute; left: 0px; top: 20px; width: 100%;"></asp:Label>
            <asp:Label ID="lblClinicLabel" runat="server" Text="Health Clinics" Font-Size="14px" Font-Bold="true" ForeColor="#94A3B8" Style="position: absolute; left: 0px; top: 75px; width: 100%; text-transform: uppercase;"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlStatMarket" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#F59E0B" BorderWidth="1px" Style="position: absolute; left: 610px; top: 400px; width: 250px; height: 120px; border-radius: 16px; text-align: center;">
            <asp:Label ID="lblMarketCount" runat="server" Text="24" Font-Size="40px" Font-Bold="true" ForeColor="#FBBF24" Style="position: absolute; left: 0px; top: 20px; width: 100%;"></asp:Label>
            <asp:Label ID="lblMarketLabel" runat="server" Text="Market Reports" Font-Size="14px" Font-Bold="true" ForeColor="#94A3B8" Style="position: absolute; left: 0px; top: 75px; width: 100%; text-transform: uppercase;"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlStatJobs" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#8B5CF6" BorderWidth="1px" Style="position: absolute; left: 890px; top: 400px; width: 250px; height: 120px; border-radius: 16px; text-align: center;">
            <asp:Label ID="lblJobCount" runat="server" Text="15" Font-Size="40px" Font-Bold="true" ForeColor="#A78BFA" Style="position: absolute; left: 0px; top: 20px; width: 100%;"></asp:Label>
            <asp:Label ID="lblJobLabel" runat="server" Text="Job Postings" Font-Size="14px" Font-Bold="true" ForeColor="#94A3B8" Style="position: absolute; left: 0px; top: 75px; width: 100%; text-transform: uppercase;"></asp:Label>
        </asp:Panel>

        <!-- DASHBOARD WIDGETS -->

        <!-- Community Posters -->
        <asp:Panel ID="PostersSection" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Style="position: absolute; left: 50px; top: 560px; width: 530px; height: 500px; border-radius: 16px;">
            <asp:Label ID="lblPostersHeader" runat="server" Text="Community Posters" Font-Size="24px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
            <asp:Panel ID="pnlPostersList" runat="server" Style="position: absolute; left: 20px; top: 70px; width: 490px; height: 410px; overflow: hidden;">
                <asp:Panel ID="pnlPoster1" runat="server" BackColor="#0F172A" Style="position: absolute; left: 0px; top: 0px; width: 490px; height: 190px; border-radius: 12px; overflow: hidden;">
                    <asp:Image ID="imgPoster1" runat="server" ImageUrl="~/images/sample_poster1.jpg" Style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; object-fit: cover; opacity: 0.7;" />
                    <asp:Label ID="lblPoster1Title" runat="server" Text="Tech In Dire Dawa" Font-Size="20px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 130px;"></asp:Label>
                    <asp:Label ID="lblPoster1Desc" runat="server" Text="Coming this summer to the community center." Font-Size="14px" ForeColor="#CBD5E1" Style="position: absolute; left: 20px; top: 160px;"></asp:Label>
                </asp:Panel>
                <asp:Panel ID="pnlPoster2" runat="server" BackColor="#0F172A" Style="position: absolute; left: 0px; top: 210px; width: 490px; height: 190px; border-radius: 12px; overflow: hidden;">
                    <asp:Image ID="imgPoster2" runat="server" ImageUrl="~/images/sample_poster2.jpg" Style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; object-fit: cover; opacity: 0.7;" />
                    <asp:Label ID="lblPoster2Title" runat="server" Text="Morning Yoga" Font-Size="20px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 130px;"></asp:Label>
                    <asp:Label ID="lblPoster2Desc" runat="server" Text="Join us every Saturday at 7 AM in the park." Font-Size="14px" ForeColor="#CBD5E1" Style="position: absolute; left: 20px; top: 160px;"></asp:Label>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>

        <!-- Weather Widget -->
        <asp:Panel ID="WeatherSection" runat="server" BackColor="#0EA5E9" BorderStyle="None" Style="position: absolute; left: 610px; top: 560px; width: 530px; height: 230px; border-radius: 16px;">
            <asp:Label ID="lblWeatherCity" runat="server" Text="Dire Dawa, Ethiopia" Font-Bold="True" Font-Size="28px" ForeColor="White" Style="position: absolute; left: 30px; top: 30px;"></asp:Label>
            <asp:Label ID="lblWeatherDate" runat="server" Text="Today" Font-Size="16px" ForeColor="#E0F2FE" Style="position: absolute; left: 30px; top: 70px;"></asp:Label>
            <asp:Label ID="lblWeatherTemp" runat="server" Text="32°C" Font-Bold="True" Font-Size="64px" ForeColor="White" Style="position: absolute; left: 30px; top: 110px;"></asp:Label>
            <asp:Label ID="lblWeatherCondition" runat="server" Text="Sunny" Font-Bold="True" Font-Size="24px" ForeColor="White" Style="position: absolute; left: 200px; top: 120px;"></asp:Label>
            <asp:Label ID="lblWeatherIcon" runat="server" Text="[Sunny]" Font-Size="60px" ForeColor="White" Style="position: absolute; left: 400px; top: 70px;"></asp:Label>
        </asp:Panel>

        <!-- Education Widget -->
        <asp:Panel ID="EducationSection" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Style="position: absolute; left: 610px; top: 820px; width: 530px; height: 240px; border-radius: 16px;">
            <asp:Label ID="lblEduHeader" runat="server" Text="Education Updates" Font-Size="24px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
            <asp:Panel ID="pnlEduItem1" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 70px; width: 490px; height: 70px; border-radius: 8px;">
                <asp:Label ID="lblEdu1Title" runat="server" Text="School Registration Open" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 15px; top: 15px;"></asp:Label>
                <asp:Label ID="lblEdu1Desc" runat="server" Text="Register your children for the upcoming semester." Font-Size="12px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 40px;"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlEduItem2" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 150px; width: 490px; height: 70px; border-radius: 8px;">
                <asp:Label ID="lblEdu2Title" runat="server" Text="New Library Hours" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 15px; top: 15px;"></asp:Label>
                <asp:Label ID="lblEdu2Desc" runat="server" Text="The city library is now open until 9 PM." Font-Size="12px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 40px;"></asp:Label>
            </asp:Panel>
        </asp:Panel>

        <!-- Water Tracker -->
        <asp:Panel ID="WaterSection" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Style="position: absolute; left: 50px; top: 1100px; width: 530px; height: 300px; border-radius: 16px;">
            <asp:Label ID="lblWaterHeader" runat="server" Text="Water Supply Tracker" Font-Size="24px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
            <asp:Panel ID="pnlWaterItem1" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 70px; width: 490px; height: 100px; border-radius: 8px; border-left: 4px solid #10B981;">
                <asp:Label ID="lblWater1Loc" runat="server" Text="Kezira District" Font-Bold="true" ForeColor="White" Font-Size="18px" Style="position: absolute; left: 15px; top: 15px;"></asp:Label>
                <asp:Label ID="lblWater1Status" runat="server" Text="ACTIVE" BackColor="#10B981" ForeColor="White" Font-Bold="true" Font-Size="12px" Style="position: absolute; right: 15px; top: 15px; padding: 5px 10px; border-radius: 12px;"></asp:Label>
                <asp:Label ID="lblWater1Time" runat="server" Text="Today, 8:00 AM - 12:00 PM" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 45px;"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlWaterItem2" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 180px; width: 490px; height: 100px; border-radius: 8px; border-left: 4px solid #F59E0B;">
                <asp:Label ID="lblWater2Loc" runat="server" Text="Megala Area" Font-Bold="true" ForeColor="White" Font-Size="18px" Style="position: absolute; left: 15px; top: 15px;"></asp:Label>
                <asp:Label ID="lblWater2Status" runat="server" Text="SCHEDULED" BackColor="#F59E0B" ForeColor="White" Font-Bold="true" Font-Size="12px" Style="position: absolute; right: 15px; top: 15px; padding: 5px 10px; border-radius: 12px;"></asp:Label>
                <asp:Label ID="lblWater2Time" runat="server" Text="Tomorrow, 2:00 PM - 6:00 PM" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 45px;"></asp:Label>
            </asp:Panel>
        </asp:Panel>

        <!-- Health Clinics -->
        <asp:Panel ID="HealthSection" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Style="position: absolute; left: 610px; top: 1100px; width: 530px; height: 300px; border-radius: 16px;">
            <asp:Label ID="lblHealthHeader" runat="server" Text="Health Clinics" Font-Size="24px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
            <asp:Panel ID="pnlHealthItem1" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 70px; width: 490px; height: 100px; border-radius: 8px;">
                <asp:Label ID="lblHealth1Name" runat="server" Text="Dil Chora Hospital" Font-Bold="true" ForeColor="White" Font-Size="18px" Style="position: absolute; left: 15px; top: 15px;"></asp:Label>
                <asp:Label ID="lblHealth1Stock" runat="server" Text="STOCK OK" BackColor="#10B981" ForeColor="White" Font-Bold="true" Font-Size="12px" Style="position: absolute; right: 15px; top: 15px; padding: 5px 10px; border-radius: 12px;"></asp:Label>
                <asp:Label ID="lblHealth1Docs" runat="server" Text="12 Doctors Available" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 45px;"></asp:Label>
                <asp:Label ID="lblHealth1Phone" runat="server" Text="Tel: 025-111-2233" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 200px; top: 45px;"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlHealthItem2" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 180px; width: 490px; height: 100px; border-radius: 8px;">
                <asp:Label ID="lblHealth2Name" runat="server" Text="Sabian Primary Clinic" Font-Bold="true" ForeColor="White" Font-Size="18px" Style="position: absolute; left: 15px; top: 15px;"></asp:Label>
                <asp:Label ID="lblHealth2Stock" runat="server" Text="LOW STOCK" BackColor="#EF4444" ForeColor="White" Font-Bold="true" Font-Size="12px" Style="position: absolute; right: 15px; top: 15px; padding: 5px 10px; border-radius: 12px;"></asp:Label>
                <asp:Label ID="lblHealth2Docs" runat="server" Text="3 Doctors Available" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 45px;"></asp:Label>
                <asp:Label ID="lblHealth2Phone" runat="server" Text="Tel: 025-111-4455" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 200px; top: 45px;"></asp:Label>
            </asp:Panel>
        </asp:Panel>

        <!-- Jobs Portal -->
        <asp:Panel ID="JobsSection" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Style="position: absolute; left: 50px; top: 1440px; width: 530px; height: 300px; border-radius: 16px;">
            <asp:Label ID="lblJobsHeader" runat="server" Text="Local Job Portal" Font-Size="24px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
            <asp:Panel ID="pnlJobItem1" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 70px; width: 490px; height: 100px; border-radius: 8px;">
                <asp:Label ID="lblJob1Title" runat="server" Text="Senior Software Engineer" Font-Bold="true" ForeColor="White" Font-Size="18px" Style="position: absolute; left: 15px; top: 15px;"></asp:Label>
                <asp:Label ID="lblJob1Type" runat="server" Text="JOB" BackColor="#3B82F6" ForeColor="White" Font-Bold="true" Font-Size="12px" Style="position: absolute; right: 15px; top: 15px; padding: 5px 10px; border-radius: 12px;"></asp:Label>
                <asp:Label ID="lblJob1Company" runat="server" Text="Tech Ethio - Dire Dawa City" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 45px;"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlJobItem2" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 180px; width: 490px; height: 100px; border-radius: 8px;">
                <asp:Label ID="lblJob2Title" runat="server" Text="Web Development Training" Font-Bold="true" ForeColor="White" Font-Size="18px" Style="position: absolute; left: 15px; top: 15px;"></asp:Label>
                <asp:Label ID="lblJob2Type" runat="server" Text="TRAINING" BackColor="#F59E0B" ForeColor="Black" Font-Bold="true" Font-Size="12px" Style="position: absolute; right: 15px; top: 15px; padding: 5px 10px; border-radius: 12px;"></asp:Label>
                <asp:Label ID="lblJob2Company" runat="server" Text="DDU IT Dept - University Campus" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 45px;"></asp:Label>
            </asp:Panel>
        </asp:Panel>

        <!-- Market Reports -->
        <asp:Panel ID="MarketSection" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Style="position: absolute; left: 610px; top: 1440px; width: 530px; height: 300px; border-radius: 16px;">
            <asp:Label ID="lblMarketHeader" runat="server" Text="Agriculture Market" Font-Size="24px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
            <asp:Panel ID="pnlMarketItem1" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 70px; width: 490px; height: 100px; border-radius: 8px;">
                <asp:Label ID="lblMarket1Date" runat="server" Text="MAY 05" Font-Bold="true" ForeColor="#3B82F6" Font-Size="12px" Style="position: absolute; left: 15px; top: 10px;"></asp:Label>
                <asp:Label ID="lblMarket1Crop" runat="server" Text="Sorghum" Font-Bold="true" ForeColor="White" Font-Size="20px" Style="position: absolute; left: 15px; top: 30px;"></asp:Label>
                <asp:Label ID="lblMarket1Loc" runat="server" Text="Chat Tera Market" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 60px;"></asp:Label>
                <asp:Label ID="lblMarket1Price" runat="server" Text="ETB 85.00" Font-Bold="true" ForeColor="White" Font-Size="24px" Style="position: absolute; right: 15px; top: 35px;"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlMarketItem2" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 180px; width: 490px; height: 100px; border-radius: 8px;">
                <asp:Label ID="lblMarket2Date" runat="server" Text="MAY 04" Font-Bold="true" ForeColor="#3B82F6" Font-Size="12px" Style="position: absolute; left: 15px; top: 10px;"></asp:Label>
                <asp:Label ID="lblMarket2Crop" runat="server" Text="Maize" Font-Bold="true" ForeColor="White" Font-Size="20px" Style="position: absolute; left: 15px; top: 30px;"></asp:Label>
                <asp:Label ID="lblMarket2Loc" runat="server" Text="Taiwan Market" Font-Size="14px" ForeColor="#94A3B8" Style="position: absolute; left: 15px; top: 60px;"></asp:Label>
                <asp:Label ID="lblMarket2Price" runat="server" Text="ETB 60.00" Font-Bold="true" ForeColor="White" Font-Size="24px" Style="position: absolute; right: 15px; top: 35px;"></asp:Label>
            </asp:Panel>
        </asp:Panel>

        <!-- Safety Alerts -->
        <asp:Panel ID="SafetySection" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Style="position: absolute; left: 50px; top: 1780px; width: 530px; height: 300px; border-radius: 16px;">
            <asp:Label ID="lblSafetyHeader" runat="server" Text="Public Safety Alerts" Font-Size="24px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
            <asp:Label ID="lblSafetyBadge" runat="server" Text="LIVE" BackColor="#EF4444" ForeColor="White" Font-Bold="true" Font-Size="12px" Style="position: absolute; right: 20px; top: 25px; padding: 5px 10px; border-radius: 12px;"></asp:Label>
            <asp:Panel ID="pnlSafetyItem1" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 70px; width: 490px; height: 210px; border-radius: 8px;">
                <asp:Label ID="lblSafetyIcon" runat="server" Text="[Alert]" Font-Size="24px" Font-Bold="true" ForeColor="#EF4444" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
                <asp:Label ID="lblSafety1Title" runat="server" Text="Traffic Diversion" Font-Bold="true" ForeColor="White" Font-Size="20px" Style="position: absolute; left: 90px; top: 20px;"></asp:Label>
                <asp:Label ID="lblSafety1Desc" runat="server" Text="Road construction near the main square. Please use alternate routes to avoid heavy traffic congestion." Font-Size="14px" ForeColor="#CBD5E1" Style="position: absolute; left: 90px; top: 60px; width: 380px;"></asp:Label>
                <asp:Label ID="lblSafety1Loc" runat="server" Text="Location: Main Square - Active" Font-Size="14px" ForeColor="#3B82F6" Style="position: absolute; left: 90px; top: 160px;"></asp:Label>
            </asp:Panel>
        </asp:Panel>

        <!-- Stats Section -->
        <asp:Panel ID="StatsSection" runat="server" BackColor="#1E293B" BorderStyle="Solid" BorderColor="#334155" BorderWidth="1px" Style="position: absolute; left: 610px; top: 1780px; width: 530px; height: 300px; border-radius: 16px;">
            <asp:Label ID="lblStatsHeader" runat="server" Text="Community Statistics" Font-Size="24px" Font-Bold="true" ForeColor="White" Style="position: absolute; left: 20px; top: 20px;"></asp:Label>
            
            <asp:Panel ID="pnlStatBox1" runat="server" BackColor="#0F172A" Style="position: absolute; left: 20px; top: 70px; width: 235px; height: 210px; border-radius: 8px; text-align: center;">
                <asp:Label ID="lblStatBox1Icon" runat="server" Text="[Chart]" Font-Size="24px" ForeColor="#60A5FA" Style="position: absolute; left: 0px; top: 40px; width: 100%;"></asp:Label>
                <asp:Label ID="lblStatBox1Val" runat="server" Text="1.2M+" Font-Bold="true" ForeColor="White" Font-Size="32px" Style="position: absolute; left: 0px; top: 100px; width: 100%;"></asp:Label>
                <asp:Label ID="lblStatBox1Desc" runat="server" Text="POPULATION" Font-Size="12px" ForeColor="#94A3B8" Style="position: absolute; left: 0px; top: 150px; width: 100%;"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlStatBox2" runat="server" BackColor="#0F172A" Style="position: absolute; left: 275px; top: 70px; width: 235px; height: 210px; border-radius: 8px; text-align: center;">
                <asp:Label ID="lblStatBox2Icon" runat="server" Text="[Data]" Font-Size="24px" ForeColor="#60A5FA" Style="position: absolute; left: 0px; top: 40px; width: 100%;"></asp:Label>
                <asp:Label ID="lblStatBox2Val" runat="server" Text="9 Woredas" Font-Bold="true" ForeColor="White" Font-Size="32px" Style="position: absolute; left: 0px; top: 100px; width: 100%;"></asp:Label>
                <asp:Label ID="lblStatBox2Desc" runat="server" Text="ADMIN UNITS" Font-Size="12px" ForeColor="#94A3B8" Style="position: absolute; left: 0px; top: 150px; width: 100%;"></asp:Label>
            </asp:Panel>

        </asp:Panel>

    </asp:Panel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server"></asp:Content>
