namespace EchoHub.Models
{
    public class Collection
    {
        public int CollectionId { get; set; }
        public int EwasteId { get; set; }
        public EwasteItem EwasteItem { get; set; }

        public int StaffId { get; set; }
        public User Staff { get; set; }

        public DateTime ScheduleDate { get; set; }
        public DateTime? CollectionDate { get; set; }

        public String Status { get; set; }
    }
}
