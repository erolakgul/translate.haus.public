namespace translate.haus.api.Models
{
    public class TranslationRequests
    {
        public string? mtext { get; set; }
        public List<string>? text { get; set; }
        public string? source_lang { get; set; }
        public string? target_lang { get; set; }
    }
}
