# simple-notes-application-38005-38038

Simple Notes App - Backend (ASP.NET Core Minimal API, .NET 8)

Service
- Port: 3001
- Swagger UI: http://localhost:3001/docs
- OpenAPI JSON: http://localhost:3001/openapi.json
- Health: GET http://localhost:3001/

Notes Model
- Id (Guid)
- Title (string, 1-200 chars)
- Content (string, 1-10,000 chars)
- CreatedAt (UTC)
- UpdatedAt (UTC)

Persistence
- File-based JSON storage for demo purposes, saved under notes_backend/data/notes.json.
- In-memory during runtime; persisted on each change.

API Endpoints
- GET /api/notes
  - List notes ordered by UpdatedAt desc.
- GET /api/notes/{id}
  - Get a single note by id.
- POST /api/notes
  - Create a note.
  - Body: { "title": "Some title", "content": "Content here" }
- PUT /api/notes/{id}
  - Update an existing note.
  - Body: { "title": "New title", "content": "Updated content" }
- DELETE /api/notes/{id}
  - Delete a note.

Sample Requests (curl)
- List all:
  curl -s http://localhost:3001/api/notes | jq .

- Create:
  curl -s -X POST http://localhost:3001/api/notes \
    -H "Content-Type: application/json" \
    -d '{ "title": "My first note", "content": "Hello world" }' | jq .

- Get by id:
  curl -s http://localhost:3001/api/notes/<GUID> | jq .

- Update:
  curl -s -X PUT http://localhost:3001/api/notes/<GUID> \
    -H "Content-Type: application/json" \
    -d '{ "title": "Updated", "content": "Updated content" }' | jq .

- Delete:
  curl -i -X DELETE http://localhost:3001/api/notes/<GUID>

Development
- Start: dotnet run (from notes_backend directory). Launch profiles bind to port 3001.
- Swagger UI available at /docs when running locally.

Notes
- This is a simple demo; replace file persistence with a proper database for production usage.
