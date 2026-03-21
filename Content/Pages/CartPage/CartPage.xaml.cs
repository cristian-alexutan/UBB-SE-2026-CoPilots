using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Content // Make sure this matches your project name
{
    public sealed partial class CartPage : Window
    {
        public CartViewModel ViewModel { get; }

        public CartPage()
        {
            ViewModel = new CartViewModel();
            this.InitializeComponent();
        }

        // 1. These methods catch the button clicks from the UI
        private async void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var ShopItem = button.DataContext as CartShopItem;

            if (ShopItem != null)
            {
                if (ShopItem.Quantity > 1)
                {
                    // Just lower the quantity if it's 2 or more
                    ViewModel.ChangeQuantity(ShopItem, ShopItem.Quantity - 1);
                }
                else if (ShopItem.Quantity == 1)
                {
                    // Show the popup if it's exactly 1
                    await ShowDeleteConfirmationAsync(ShopItem);
                }
            }
        }
        private async System.Threading.Tasks.Task ShowDeleteConfirmationAsync(CartShopItem ShopItem)
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Delete ShopItem",
                Content = "Are you sure you want to delete this ShopItem?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                // XamlRoot is absolutely required in WinUI 3 Desktop apps to show a dialog
                XamlRoot = this.Content.XamlRoot
            };

            // Wait for the user to click a button
            ContentDialogResult result = await deleteDialog.ShowAsync();

            // If they clicked the primary button ("Delete")
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
                // In your final app, you will check the database stock here first
                ViewModel.ChangeQuantity(ShopItem, ShopItem.Quantity + 1);
            }
        }
        private async void EmptyCart_Click(object sender, RoutedEventArgs e)
        {
            // Don't show the popup if the cart is already empty
            if (ViewModel.CartShopItems.Count == 0) return;

            ContentDialog emptyDialog = new ContentDialog
            {
                
                Title = "Empty Cart",
                Content = "Are you sure you want to remove all ShopItems from your cart?",
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
            // Only allow reservation if there are actually ShopItems in the cart
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
                // In the future, your navigation logic to switch views goes here.
                // For now, it just closes the dialog.
            }
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // The requirements state this should be clickable but do nothing.
            // Leaving this empty achieves exactly that.
        }
    }


    public class CartViewModel : INotifyPropertyChanged
    {
        private bool _isReserved;
        public bool IsReserved
        {
            get => _isReserved;
            set
            {
                _isReserved = value;
                OnPropertyChanged();
                // Tell the UI that these specific properties also need to be re-checked
                OnPropertyChanged(nameof(IsReserveButtonEnabled));
                OnPropertyChanged(nameof(CancelButtonVisibility));
            }
        }

        // Controls if the Reserve button is clickable
        public bool IsReserveButtonEnabled => !IsReserved;

        // Controls if the Cancel button is visible on the screen
        public Visibility CancelButtonVisibility => IsReserved ? Visibility.Visible : Visibility.Collapsed;

       
        public void ReserveCart()
        {
            IsReserved = true;
        }

        public void CancelReservation()
        {
            IsReserved = false;
        }
        public ObservableCollection<CartShopItem> CartShopItems { get; set; }

        private double _overallTotal;
        public string OverallTotal => $"${_overallTotal:0.00}"; // Formats the number as currency

        public CartViewModel()
        {
            CartShopItems = new ObservableCollection<CartShopItem>
            {
                
                new CartShopItem { ShopItem = new ShopItem { Name = "Luxury Perfume", Price = 89.99 }, Quantity = 2 },
                new CartShopItem { ShopItem = new ShopItem { Name = "Premium Chocolate Box", Price = 45.50 }, Quantity = 1 },
                new CartShopItem { ShopItem = new ShopItem { Name = "Irish Whiskey (1L)", Price = 65.00 }, Quantity = 1 },
                new CartShopItem { ShopItem = new ShopItem { Name = "Designer Sunglasses", Price = 120.00 }, Quantity = 1 },
                new CartShopItem { ShopItem = new ShopItem { Name = "Pufuleti Snacks", Price = 5.00 }, Quantity = 4 }
            };

            CalculateOverallTotal();
        }

        // 2. The ViewModel handles the math logic
        public void ChangeQuantity(CartShopItem ShopItem, int newQuantity)
        {
            ShopItem.Quantity = newQuantity;
            CalculateOverallTotal();
        }
        public void RemoveShopItem(CartShopItem ShopItem)
        {
            CartShopItems.Remove(ShopItem);
            CalculateOverallTotal();
        }
        public void EmptyCart()
        {
            CartShopItems.Clear();
            CalculateOverallTotal(); // This will recalculate the sum (which is now 0) and update the UI
        }

        private void CalculateOverallTotal()
        {
            // Updated to i.ShopItem.Price
            _overallTotal = CartShopItems.Sum(i => i.Quantity * i.ShopItem.Price);
            OnPropertyChanged(nameof(OverallTotal));
        }

        // Standard MVVM boilerplate to notify the UI
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CartShopItem : INotifyPropertyChanged
    {
      
        public ShopItem ShopItem { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShopItemTotalPrice));
            }
        }

        
        public string ShopItemTotalPrice => $"${(Quantity * ShopItem.Price):0.00}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

   
    public class ShopItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string DisplayPrice => $"${Price:0.00}";
    }
}