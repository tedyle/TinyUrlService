namespace TinyUrlService.Utils
{
    public interface IUrlShorterner
    {
        string GetUrlChunk(int id);
    }
}