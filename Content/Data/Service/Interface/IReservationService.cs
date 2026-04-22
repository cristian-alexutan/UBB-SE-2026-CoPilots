using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface IReservationService
    {
        void CancelReservation(int id);
        void DeleteReservation(int id);
        IEnumerable<Reservation> GetAllReservations();
        Reservation GetReservationById(int id);
        void ReserveCart(Reservation reservation);
    }
}