using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Content.ViewModel;
using Microsoft.UI.Xaml.Media;
using Content.Domain;
using Content.Service;
using Content.User;
using System;
using System.Linq;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Content
{
    public sealed partial class ShopPage : Window
    {
        public ShopPageViewModel ViewModel { get; }
        private readonly MainService _service;
        private readonly UserSession _session;

        public ShopPage(MainService Service, UserSession Session)
        {
            this.InitializeComponent();
            this._service = Service;
            // Note: Managers are the ones who have admin privileges 
            this._session = Session;

            ViewModel = new ShopPageViewModel(Service, Session);

            // This binds the list of shops to the grid view
            ShopsGridView.ItemsSource = ViewModel.Shops;

            // Add Shop button is only visible to admins
            AddShopButton.Visibility = ViewModel.AddShopVisibility;
            AddShopButton.Click += AddShopButton_Click;

            // Cart is only enabled for regular clients, admins cannot access the cart (it's greyed out)
            CartButton.IsEnabled = ViewModel.IsCartEnabled;
            CartButton.Opacity = ViewModel.CartOpacity;
            CartButton.Click += CartButton_Click;

            ShopsGridView.ItemClick += ShopsGridView_ItemClick;
            ProfileButton.Click += ProfileButton_Click;
        }

        // Profile button (next to cart) sends you back to the landing page 
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var landingPage = new LandingPage(_service, _session);
            landingPage.Activate();
            this.Close();
        }

        // Navigation to the ShopItems window
        private void ShopsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {

            var shopItemPage = new ShopItemsPage(_service, _session, ((Shop)e.ClickedItem));
            shopItemPage.Activate();
            this.Close();
        }

        // Navigation to the Cart window
        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            var cart = new CartPage(_service, _session);
            cart.Activate();
            this.Close();
        }

        // Opens a dialog box that lets you add the shop details and adds it via the ViewModel 
        private async void AddShopButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var nameBox = new TextBox { PlaceholderText = "Enter shop name" };
            var typeBox = new TextBox { PlaceholderText = "Enter type" };

            var dialog = new ContentDialog
            {
                Title = "Add Shop",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = button.XamlRoot,
                Content = new StackPanel
                {
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock { Text = "Shop Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                        nameBox,
                        new TextBlock { Text = "Type", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                        typeBox
                    }
                }
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModel.AddShop(nameBox.Text, typeBox.Text);
            }
        }

        // Opens a dialog box where you can edit the shop's current details
        private async void EditShopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsAdmin) return;

            if (sender is Button btn && btn.DataContext is Shop shop)
            {
                var nameBox = new TextBox { Text = shop.Name };
                var typeBox = new TextBox { Text = shop.Type };

                var dialog = new ContentDialog
                {
                    Title = "Edit Shop",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Save",
                    RequestedTheme = ElementTheme.Light,
                    XamlRoot = btn.XamlRoot,
                    Content = new StackPanel
                    {
                        Spacing = 15,
                        Children =
                        {
                            new TextBlock { Text = "Shop Name", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            nameBox,
                            new TextBlock { Text = "Type", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            typeBox
                        }
                    }
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.EditShop(shop, nameBox.Text, typeBox.Text);
                }
            }
        }

        // Opens a confirmation dialog box and deletes the shop based on user answer
        private async void DeleteShopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsAdmin) return;

            if (sender is Button btn && btn.DataContext is Shop shop)
            {
                var dialog = new ContentDialog
                {
                    Title = "Delete Shop",
                    Content = $"Are you sure you want to delete \"{shop.Name}\"?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "Cancel",
                    RequestedTheme = ElementTheme.Light,
                    XamlRoot = btn.XamlRoot
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.DeleteShop(shop);
                }
            }
        }

        // Returns matching results (doesn't update the result list in real time, you need to press enter)
        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.Search(sender.Text);
        }

        // Sorts the shops based on choice. Initially there is no sorting. 
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem item)
            {
                string selected = item.Content.ToString();

                if (selected == "None")
                {
                    ViewModel.LoadItems();

                }
                else if (selected == "Shop Name")
                {
                    ViewModel.SortAlphabetically();
                }
                else if (selected == "Reviews")
                {
                    ViewModel.SortByReviews();
                }

            }
        }

    }
}