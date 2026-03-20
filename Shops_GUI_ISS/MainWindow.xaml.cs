using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;

namespace Shops_GUI_ISS
{
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<ShopItem> Shops { get; set; } = new ObservableCollection<ShopItem>();

        public MainWindow()
        {
            this.InitializeComponent();
            Shops.Add(new ShopItem { Name = "Chocolate Heaven", Category = "Food" });
            Shops.Add(new ShopItem { Name = "Cosmetics Corner", Category = "Beauty" });
            Shops.Add(new ShopItem { Name = "Designer Bags", Category = "Fashion" });
            Shops.Add(new ShopItem { Name = "Gourmet Delights", Category = "Food" });
            Shops.Add(new ShopItem { Name = "Luxury Boutique", Category = "Fashion" });
            ShopsGridView.ItemsSource = Shops;

            AddShopButton.Click += AddShopButton_Click;
        }

        private async void AddShopButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var nameBox = new TextBox { PlaceholderText = "Enter shop name" };
            var categoryBox = new TextBox { PlaceholderText = "Enter category" };

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
                        new TextBlock { Text = "Category", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                        categoryBox
                    }
                }
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (!string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    Shops.Add(new ShopItem
                    {
                        Name = nameBox.Text,
                        Category = categoryBox.Text
                    });
                }
            }
        }

        private async void EditShopButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ShopItem shop)
            {
                var nameBox = new TextBox { Text = shop.Name };
                var categoryBox = new TextBox { Text = shop.Category };

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
                            new TextBlock { Text = "Category", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                            categoryBox
                        }
                    }
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    shop.Name = nameBox.Text;
                    shop.Category = categoryBox.Text;

                    ShopsGridView.ItemsSource = null;
                    ShopsGridView.ItemsSource = Shops;
                }
            }
        }

        private async void DeleteShopButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ShopItem shop)
            {
                var dialog = new ContentDialog
                {
                    Title = "Delete Shop",
                    Content = $"Are you sure you want to delete \"{shop.Name}\"?",
                    PrimaryButtonText = "Yes",
                    RequestedTheme = ElementTheme.Light,
                    CloseButtonText = "Cancel",
                    XamlRoot = btn.XamlRoot
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    Shops.Remove(shop);
                }
            }
        }
    }
    public class ShopItem
    {
        public string Name { get; set; }
        public string Category { get; set; }
    }
}