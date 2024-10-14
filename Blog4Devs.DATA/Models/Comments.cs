using Microsoft.Extensions.Hosting;

namespace Blog4Devs.Models
{
    public class Comments
    {
        public Guid Id { get; set; }                // ID do comentário (Primary Key)
        public string AuthorName { get; set; }     // Nome do autor do comentário
        public string? Content { get; set; }        // Conteúdo do comentário
        public DateTime CreatedAt { get; set; }    // Data do comentário
        public Guid PostId { get; set; }            // Chave estrangeira para o Post

        public Comments()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
