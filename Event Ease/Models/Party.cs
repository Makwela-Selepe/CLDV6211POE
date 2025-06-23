using Event_Ease.Models;
namespace Event_Ease.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public string? EventLocation { get; set; }
        public DateTime EventDate { get; set; }
        public int VenueId { get; set; }
        public Venue Venue { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}