using Microsoft.EntityFrameworkCore;
using MinimalAPIExample;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<StudentDb>(opt => opt.UseInMemoryDatabase("StudentDb"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ===================== CRUD Endpoints =====================

// CREATE
app.MapPost("/students", async (Student student, StudentDb db) =>
{
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/students/{student.Id}", student);
});

// READ - Get All
app.MapGet("/students", async (StudentDb db) =>
    await db.Students.ToListAsync());

// READ - Get by ID
app.MapGet("/students/{id}", async (int id, StudentDb db) =>
    await db.Students.FindAsync(id)
        is Student student
            ? Results.Ok(student)
            : Results.NotFound("Student not found"));

// UPDATE
app.MapPut("/students/{id}", async (int id, Student updatedStudent, StudentDb db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null)
        return Results.NotFound("Student not found");

    student.Name = updatedStudent.Name;
    student.Age = updatedStudent.Age;
    student.Grade = updatedStudent.Grade;

    await db.SaveChangesAsync();
    return Results.Ok(student);
});

// DELETE
app.MapDelete("/students/{id}", async (int id, StudentDb db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null)
        return Results.NotFound("Student not found");

    db.Students.Remove(student);
    await db.SaveChangesAsync();
    return Results.Ok($"Student {id} deleted successfully");
});

app.Run();