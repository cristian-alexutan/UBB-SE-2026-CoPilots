using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.User;
using Content.ViewModel.Interface;

namespace Content.ViewModel
{
    public class CartViewModel : INotifyPropertyChanged, ICartViewModel
    {
        private readonly ICartService cartService;
        private readonly IReservationService reservationService;
        private readonly UserSession session;
        private int currentReservationId;

        private bool isReserved;

        private double overallTotal;

        public ObservableCollection<CartShopItem> CartShopItems { get; set; }

        public bool IsAdmin
        {
            get
            {
                return this.session.IsAdmin;
            }
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
            get
            {
                return !this.IsReserved;
            }
        }

        public bool IsCancelButtonVisible
        {
            get
            {
                return this.IsReserved;
            }
        }

        public string OverallTotal
        {
            get
            {
                return string.Format("${0:0.00}", this.overallTotal);
            }
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

        public void RemoveShopItem(CartShopItem cartShopItem)
        {
            this.cartService.RemoveItemFromCart(this.session.UserId, cartShopItem.CartItemId);
            this.CartShopItems.Remove(cartShopItem);
            this.overallTotal = this.cartService.GetCartTotal(this.session.UserId);
            this.OnPropertyChanged(nameof(this.OverallTotal));
        }

        public void EmptyCart()
        {
            this.cartService.ClearCart(this.session.UserId);
            this.CartShopItems.Clear();
            this.overallTotal = this.cartService.GetCartTotal(this.session.UserId);
            this.OnPropertyChanged(nameof(this.OverallTotal));
        }

        public void DecreaseQuantity(CartShopItem cartShopItem)
        {
            this.cartService.DecreaseItemQuantity(this.session.UserId, cartShopItem.CartItemId);
            this.Reload();
        }

        public void ReserveCart()
        {
            var cart = this.cartService.GetCartById(this.session.UserId);
            var newReservation = new Reservation(cart, true, DateTime.Now);
            this.reservationService.ReserveCart(newReservation);
            this.currentReservationId = newReservation.Id;
            this.IsReserved = true;
        }

        public void CancelReservation()
        {
            this.reservationService.CancelReservation(this.currentReservationId);
            this.IsReserved = false;
        }

        public bool IsLastItem(CartShopItem cartShopItem)
        {
            return this.cartService.IsLastCartItem(this.session.UserId, cartShopItem.CartItemId);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void CheckExistingReservation()
        {
            var cart = this.cartService.GetCartById(this.session.UserId);
            if (cart == null)
            {
                return;
            }

            var allReservations = this.reservationService.GetAllReservations();
            Reservation activeReservation = null;
            foreach (var reservation in allReservations)
            {
                if (reservation.ReservationCart.Id == cart.Id && reservation.Active)
                {
                    activeReservation = reservation;
                    break;
                }
            }

            if (activeReservation != null)
            {
                this.currentReservationId = activeReservation.Id;
                this.IsReserved = true;
            }
        }

        private void LoadCartItems()
        {
            this.CartShopItems.Clear();
            var cart = this.cartService.GetCartById(this.session.UserId);

            if (cart != null && cart.CartItems != null)
            {
                foreach (var existingCartItem in cart.CartItems.Values)
                {
                    this.CartShopItems.Add(new CartShopItem
                    {
                        CartItemId = existingCartItem.Id,
                        ShopItem = existingCartItem.ShopItem,
                        Quantity = existingCartItem.Quantity,
                    });
                }
            }

            this.overallTotal = this.cartService.GetCartTotal(this.session.UserId);
            this.OnPropertyChanged(nameof(this.OverallTotal));
        }
    }

    public class CartShopItem : INotifyPropertyChanged
    {
        private int quantity;

        public int CartItemId { get; set; }

        public ShopItem ShopItem { get; set; }

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

        public int Quantity
        {
            get
            {
                return this.quantity;
            }

            set
            {
                this.quantity = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.ItemTotalPrice));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
