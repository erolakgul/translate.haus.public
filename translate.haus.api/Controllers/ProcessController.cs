using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using translate.haus.api.Models;
using translate.haus.api.Services.interfaces;


namespace translate.haus.api.Controllers
{
    public class ProcessController : Controller
    {
        private readonly IDeepLServices _deepLServices;
        public ProcessController(IDeepLServices deepLServices)
        {
            _deepLServices = deepLServices;
        }

        [HttpPost("translate")]
        public async Task<IActionResult> Post([FromBody] TranslationRequests request)
        {
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");

            await System.IO.File.AppendAllTextAsync(logFilePath, $"[{DateTime.Now}] Gelen İstek Bilgisi : {JsonSerializer.Serialize(request)}\n");

            TranslationResponse translations = new();
            try
            {
                if (request is null ||
                    string.IsNullOrWhiteSpace(request.mtext) ||
                    string.IsNullOrWhiteSpace(request.source_lang) ||
                    string.IsNullOrWhiteSpace(request.target_lang))
                {
                    await System.IO.File.AppendAllTextAsync(logFilePath, $"[{DateTime.Now}] Boş Veri Bilgisi : {JsonSerializer.Serialize(request)}\n");
                    return BadRequest("Tüm alanlar zorunludur.");
                }

                if (request.text?.Count == 1 && request.text.Any(x=> x.Length == 0)) 
                    request.text.Clear();

                if (request.mtext != "string" && request.mtext is not null)
                    request?.text?.Add(request.mtext);

                if (request is not null)
                    translations = await _deepLServices.RequestAsync(request);
            }
            catch (Exception ex)
            {
                await System.IO.File.AppendAllTextAsync(logFilePath, $"[{DateTime.Now}] Translate İşlemi Hata Bilgisi : {JsonSerializer.Serialize(ex)}\n");
                return BadRequest(ex);
            }

            return Ok(translations);
        }
    }
}
