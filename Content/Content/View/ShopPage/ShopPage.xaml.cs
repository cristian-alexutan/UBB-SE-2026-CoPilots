using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Content.ViewModel;
using Microsoft.UI.Xaml.Media;
using Content.Domain;
using Content.Service;
using Content.User;
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
            this._session = Session;

            ViewModel = new ShopPageViewModel(Service, Session);

            ShopsGridView.ItemsSource = ViewModel.Shops;

            AddShopButton.Visibility = ViewModel.AddShopVisibility;
            AddShopButton.Click += AddShopButton_Click;

            CartButton.IsEnabled = ViewModel.IsCartEnabled;
            CartButton.Opacity = ViewModel.CartOpacity;
            CartButton.Click += CartButton_Click;

            ShopsGridView.ItemClick += ShopsGridView_ItemClick;
            ShopsGridView.Loaded += ShopsGridView_Loaded;
        }

        private void ShopsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsAdmin)
                HideAdminButtons();
        }

        private void HideAdminButtons()
        {
            foreach (var item in ViewModel.Shops)
            {
                var container = ShopsGridView.ContainerFromItem(item) as GridViewItem;
                if (container == null) continue;

                var editBtn = FindChildByName(container, "EditButton") as Button;
                var deleteBtn = FindChildByName(container, "DeleteButton") as Button;

                if (editBtn != null) editBtn.Visibility = Visibility.Collapsed;
                if (deleteBtn != null) deleteBtn.Visibility = Visibility.Collapsed;
            }
        }

        private DependencyObject FindChildByName(DependencyObject parent, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement fe && fe.Name == name)
                    return child;
                var result = FindChildByName(child, name);
                if (result != null) return result;
            }
            return null;
        }

        private void ShopsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {

            if (e.ClickedItem is Shop shop)
            {
                var shopItemPage = new ShopItemsPage(_service, _session, shop);
                shopItemPage.Activate();
                this.Close();
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectClientCommand.Execute(null);
            var cart = new CartPage(_service, _session);
            cart.Activate();
            this.Close();
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
                ViewModel.AddShop(nameBox.Text, typeBox.Text);
                ShopsGridView.ItemsSource = null;
                ShopsGridView.ItemsSource = ViewModel.Shops;
            }
        }

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
                    ShopsGridView.ItemsSource = null;
                    ShopsGridView.ItemsSource = ViewModel.Shops;
                }
            }
        }

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
    }
}