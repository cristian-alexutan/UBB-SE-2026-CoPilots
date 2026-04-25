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

        public bool IsAdmin => this.session.IsAdmin;

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

        public bool IsReserveButtonEnabled => !this.IsReserved;

        public bool IsCancelButtonVisible => this.IsReserved;

        public string OverallTotal => $"${this.overallTotal:0.00}";

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

        public void ChangeQuantity(CartShopItem item, int newQuantity)
        {
            this.cartService.UpdateItemQuantity(this.session.UserId, item.CartItemId, newQuantity);
            item.Quantity = newQuantity;
            this.overallTotal = this.cartService.GetCartTotal(this.session.UserId);
            this.OnPropertyChanged(nameof(this.OverallTotal));
        }

        public void RemoveShopItem(CartShopItem item)
        {
            this.cartService.RemoveItemFromCart(this.session.UserId, item.CartItemId);
            this.CartShopItems.Remove(item);
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
        public void DecreaseQuantity(CartShopItem item)
        {
            this.cartService.DecreaseItemQuantity(this.session.UserId, item.CartItemId);
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
        public bool IsLastItem(CartShopItem item)
        {
            return this.cartService.IsLastCartItem(this.session.UserId, item.CartItemId);
        }

        private void LoadCartItems()
        {
            this.CartShopItems.Clear();
            var cart = this.cartService.GetCartById(this.session.UserId);

            if (cart != null && cart.CartItems != null)
            {
                foreach (var dbCartItem in cart.CartItems.Values)
                {
                    this.CartShopItems.Add(new CartShopItem
                    {
                        CartItemId = dbCartItem.Id,
                        ShopItem = dbCartItem.ShopItem,
                        Quantity = dbCartItem.Quantity,
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

        public string DisplayPrice => this.ShopItem != null ? $"${this.ShopItem.Price:0.00}" : "$0.00";

        public string ItemTotalPrice => this.ShopItem != null ? $"${(this.Quantity * this.ShopItem.Price):0.00}" : "$0.00";

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
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
