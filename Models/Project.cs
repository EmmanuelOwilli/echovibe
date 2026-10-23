using System.ComponentModel.DataAnnotations;

namespace EchoVibe.Models;

public class Project
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    // No length limit — long posts/stories allowed
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    [StringLength(80)]
    public string? VibeType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<VibePost> Posts { get; set; } = new List<VibePost>();
}
