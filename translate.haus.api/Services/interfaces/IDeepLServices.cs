using translate.haus.api.Models;

namespace translate.haus.api.Services.interfaces
{
    public interface IDeepLServices
    {
        Task<TranslationResponse> RequestAsync(TranslationRequests translationRequests);
    }
}
