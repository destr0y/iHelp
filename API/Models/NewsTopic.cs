using System;
namespace API.Models
{
    public class NewsTopic
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public Account Author { get; set; }
    }
}
