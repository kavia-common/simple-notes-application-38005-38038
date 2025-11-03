using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using NSwag.Annotations;
using NotesBackend.DTOs;
using NotesBackend.Models;
using NotesBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// App metadata for OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Simple Notes API";
    settings.Description = "A minimal REST API for creating, reading, updating, and deleting notes.";
    settings.Version = "1.0.0";
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register repository
builder.Services.AddSingleton<INoteRepository, FileNoteRepository>();

var app = builder.Build();

// Ensure running on port 3001 if not provided by environment (launchSettings already defines 3001)
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (string.IsNullOrWhiteSpace(urls))
{
    // If not configured, default to http://0.0.0.0:3001 for container friendliness
    app.Urls.Add("http://0.0.0.0:3001");
}

// CORS
app.UseCors("AllowAll");

// OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

// Health
// PUBLIC_INTERFACE
app.MapGet("/", () => Results.Ok(new { message = "Healthy" }))
   .WithName("HealthCheck")
   .WithSummary("Health check")
   .WithDescription("Simple health check endpoint.")
   .Produces(StatusCodes.Status200OK)
   .WithTags("Health");

// Notes endpoints
var notes = app.MapGroup("/api/notes").WithTags("Notes");

// PUBLIC_INTERFACE
notes.MapGet("/", (INoteRepository repo) =>
{
    /// <summary>
    /// List notes
    /// </summary>
    /// <returns>All notes ordered by last updated descending.</returns>
    var all = repo.GetAll();
    return Results.Ok(all);
})
.WithName("ListNotes")
.WithSummary("List all notes")
.WithDescription("Returns all notes ordered by UpdatedAt descending.")
.Produces<IEnumerable<Note>>(StatusCodes.Status200OK);

// PUBLIC_INTERFACE
notes.MapGet("/{id:guid}", (INoteRepository repo, Guid id) =>
{
    /// <summary>
    /// Get a single note by id.
    /// </summary>
    var note = repo.GetById(id);
    return note is null ? Results.NotFound() : Results.Ok(note);
})
.WithName("GetNote")
.WithSummary("Get note by id")
.WithDescription("Returns a single note by its unique identifier.")
.Produces<Note>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// PUBLIC_INTERFACE
notes.MapPost("/", (INoteRepository repo, CreateNoteRequest request) =>
{
    /// <summary>
    /// Create a note.
    /// </summary>
    if (request is null)
    {
        return Results.BadRequest(new { message = "Request body is required." });
    }

    // Manual validation since we are using minimal APIs without automatic model validation
    var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(request);
    if (!Validator.TryValidateObject(request, context, validationResults, true))
    {
        return Results.ValidationProblem(validationResults
            .GroupBy(v => v.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Select(v => v.ErrorMessage ?? "Invalid").ToArray()));
    }

    var created = repo.Create(request.Title.Trim(), request.Content.Trim());
    return Results.Created($"/api/notes/{created.Id}", created);
})
.WithName("CreateNote")
.WithSummary("Create a new note")
.WithDescription("Creates a new note with a title and content.")
.Produces<Note>(StatusCodes.Status201Created)
.ProducesValidationProblem(StatusCodes.Status400BadRequest);

// PUBLIC_INTERFACE
notes.MapPut("/{id:guid}", (INoteRepository repo, Guid id, UpdateNoteRequest request) =>
{
    /// <summary>
    /// Update an existing note by id.
    /// </summary>
    if (request is null)
    {
        return Results.BadRequest(new { message = "Request body is required." });
    }

    var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(request);
    if (!Validator.TryValidateObject(request, context, validationResults, true))
    {
        return Results.ValidationProblem(validationResults
            .GroupBy(v => v.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Select(v => v.ErrorMessage ?? "Invalid").ToArray()));
    }

    var updated = repo.Update(id, request.Title.Trim(), request.Content.Trim());
    return updated is null ? Results.NotFound() : Results.Ok(updated);
})
.WithName("UpdateNote")
.WithSummary("Update a note")
.WithDescription("Updates the title and content of an existing note.")
.Produces<Note>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.ProducesValidationProblem(StatusCodes.Status400BadRequest);

// PUBLIC_INTERFACE
notes.MapDelete("/{id:guid}", (INoteRepository repo, Guid id) =>
{
    /// <summary>
    /// Delete a note by id.
    /// </summary>
    var deleted = repo.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteNote")
.WithSummary("Delete a note")
.WithDescription("Deletes a note by its id.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();