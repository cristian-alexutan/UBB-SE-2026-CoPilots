using System;
using System.Configuration;
using Microsoft.UI.Xaml;
using Content.Repository.Database;
using Content.Repository;
using Content.Domain;
using Content.Service; 
using Content.User;
namespace Content
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            string conn = ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            var service = new MainService(conn);
            var session = new UserSession();
            var landingWindow = new LandingPage(service, session);
            landingWindow.Activate();
        }

        private Window? m_window;
    }
}