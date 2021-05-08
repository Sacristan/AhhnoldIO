using System.IO;

namespace Sacristan.Ahhnold.IO
{
    public static partial class SaveFile
    {
        private static string GetDataPath(string fileName)
        {
            return Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
        }
    }
}