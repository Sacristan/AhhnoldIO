using System.IO;
using System.Text;
using System.Collections;

namespace Sacristan.Ahhnold.IO
{
    public static partial class SaveFile
    {
        public interface IPacker
        {
            void Save();
            IEnumerator SaveAsync();
            void Load();
            IEnumerator LoadAsync();
        }

        public abstract class Packer : IPacker //TEMP
        {
            protected const string InvalidHashMessage = "InvalidHash";
            protected virtual string FileName { get; }
            protected virtual string Extension => ".dat";
            protected virtual string Salt => "17t5j010Z611KIx";
            protected string FileNameWithExtension => FileName + Extension;
            public bool HasSaveFile => File.Exists(GetDataPath(FileNameWithExtension));
            public string SaveFilePath => GetDataPath(FileNameWithExtension);

            #region  REMOVE
            public void Save()
            {
                throw new System.NotImplementedException();
            }

            public IEnumerator SaveAsync()
            {
                throw new System.NotImplementedException();
            }

            public void Load()
            {
                throw new System.NotImplementedException();
            }

            public IEnumerator LoadAsync()
            {
                throw new System.NotImplementedException();
            }
            #endregion

            public void Delete()
            {
                string path = GetDataPath(FileNameWithExtension);
                if (HasSaveFile) File.Delete(path);
            }

            protected string GetHash(string data)
            {
                return Sha256Sum(data + "(╯°□°）╯︵ ┻━┻" + Salt);
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