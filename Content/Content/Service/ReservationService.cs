using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;

namespace Content.Service
{
    public class ReservationService
    {
        private readonly IReservationRepo _reservationRepo;

        public ReservationService(IReservationRepo reservationRepo)
        {
            _reservationRepo = reservationRepo;
        }
        public IEnumerable<Reservation> GetAllReservations()
        {
            return _reservationRepo.GetAll();
        }
        public Reservation GetReservationById(int id)
        {
            return _reservationRepo.GetById(id);
        }
        public void reserveCart(Reservation reservation)
        {
            _reservationRepo.Add(reservation);
        }
        /*public void reserveCart(Cart ReservationCart,bool Active,DateTime ReservationDate)
        {
            Reservation reservation = new Reservation(Id, ReservationCart, Active, ReservationDate);
            _reservationRepo.Add(reservation);
        }*/
        public void DeleteReservation(int id)
        {
            _reservationRepo.Delete(id);
        }

        public void cancelReservation(int id)
        {
            Reservation reservation = _reservationRepo.GetById(id);
            reservation.Active = false;
            _reservationRepo.Update(reservation);

        }
    }
}