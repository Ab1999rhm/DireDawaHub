using System;
using System.Collections.Generic;
using System.Web.UI;

namespace DireDawaHubWebForms
{
    // Robust Mock Data Models
    public class WaterSchedule { public string Location { get; set; } public string Status { get; set; } }
    public class Job { public string Title { get; set; } public string Company { get; set; } }
    public class Poster { public string ImagePath { get; set; } public string Title { get; set; } }
    public class WeatherData { public string Temperature { get; set; } public string Condition { get; set; } public string Icon { get; set; } }

    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            // Populate robust dummy data for attractive UI
            var waterSchedules = new List<WaterSchedule> { 
                new WaterSchedule { Location = "Kebele 02 Pipeline", Status = "Flowing Normally" },
                new WaterSchedule { Location = "Industrial Zone Line", Status = "Flowing Normally" },
                new WaterSchedule { Location = "Dechatu River Pump", Status = "Maintenance Mode" }
            };
            
            var jobs = new List<Job> { 
                new Job { Title = "Senior Software Engineer", Company = "Ethio Telecom" },
                new Job { Title = "Site Supervisor", Company = "Dire Dawa Construction Ltd." },
                new Job { Title = "Bank Teller", Company = "Awash Bank Dire Dawa" },
                new Job { Title = "High School Teacher", Company = "Sabian Secondary School" }
            };
            
            var posters = new List<Poster> { 
                new Poster { ImagePath = "https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?auto=format&fit=crop&q=80&w=800", Title = "Tech In Dire Dawa Summit" },
                new Poster { ImagePath = "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?auto=format&fit=crop&q=80&w=800", Title = "Morning Community Yoga" }
            };
            
            var weather = new WeatherData { Temperature = "32", Condition = "Sunny", Icon = "Clear Sky" };

            // Bind Labels
            lblWaterCount.Text = waterSchedules.Count.ToString();
            lblClinicCount.Text = "12";
            lblJobCount.Text = jobs.Count.ToString();
            lblMarketCount.Text = "8";

            lblWeatherTemp.Text = weather.Temperature + "°C";
            lblWeatherCondition.Text = weather.Condition;
            lblWeatherIcon.Text = weather.Icon;

            // Bind Repeaters
            // rptWater.DataSource = waterSchedules; rptWater.DataBind();
            // rptJobs.DataSource = jobs; rptJobs.DataBind();
            // rptPosters.DataSource = posters; rptPosters.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e) 
        { 
            // Mock Search Logic
        }
    }
}
