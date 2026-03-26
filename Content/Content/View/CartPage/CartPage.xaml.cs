using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Content.Service;
using Content.User;
using Content.ViewModel; // Added to access the new CartViewModel

namespace Content
{
    public sealed partial class CartPage : Window
    {
        public CartViewModel ViewModel { get; }

        // 1. Changed constructor to accept service and session
        private readonly MainService _service;
        private readonly UserSession _session;
        public CartPage(MainService service, UserSession session)
        {
            // 2. Added safety check to kick out admins if they somehow get here
            _service = service;
            _session = session;

            if (session.IsAdmin)
            {
                throw new UnauthorizedAccessException("Admins are not allowed to view or enter the Cart.");
            }

            ViewModel = new CartViewModel(service, session);
            this.InitializeComponent();

            ViewModel.Reload();

        }

        private async void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var ShopItem = button.DataContext as CartShopItem;

            if (ShopItem != null)
            {
                if (ShopItem.Quantity > 1)
                {
                    ViewModel.ChangeQuantity(ShopItem, ShopItem.Quantity - 1);
                }
                else if (ShopItem.Quantity == 1)
                {
                    await ShowDeleteConfirmationAsync(ShopItem);
                }
            }
        }

        private async Task ShowDeleteConfirmationAsync(CartShopItem ShopItem)
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Delete Item",
                Content = "Are you sure you want to delete this item?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.RemoveShopItem(ShopItem);
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var ShopItem = button.DataContext as CartShopItem;

            if (ShopItem != null)
            {
                ViewModel.ChangeQuantity(ShopItem, ShopItem.Quantity + 1);
            }
        }

        private async void EmptyCart_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CartShopItems.Count == 0) return;

            ContentDialog emptyDialog = new ContentDialog
            {
                Title = "Empty Cart",
                Content = "Are you sure you want to remove all items from your cart?",
                PrimaryButtonText = "Empty Cart",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            ContentDialogResult result = await emptyDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.EmptyCart();
            }
        }

        private void Reserve_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CartShopItems.Count > 0)
            {
                ViewModel.ReserveCart();
            }
        }

        private void CancelReservation_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelReservation();
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog backDialog = new ContentDialog
            {
                Title = "Leave Cart",
                Content = "Are you sure you want to go back to the shop page?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            ContentDialogResult result = await backDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Navigate back logic goes here
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the landing page passing the stored service and session
            var landingPage = new LandingPage(_service, _session);

            // Show the landing page
            landingPage.Activate();


            this.Close();
        }
    }


}