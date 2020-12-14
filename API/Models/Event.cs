using System;
namespace API.Models
{
    public class Event
    {
        public int Id { get; set; }

        public EventType Type { get; set; }

        public DateTime Date { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string Image { get; set; }
    }

    public enum EventType
    {
        News,
        Event
    }
}
