using System;
namespace iHelp.Models
{
    public class Review
    {
        public int Id { get; set; }

        public Account Author { get; set; }

        public int RequestId { get; set; }

        public string Text { get; set; }

        public int Mark { get; set; }
    }
}
