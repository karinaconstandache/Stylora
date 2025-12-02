using Stylora.Domain.Interfaces;
using Stylora.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 1. Register the Service with a Typed HttpClient
// This tells .NET: "When a controller asks for IVirtualTryOnService, create a VertexTryOnService and give it an HttpClient."
builder.Services.AddHttpClient<IVirtualTryOnService, VertexTryOnService>();

// 2. Set Google Auth Credential Path
// This forces the Google Library to look for 'gcp-key.json' in your API root folder.
// Note: In production, you should set this in the server's actual Environment Variables, not code.
System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "gcp-key.json");

// -----------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAngularApp",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();