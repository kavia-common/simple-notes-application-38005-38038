using NotesBackend.Models;

namespace NotesBackend.Services
{
    // PUBLIC_INTERFACE
    public interface INoteRepository
    {
        /// <summary>
        /// Returns all notes ordered by UpdatedAt descending.
        /// </summary>
        IEnumerable<Note> GetAll();

        /// <summary>
        /// Returns a note by id or null if not found.
        /// </summary>
        Note? GetById(Guid id);

        /// <summary>
        /// Creates and returns the created note.
        /// </summary>
        Note Create(string title, string content);

        /// <summary>
        /// Updates an existing note and returns it, or null if not found.
        /// </summary>
        Note? Update(Guid id, string title, string content);

        /// <summary>
        /// Deletes a note by id. Returns true if it existed and was deleted.
        /// </summary>
        bool Delete(Guid id);
    }
}
