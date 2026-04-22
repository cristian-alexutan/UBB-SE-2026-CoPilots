using Content.Data.Service.Interface;
using Content.Helper;
using Content.Service;
using Content.User;
using Content.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Content
{
    public sealed partial class LandingPage : Window
    {
        private readonly MainService service;
        private readonly UserSession session;
        public LandingViewModel ViewModel { get; }
        public LandingPage(MainService service, UserSession session)
        {
            this.InitializeComponent();

            this.service = service;
            this.session = session;

            ViewModel = new LandingViewModel(service, session);

            ClientButton.Click += ClientButton_Click;
            AdminButton.Click += AdminButton_Click;
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectClientCommand.Execute(null);
            if (ViewModel.IsRoleSelected)
            {
                OpenShop();
            }
            else
            {
                ErrorText.Text = ViewModel.ErrorMessage;
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectAdminCommand.Execute(null);
            if (ViewModel.IsRoleSelected)
            {
                OpenShop();
            }
            else
            {
                ErrorText.Text = ViewModel.ErrorMessage;
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void OpenShop()
        {
            var shopPage = new ShopPage(service, session);
            shopPage.Activate();
            this.Close();
        }
    }
}
