using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Content.Domain;
using Content.Helper;
using Content.Service;
using Content.User;
using Content.ViewModel.Interface;

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
        private Cart cart;

        private int quantity = MinimumQuantity;
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
                var clamped = Math.Max(MinimumQuantity, Math.Min(MaximumQuantity, value));
                if (quantity == clamped)
                {
                    return;
                }

                quantity = clamped;
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

            AddToCartCommand = new RelayCommand(AddToCart);
            IncrementQuantityCommand = new RelayCommand(IncrementQuantity);
            DecrementQuantityCommand = new RelayCommand(DecrementQuantity);
            SaveChangesCommand = new RelayCommand(SaveChanges);
        }

        public void SetQuantityFromText(string text)
        {
            if (int.TryParse(text, out var parsed))
            {
                Quantity = parsed;
            }
            else
            {
                OnPropertyChanged(nameof(Quantity));
            }
        }

        private void IncrementQuantity()
        {
            Quantity++;
        }

        private void DecrementQuantity()
        {
            Quantity--;
        }

        private void AddToCart()
        {
            if (quantity > item.Quantity)
            {
                ErrorOccurred?.Invoke(this, $"You requested {quantity} item(s), but only {item.Quantity} are available.");
                return;
            }

            EnsureCartExists();

            try
            {
                cartService.AddItemToCart(cart.Id, new CartItem(0, item, quantity));
            }
            catch (InvalidOperationException ex)
            {
                ErrorOccurred?.Invoke(this, ex.Message);
                return;
            }

            AddedToCartSuccessfully?.Invoke(this, EventArgs.Empty);
        }

        private void EnsureCartExists()
        {
            var existingCart = cartService.GetCartById(session.UserId);
            if (existingCart != null)
            {
                cart = existingCart;
                return;
            }

            var newCart = new Cart(
                session.UserId,
                new Client(session.UserId, "Current Client"),
                new Dictionary<int, CartItem>());

            cartService.AddCart(newCart);
            cart = newCart;
        }

        private void SaveChanges()
        {
            var name = (editName ?? string.Empty).Trim();
            var description = editDescription ?? string.Empty;
            var priceText = (editPrice ?? string.Empty).Trim();

            if (!int.TryParse(editStock, out var parsedStock) || parsedStock < 0)
            {
                StatusMessage = "Error: Stock must be a non-negative number.";
                return;
            }

            if (!float.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedPrice) || parsedPrice <= 0)
            {
                StatusMessage = "Error: Price must be a positive number.";
                return;
            }

            item.Name = name;
            item.Description = description;
            item.Price = parsedPrice;
            item.Quantity = parsedStock;

            shopItemService.UpdateShopItem(
                new ShopItem(item.Id, item.Quantity, item.Price, item.ShopId, item.Photo, item.Name, item.Description));

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(FormattedPrice));
            OnPropertyChanged(nameof(Stock));

            StatusMessage = "Saved";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}