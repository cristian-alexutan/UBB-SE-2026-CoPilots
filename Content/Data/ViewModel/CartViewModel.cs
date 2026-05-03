using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.User;
using Content.ViewModel.Interface;

namespace Content.ViewModel
{
    public partial class CartViewModel : INotifyPropertyChanged, ICartViewModel
    {
        private readonly ICartService cartService;
        private readonly IReservationService reservationService;
        private readonly UserSession session;
        private Reservation currentReservation;

        private bool isReserved;
        private double overallTotal;

        public ObservableCollection<CartShopItem> CartShopItems { get; set; }

        public bool IsAdmin
        {
            get { return this.session.IsAdmin; }
        }

        public bool IsReserved
        {
            get
            {
                return this.isReserved;
            }

            set
            {
                this.isReserved = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.IsReserveButtonEnabled));
                this.OnPropertyChanged(nameof(this.IsCancelButtonVisible));
            }
        }

        public bool IsReserveButtonEnabled
        {
            get { return !this.IsReserved; }
        }

        public bool IsCancelButtonVisible
        {
            get { return this.IsReserved; }
        }

        public string OverallTotal
        {
            get { return string.Format("${0:0.00}", this.overallTotal); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CartViewModel(ICartService cartService, IReservationService reservationService, UserSession session)
        {
            if (session.IsAdmin)
            {
                throw new UnauthorizedAccessException("Admins are not allowed to view or enter the Cart.");
            }

            this.cartService = cartService;
            this.reservationService = reservationService;
            this.session = session;
            this.CartShopItems = new ObservableCollection<CartShopItem>();
            this.LoadCartItems();
            this.CheckExistingReservation();
        }

        public void Reload()
        {
            this.LoadCartItems();
        }

        public void ChangeQuantity(CartShopItem cartShopItem, int newQuantity)
        {
            this.cartService.UpdateItemQuantity(this.session.UserId, cartShopItem.CartItemId, newQuantity);
            cartShopItem.Quantity = newQuantity;
            this.overallTotal = this.cartService.GetCartTotal(this.session.UserId);
            this.OnPropertyChanged(nameof(this.OverallTotal));
        }

        [RelayCommand]
        public void RemoveShopItem(CartShopItem cartShopItem)
        {
            this.cartService.RemoveItemFromCart(this.session.UserId, cartShopItem.CartItemId);
            this.CartShopItems.Remove(cartShopItem);
            this.overallTotal = this.cartService.GetCartTotal(this.session.UserId);
            this.OnPropertyChanged(nameof(this.OverallTotal));
        }

        [RelayCommand]
        public void EmptyCart()
        {
            this.cartService.ClearCart(this.session.UserId);
            this.CartShopItems.Clear();
            this.overallTotal = this.cartService.GetCartTotal(this.session.UserId);
            this.OnPropertyChanged(nameof(this.OverallTotal));
        }

        [RelayCommand]
        public void DecreaseQuantity(CartShopItem cartShopItem)
        {
            this.cartService.DecreaseItemQuantity(this.session.UserId, cartShopItem.CartItemId);
            this.Reload();
        }

        [RelayCommand]
        public void ReserveCart()
        {
            var cart = this.cartService.GetCartById(this.session.UserId);
            var newReservation = new Reservation(cart, true, DateTime.Now);
            this.reservationService.ReserveCart(newReservation);
            this.currentReservation = newReservation;
            this.IsReserved = true;
        }

        [RelayCommand]
        public void CancelReservation()
        {
            this.reservationService.CancelReservation(this.currentReservation.Id);
            this.IsReserved = false;
            this.Reload();
        }

        public bool IsLastItem(CartShopItem cartShopItem)
        {
            return this.cartService.IsLastCartItem(this.session.UserId, cartShopItem.CartItemId);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CheckExistingReservation()
        {
            var cart = this.cartService.GetCartById(this.session.UserId);
            if (cart == null)
            {
                return;
            }

            var activeReservation = this.reservationService.GetActiveReservationForCart(cart.Id);
            if (activeReservation != null)
            {
                this.currentReservation = activeReservation;
                this.IsReserved = true;
            }
        }

        private void LoadCartItems()
        {
            this.CartShopItems.Clear();
            foreach (var existingCartItem in this.cartService.GetCartItems(this.session.UserId))
            {
                this.CartShopItems.Add(new CartShopItem
                {
                    CartItemId = existingCartItem.Id,
                    ShopItem = existingCartItem.ShopItem,
                    Quantity = existingCartItem.Quantity,
                });
            }

            this.overallTotal = this.cartService.GetCartTotal(this.session.UserId);
            this.OnPropertyChanged(nameof(this.OverallTotal));
        }
    }

    public partial class CartShopItem : INotifyPropertyChanged
    {
        private int quantity;
        public int CartItemId { get; set; }
        public ShopItem ShopItem { get; set; }

        public int Quantity
        {
            get => this.quantity;
            set
            {
                this.quantity = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.ItemTotalPrice));
            }
        }

        public string DisplayPrice
        {
            get
            {
                if (this.ShopItem != null)
                {
                    return string.Format("${0:0.00}", this.ShopItem.Price);
                }
                return "$0.00";
            }
        }

        public string ItemTotalPrice
        {
            get
            {
                if (this.ShopItem != null)
                {
                    return string.Format("${0:0.00}", this.Quantity * this.ShopItem.Price);
                }
                return "$0.00";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}