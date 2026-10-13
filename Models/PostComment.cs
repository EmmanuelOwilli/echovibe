using System.ComponentModel.DataAnnotations;

namespace EchoVibe.Models;

public class PostComment
{
    public int Id { get; set; }
    public int VibePostId { get; set; }
    public VibePost? VibePost { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    [Required, StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}