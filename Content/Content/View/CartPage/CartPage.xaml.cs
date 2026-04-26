using System;
using System.Threading.Tasks;
using Content.ViewModel;
using Content.ViewModel.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Content
{
    public sealed partial class CartPage : Page
    {
        public ICartViewModel ViewModel { get; }

        public CartPage()
        {
            this.ViewModel = App.Services.GetRequiredService<ICartViewModel>();
            this.InitializeComponent();
        }

        public Visibility BoolToVisibility(bool value)
        {
            if (value)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        private async void DecreaseQuantity_Click(object sender, RoutedEventArgs eventArgs)
        {
            var clickedButton = sender as Button;
            var cartShopItem = clickedButton.DataContext as CartShopItem;

            if (cartShopItem != null)
            {
                if (this.ViewModel.IsLastItem(cartShopItem))
                {
                    await this.ShowDeleteConfirmationAsync(cartShopItem);
                }
                else
                {
                    this.ViewModel.DecreaseQuantity(cartShopItem);
                }
            }
        }

        private async Task ShowDeleteConfirmationAsync(CartShopItem cartShopItem)
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

            ContentDialogResult dialogResult = await deleteDialog.ShowAsync();

            if (dialogResult == ContentDialogResult.Primary)
            {
                this.ViewModel.RemoveShopItem(cartShopItem);
            }
        }

        private async void IncreaseQuantity_Click(object sender, RoutedEventArgs eventArgs)
        {
            var clickedButton = sender as Button;
            var cartShopItem = clickedButton.DataContext as CartShopItem;

            if (cartShopItem != null)
            {
                try
                {
                    this.ViewModel.ChangeQuantity(cartShopItem, cartShopItem.Quantity + 1);
                }
                catch (InvalidOperationException exception)
                {
                    await this.ShowErrorDialogAsync("Cannot increase quantity", exception.Message);
                }
            }
        }

        private async void EmptyCart_Click(object sender, RoutedEventArgs eventArgs)
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

            ContentDialogResult dialogResult = await emptyDialog.ShowAsync();

            if (dialogResult == ContentDialogResult.Primary)
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

        private async void Reserve_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.ViewModel.CartShopItems.Count > 0)
            {
                try
                {
                    this.ViewModel.ReserveCart();
                }
                catch (Exception exception)
                {
                    await this.ShowErrorDialogAsync("Reservation Error", exception.Message);
                }
            }
        }

        private async void CancelReservation_Click(object sender, RoutedEventArgs eventArgs)
        {
            try
            {
                this.ViewModel.CancelReservation();
                this.ViewModel.Reload();
            }
            catch (Exception exception)
            {
                await this.ShowErrorDialogAsync("Cancellation Error", exception.Message);
            }
        }

        private async void BackButton_Click(object sender, RoutedEventArgs eventArgs)
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

            ContentDialogResult dialogResult = await backDialog.ShowAsync();

            if (dialogResult == ContentDialogResult.Primary)
            {
                this.Frame.Navigate(typeof(ShopPage));
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            this.Frame.Navigate(typeof(LandingPage));
        }
    }
}
