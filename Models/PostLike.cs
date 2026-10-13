namespace EchoVibe.Models;

public class PostLike
{
    public int Id { get; set; }
    public int VibePostId { get; set; }
    public VibePost? VibePost { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}