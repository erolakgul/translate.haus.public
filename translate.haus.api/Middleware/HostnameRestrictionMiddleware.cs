using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using translate.haus.api.Settings;

namespace translate.haus.api.Middleware
{
    public class HostnameRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HostnameRestrictionMiddleware> _logger;
        private readonly AllowedMachinesSettings _settings;
        public HostnameRestrictionMiddleware(RequestDelegate next, ILogger<HostnameRestrictionMiddleware> logger,IOptions<AllowedMachinesSettings> settings)
        {
            _next = next;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var hostname = context.Connection.RemoteIpAddress?.ToString();
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");
            try
            {
                // IP'den hostname'e çözümleme yapılıyor
                if (!string.IsNullOrEmpty(hostname))
                {
                    var hostEntry = await Dns.GetHostEntryAsync(hostname);
                    var clientMachineName = hostEntry.HostName;

                    //_logger.LogInformation($"Gelen istek: {clientMachineName}");
                 
                    await System.IO.File.AppendAllTextAsync(logFilePath, $"[{DateTime.Now}] İstekte Bulunan Makina: {JsonSerializer.Serialize(clientMachineName)}\n");

                    // appsettings.json daki paternlerle uyuşmuyorsa istek atamasın
                    bool allowed = _settings.Patterns?.Any(pattern =>
                                                            clientMachineName.StartsWith(pattern, StringComparison.OrdinalIgnoreCase)) ?? false;

                    if (!allowed)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Erişim reddedildi: Yetkisiz cihaz.");
                        await System.IO.File.AppendAllTextAsync(logFilePath, $"[{DateTime.Now}] İzin Sonuç Bilgisi: {JsonSerializer.Serialize(context.Response.StatusCode)}\n");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Hostname kontrolü sırasında hata oluştu.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"Sunucu hatası.{ex.Message}");
                await System.IO.File.AppendAllTextAsync(logFilePath, $"[{DateTime.Now}] İzin Sorgulama Hata Bilgisi: {JsonSerializer.Serialize(ex)}\n");
                return;
            }

            await _next(context);
        }
    }

}
