using Event_Ease.Models;
namespace Event_Ease.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }

        public int VenueId { get; set; }

        public DateTime BookingDate { get; set; }
    }
}