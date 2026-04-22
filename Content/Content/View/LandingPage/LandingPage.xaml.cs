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
            var client = service.ClientService.GetAnyClient();
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
            var admin = service.ManagerService.GetAnyManager();
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
            var shopPage = new ShopPage(service, session);
            shopPage.Activate();
            this.Close();
        }
    }
}
