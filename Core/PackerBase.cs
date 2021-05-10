using System.IO;
using System.Text;
using System.Collections;

namespace Sacristan.Ahhnold.IO
{
    public static partial class SaveFile
    {
        public abstract class PackerBase
        {
            protected const string InvalidHashMessage = "InvalidHash";
            protected virtual string FileName { get; }
            protected virtual string Extension => ".dat";
            protected virtual string Salt => "17t5j010Z611KIx";
            protected string FileNameWithExtension => FileName + Extension;
            public bool HasSaveFile => File.Exists(GetDataPath(FileNameWithExtension));
            public string SaveFilePath => GetDataPath(FileNameWithExtension);

            public void Delete()
            {
                string path = GetDataPath(FileNameWithExtension);
                if (HasSaveFile) File.Delete(path);
            }

            protected string GetHash(string data)
            {
                return Sha256Sum(data + "(╯°□°）╯︵ ┻━┻" + Salt);
            }

            protected bool IsHashValid(string data, string hash)
            {
                return GetHash(data).Equals(hash);
            }

            private static string Sha256Sum(string str)
            {
                System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
                StringBuilder hash = new StringBuilder();
                byte[] cryptedBytes = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));

                for (int i = 0; i < cryptedBytes.Length; i++)
                {
                    byte cryptedByte = cryptedBytes[i];
                    hash.Append(cryptedByte.ToString("x2"));
                }

                return hash.ToString().ToLower();
            }


        }

    }
}