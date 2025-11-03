using System.ComponentModel.DataAnnotations;

namespace NotesBackend.Models
{
    /// <summary>
    /// Domain model representing a Note.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Unique identifier of the note.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the note.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Content/body of the note.
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// Creation timestamp (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update timestamp (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
