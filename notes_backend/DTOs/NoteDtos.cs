using System.ComponentModel.DataAnnotations;

namespace NotesBackend.DTOs
{
    /// <summary>
    /// Request payload for creating a note.
    /// </summary>
    public class CreateNoteRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(10000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 10,000 characters.")]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request payload for updating a note.
    /// </summary>
    public class UpdateNoteRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(10000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 10,000 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
