using System.Collections.Concurrent;
using System.Text.Json;
using NotesBackend.Models;

namespace NotesBackend.Services
{
    /// <summary>
    /// Simple file-based note repository using JSON for persistence.
    /// Thread-safe for typical dev/demo usage with a ConcurrentDictionary backing store.
    /// </summary>
    public class FileNoteRepository : INoteRepository
    {
        private readonly string _dataFilePath;
        private readonly ConcurrentDictionary<Guid, Note> _notes = new();

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

        public FileNoteRepository(IHostEnvironment env, IConfiguration configuration)
        {
            // Data directory within app root
            var dataDir = Path.Combine(env.ContentRootPath, "data");
            Directory.CreateDirectory(dataDir);
            _dataFilePath = Path.Combine(dataDir, "notes.json");

            LoadFromDisk();
        }

        public IEnumerable<Note> GetAll()
        {
            return _notes.Values
                .OrderByDescending(n => n.UpdatedAt)
                .ToList();
        }

        public Note? GetById(Guid id)
        {
            _notes.TryGetValue(id, out var note);
            return note;
        }

        public Note Create(string title, string content)
        {
            var now = DateTime.UtcNow;
            var note = new Note
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                CreatedAt = now,
                UpdatedAt = now
            };

            _notes[note.Id] = note;
            SaveToDisk();
            return note;
        }

        public Note? Update(Guid id, string title, string content)
        {
            if (!_notes.TryGetValue(id, out var existing))
            {
                return null;
            }

            existing.Title = title;
            existing.Content = content;
            existing.UpdatedAt = DateTime.UtcNow;

            _notes[id] = existing;
            SaveToDisk();
            return existing;
        }

        public bool Delete(Guid id)
        {
            var removed = _notes.TryRemove(id, out _);
            if (removed)
            {
                SaveToDisk();
            }
            return removed;
        }

        private void LoadFromDisk()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    // Seed with an example note to help first-time users
                    var seed = new[]
                    {
                        new Note
                        {
                            Id = Guid.NewGuid(),
                            Title = "Welcome to Simple Notes",
                            Content = "Use this API to create, read, update, and delete notes.",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }
                    };
                    foreach (var n in seed)
                    {
                        _notes[n.Id] = n;
                    }
                    SaveToDisk();
                    return;
                }

                var json = File.ReadAllText(_dataFilePath);
                var list = JsonSerializer.Deserialize<List<Note>>(json, JsonOptions) ?? new List<Note>();
                foreach (var n in list)
                {
                    _notes[n.Id] = n;
                }
            }
            catch
            {
                // If file is corrupt or unreadable, start fresh to keep API available.
                _notes.Clear();
            }
        }

        private void SaveToDisk()
        {
            try
            {
                var list = _notes.Values.OrderByDescending(n => n.UpdatedAt).ToList();
                var json = JsonSerializer.Serialize(list, JsonOptions);
                File.WriteAllText(_dataFilePath, json);
            }
            catch
            {
                // In a demo app we swallow exceptions; production apps should log and handle properly.
            }
        }
    }
}
