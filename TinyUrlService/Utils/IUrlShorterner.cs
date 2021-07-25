namespace TinyUrlService.Utils
{
    public interface IUrlShorterner
    {
        int GetId(string urlChunk);
        string GetUrlChunk(int id);
    }
}