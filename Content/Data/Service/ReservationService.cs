using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;

namespace Content.Service
{
    public class ReservationService
    {
        private readonly IReservationRepo _reservationRepo;
        private readonly IShopItemService _shopItemService;
        private readonly ICartService _cartService;

        public ReservationService(IReservationRepo reservationRepo, IShopItemService shopItemService, ICartService cartService)
        {
            _reservationRepo = reservationRepo;
            _shopItemService = shopItemService;
            _cartService = cartService;
        }
        public IEnumerable<Reservation> GetAllReservations()
        {
            return _reservationRepo.GetAll();
        }
        public Reservation GetReservationById(int id)
        {
            return _reservationRepo.GetById(id);
        }
        public void ReserveCart(Reservation reservation)
        {

            var reservationCartItems = reservation.ReservationCart.CartItems;
           
            foreach(var cartItem in reservationCartItems.Values)
            {
                var shopItem = _shopItemService.GetById(cartItem.ShopItem.Id);
                if(shopItem.Quantity < cartItem.Quantity)
                {
                    throw new Exception($"Not enough stock for '{shopItem.Name}'. " +
                        $"Requested: {cartItem.Quantity}, Available: {shopItem.Quantity}");
                }
            }

            foreach (var cartItem in reservationCartItems.Values)
            {
                var shopItem = _shopItemService.GetById(cartItem.ShopItem.Id);
                shopItem.Quantity -= cartItem.Quantity;
                _shopItemService.UpdateShopItem(shopItem);
            }

            _reservationRepo.Add(reservation);
        }
        
        public void DeleteReservation(int id)
        {
            _reservationRepo.Delete(id);
        }

        public void cancelReservation(int id)
        {
            Reservation reservation = _reservationRepo.GetById(id);

            if (!reservation.Active)
            {
                return;
            }

            if (reservation.ReservationCart?.CartItems != null)
            {
                foreach (var cartItem in reservation.ReservationCart.CartItems.Values)
                {
                    var shopItem = _shopItemService.GetById(cartItem.ShopItem.Id);
                    shopItem.Quantity += cartItem.Quantity;
                    _shopItemService.UpdateShopItem(shopItem);
                }
            }

            _cartService.ClearCart(reservation.ReservationCart.Id);
            reservation.Active = false;

            _reservationRepo.Update(reservation);

        }
    }
}