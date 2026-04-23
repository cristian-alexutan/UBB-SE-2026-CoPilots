using Content.ViewModel;
using Content.ViewModel.Interface;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Content
{
    public sealed partial class LandingPage : Page
    {
        public ILandingViewModel ViewModel { get; }

        public LandingPage()
        {
            this.InitializeComponent();

            ViewModel = new LandingViewModel(App.ClientService, App.ManagerService, App.Session);

            ClientButton.Click += ClientButton_Click;
            AdminButton.Click += AdminButton_Click;
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectClientCommand.Execute(null);
            if (ViewModel.IsRoleSelected)
            {
                this.Frame.Navigate(typeof(ShopPage));
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
                this.Frame.Navigate(typeof(ShopPage));
            }
            else
            {
                ErrorText.Text = ViewModel.ErrorMessage;
                ErrorText.Visibility = Visibility.Visible;
            }
        }
    }
}
