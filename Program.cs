using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Add services
builder.Services.AddControllers();

// Register DbContext
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//STEP-1 This fixes CORS issue on the ReactJS web app Front end that runs at http://localhost:5173
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});



var app = builder.Build();
// Enable Swagger for testing API endpoints
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

//STEP-2 This fixes CORS issue on the ReactJS web app Front end that runs at http://localhost:5173
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();