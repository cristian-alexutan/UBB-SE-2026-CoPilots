using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Content.Domain;
using Content.Data.Service.Interface;
using Content.User;
using Content.ViewModel.Interface;
using CommunityToolkit.Mvvm.Input;

namespace Content.ViewModel
{
    public class ItemDetailsViewModel : IItemDetailsViewModel
    {
        private const int MinimumQuantity = 1;
        private const int MaximumQuantity = 99;

        private readonly ICartService cartService;
        private readonly IShopItemService shopItemService;
        private readonly UserSession session;
        private readonly ShopItem item;

        private int quantity;
        private string editName;
        private string editDescription;
        private string editPrice;
        private string editStock;
        private string statusMessage = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler AddedToCartSuccessfully;
        public event EventHandler<string> ErrorOccurred;

        public string Name
        {
            get { return item.Name; }
        }

        public string Description
        {
            get { return item.Description; }
        }

        public string FormattedPrice
        {
            get { return $"{item.Price:C}"; }
        }

        public int Stock
        {
            get { return item.Quantity; }
        }

        public string Photo
        {
            get { return item.Photo; }
        }

        public bool IsAdmin
        {
            get { return session.IsAdmin; }
        }

        public Shop CurrentShop { get; }

        public int Quantity
        {
            get
            {
                return quantity;
            }

            private set
            {
                if (quantity == value)
                {
                    return;
                }

                quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public string EditName
        {
            get
            {
                return editName;
            }

            set
            {
                editName = value;
                OnPropertyChanged(nameof(EditName));
            }
        }

        public string EditDescription
        {
            get
            {
                return editDescription;
            }

            set
            {
                editDescription = value;
                OnPropertyChanged(nameof(EditDescription));
            }
        }

        public string EditPrice
        {
            get
            {
                return editPrice;
            }

            set
            {
                editPrice = value;
                OnPropertyChanged(nameof(EditPrice));
            }
        }

        public string EditStock
        {
            get
            {
                return editStock;
            }

            set
            {
                editStock = value;
                OnPropertyChanged(nameof(EditStock));
            }
        }

        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }

            private set
            {
                statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public ICommand AddToCartCommand { get; }
        public ICommand IncrementQuantityCommand { get; }
        public ICommand DecrementQuantityCommand { get; }
        public ICommand SaveChangesCommand { get; }

        public ItemDetailsViewModel(
            ICartService cartService,
            IShopItemService shopItemService,
            UserSession session,
            ShopItem item,
            Shop currentShop)
        {
            this.cartService = cartService;
            this.shopItemService = shopItemService;
            this.session = session;
            this.item = item;
            this.CurrentShop = currentShop;

            editName = item.Name;
            editDescription = item.Description;
            editPrice = item.Price.ToString(CultureInfo.InvariantCulture);
            editStock = item.Quantity.ToString(CultureInfo.InvariantCulture);
            quantity = MinimumQuantity;

            AddToCartCommand = new RelayCommand(AddToCart);
            IncrementQuantityCommand = new RelayCommand(IncrementQuantity);
            DecrementQuantityCommand = new RelayCommand(DecrementQuantity);
            SaveChangesCommand = new RelayCommand(SaveChanges);
        }

        public void SetQuantityFromText(string text)
        {
            if (int.TryParse(text, out int parsedQuantity))
            {
                Quantity = this.LimitQuantityToValidRange(parsedQuantity);
            }
            else
            {
                OnPropertyChanged(nameof(Quantity));
            }
        }

        private void IncrementQuantity()
        {
            Quantity = this.LimitQuantityToValidRange(Quantity + 1);
        }

        private void DecrementQuantity()
        {
            Quantity = this.LimitQuantityToValidRange(Quantity - 1);
        }

        private void AddToCart()
        {
            Cart cart = this.cartService.GetOrCreateCart(session.UserId);

            try
            {
                cartService.AddItemToCart(cart.Id, new CartItem(0, item, quantity));
            }
            catch (InvalidOperationException exception)
            {
                ErrorOccurred?.Invoke(this, exception.Message);
                return;
            }

            AddedToCartSuccessfully?.Invoke(this, EventArgs.Empty);
        }

        private void SaveChanges()
        {
            string trimmedName = (editName ?? string.Empty).Trim();
            string trimmedDescription = editDescription ?? string.Empty;
            string trimmedPriceText = (editPrice ?? string.Empty).Trim();

            if (!int.TryParse(editStock, out int parsedStock))
            {
                StatusMessage = "Error: Stock must be a number.";
                return;
            }

            if (!float.TryParse(trimmedPriceText, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedPrice))
            {
                StatusMessage = "Error: Price must be a number.";
                return;
            }

            item.Name = trimmedName;
            item.Description = trimmedDescription;
            item.Price = parsedPrice;
            item.Quantity = parsedStock;

            try
            {
                shopItemService.UpdateShopItem(
                    new ShopItem(item.Id, item.Quantity, item.Price, item.Shop, item.Photo, item.Name, item.Description));

                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(FormattedPrice));
                OnPropertyChanged(nameof(Stock));

                StatusMessage = "Saved";
            }
            catch (ArgumentException exception)
            {
                StatusMessage = exception.Message;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int LimitQuantityToValidRange(int quantity)
        {
            return Math.Max(MinimumQuantity, Math.Min(MaximumQuantity, quantity));
        }
    }
}