using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface IReservationService
    {
        void CancelReservation(int reservationId);
        void DeleteReservation(int reservationId);
        IEnumerable<Reservation> GetAllReservations();
        Reservation GetReservationById(int reservationId);
        void ReserveCart(Reservation reservation);

        Reservation GetActiveReservationForCart(int cartId);
    }
}