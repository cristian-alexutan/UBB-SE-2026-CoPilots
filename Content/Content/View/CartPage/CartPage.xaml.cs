using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Content.ViewModel;
using Content.ViewModel.Interface;

namespace Content
{
    public sealed partial class CartPage : Page
    {
        public Visibility BoolToVisibility(bool value) => value ? Visibility.Visible : Visibility.Collapsed;

        public ICartViewModel ViewModel { get; }

        public CartPage()
        {
            this.ViewModel = new CartViewModel(App.CartService, App.ReservationService, App.Session);
            this.InitializeComponent();
        }

        private async void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var shopItem = button.DataContext as CartShopItem;

            if (shopItem != null)
            {
                if (this.ViewModel.IsLastItem(shopItem))
                {
                    await this.ShowDeleteConfirmationAsync(shopItem);
                }
                else
                {
                    this.ViewModel.DecreaseQuantity(shopItem);
                }
            }
        }

        private async Task ShowDeleteConfirmationAsync(CartShopItem shopItem)
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Delete Item",
                Content = "Are you sure you want to delete this item?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot,
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                this.ViewModel.RemoveShopItem(shopItem);
            }
        }

        private async void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var shopItem = button.DataContext as CartShopItem;

            if (shopItem != null)
            {
                try
                {
                    this.ViewModel.ChangeQuantity(shopItem, shopItem.Quantity + 1);
                }
                catch (InvalidOperationException ex)
                {
                    await this.ShowErrorDialogAsync("Cannot increase quantity", ex.Message);
                }
            }
        }

        private async void EmptyCart_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.CartShopItems.Count == 0)
            {
                return;
            }

            ContentDialog emptyDialog = new ContentDialog
            {
                Title = "Empty Cart",
                Content = "Are you sure you want to remove all items from your cart?",
                PrimaryButtonText = "Empty Cart",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot,
            };

            ContentDialogResult result = await emptyDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                this.ViewModel.EmptyCart();
            }
        }

        private async Task ShowErrorDialogAsync(string title, string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot,
            };

            await errorDialog.ShowAsync();
        }

        private async void Reserve_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.CartShopItems.Count > 0)
            {
                try
                {
                    this.ViewModel.ReserveCart();
                }
                catch (Exception ex)
                {
                    await this.ShowErrorDialogAsync("Reservation Error", ex.Message);
                }
            }
        }

        private async void CancelReservation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ViewModel.CancelReservation();
                this.ViewModel.Reload();
            }
            catch (Exception ex)
            {
                await this.ShowErrorDialogAsync("Cancellation Error", ex.Message);
            }
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
                XamlRoot = this.XamlRoot,
            };

            ContentDialogResult result = await backDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                this.Frame.Navigate(typeof(ShopPage));
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LandingPage));
        }
    }
}
