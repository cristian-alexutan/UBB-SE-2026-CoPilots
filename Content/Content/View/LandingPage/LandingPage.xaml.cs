using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Content.ViewModel;
using Content.User;
using Content.Service;
using Content;
using Content.Helper;


namespace Content
{
    public sealed partial class LandingPage : Window
    {
        private readonly MainService _service;
        private readonly UserSession _session;
        public LandingViewModel ViewModel { get; }
        public LandingPage(MainService service, UserSession session)
        {
            this.InitializeComponent();

            _service = service;
            _session = session;


            ViewModel = new LandingViewModel(service, session);

            ClientButton.Click += ClientButton_Click;
            AdminButton.Click += AdminButton_Click;
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            var client = _service.clientService.GetAnyClient();
            if (client == null)
            {
                ErrorText.Text = "No client found in database.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }
            if (ViewModel.SelectClientCommand.CanExecute(null))
            {
                ViewModel.SelectClientCommand.Execute(null);
                OpenShop();
            }
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            var admin = _service.clientService.GetAnyClient();
            if (admin == null)
            {
                ErrorText.Text = "No admin found in database.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }
            if (ViewModel.SelectAdminCommand.CanExecute(null))
            {
                ViewModel.SelectAdminCommand.Execute(null);
                OpenShop();
            }
        }

        private void OpenShop()
        {
            var shopPage = new ShopPage(_service, _session);
            shopPage.Activate();
            this.Close();
        }

    }
}
