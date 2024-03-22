

using PixelTestTask.Services.Pixel;
using Services.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServiceDefaults(builder.Configuration);

builder.Services.AddScoped<IPixelService, PixelService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("track", async (HttpContext context, IPixelService pixelService) =>
{
    pixelService.TrackImage();
    
    // for test purposes used base64 for 1 pixel gif image
    var base64GifData = "R0lGODlhAQABAIAAAAd9EwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==";
    var gifBytes = Convert.FromBase64String(base64GifData);

    context.Response.ContentType = "image/gif";
    
    await context.Response.Body.WriteAsync(gifBytes.AsMemory(0, gifBytes.Length));
});

app.Run();