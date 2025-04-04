using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Host.UseSerilog((ctx, lc)=>
lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyMethod().AllowAnyHeader()
    .AllowAnyOrigin());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors ("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
