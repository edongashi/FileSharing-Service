namespace FileSharing.Serveri.Infrastruktura.Abstrakt
{
    public interface IPathResolver
    {
        string GetRootFolder();

        string GetDataFolder();

        string GetTempFolder();

        string GetFileInRootPath(string file);

        string GetFileInDataPath(string file);

        string GetTempFile();
    }
}
