namespace Blog4Devs.Models
{
    public class Posts
    {
        public Guid Id { get; set; }

        public string Title { get; set; }   

        public string? Description { get; set; }

        public bool Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Comments> Comments { get; set; } = new List<Comments>();

        public Posts()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
