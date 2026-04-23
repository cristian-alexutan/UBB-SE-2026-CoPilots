using System;
using Content.Domain;
using Content.ViewModel.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Content
{
    public sealed partial class ShopPage : Page
    {
        public IShopPageViewModel ViewModel { get; }

        public ShopPage()
        {
            this.InitializeComponent();

            ViewModel = App.Services.GetRequiredService<IShopPageViewModel>();
            ShopsGridView.ItemsSource = ViewModel.Shops;

            AddShopButton.Visibility = ViewModel.CanAddShop ? Visibility.Visible : Visibility.Collapsed;
            AddShopButton.Click += AddShopButton_Click;

            CartButton.IsEnabled = ViewModel.IsCartEnabled;
            CartButton.Opacity = ViewModel.CartOpacity;
            CartButton.Click += CartButton_Click;

            ShopsGridView.ItemClick += ShopsGridView_ItemClick;
            ProfileButton.Click += ProfileButton_Click;

            SortComboBox.SelectedIndex = 0;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LandingPage));
        }

        private void ShopsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(ShopItemsPage), (Shop)e.ClickedItem);
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CartPage));
        }

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
                try
                {
                    ViewModel.AddShop(nameBox.Text, typeBox.Text);
                }
                catch (Exception ex)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = ex.Message,
                        CloseButtonText = "OK",
                        RequestedTheme = ElementTheme.Light,
                        XamlRoot = button.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void EditShopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsAdmin)
            {
                return;
            }

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
                    try
                    {
                        ViewModel.EditShop(shop, nameBox.Text, typeBox.Text);
                    }
                    catch (Exception ex)
                    {
                        var errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = ex.Message,
                            CloseButtonText = "OK",
                            RequestedTheme = ElementTheme.Light,
                            XamlRoot = btn.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }
            }
        }

        private async void DeleteShopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsAdmin)
            {
                return;
            }

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

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.Search(sender.Text);
        }

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
