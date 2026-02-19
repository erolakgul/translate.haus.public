using translate.haus.api.Middleware;
using translate.haus.api.Services;
using translate.haus.api.Services.interfaces;
using translate.haus.api.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// service lerin container a kaydedilmesi
builder.Services.AddScoped<IDeepLServices, DeeplServices>();

// DeepL ayarlarýný otomatik olarak appsettings.json'dan oku ve DI'a ekle
builder.Services.Configure<DeepLSettings>(builder.Configuration.GetSection("DeepL"));
// Ýzin verilen makina ayarlarýný otomatik olarak appsettings.json'dan oku ve DI'a ekle
builder.Services.Configure<AllowedMachinesSettings>(builder.Configuration.GetSection("AllowedMachines"));

var app = builder.Build();

// api ye istek atabilecek makinalarý kontrol et
app.UseMiddleware<HostnameRestrictionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
