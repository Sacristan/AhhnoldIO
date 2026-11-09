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
            public virtual bool HasSaveFile => File.Exists(SaveFilePath);
            public string SaveFilePath => GetDataPath(FileNameWithExtension);

            public virtual void Delete()
            {
                string path = SaveFilePath;
                if (HasSaveFile) File.Delete(path);
                DeleteTempFile(path);
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

            protected const string TempFileExtension = ".tmp";

            // Waits until the save is physically on storage before swapping it in, so it survives power loss or an
            // OS crash. An app kill doesn't need it (the OS still writes out what it was handed), so packers that
            // save very often can turn it off to skip that wait and keep the crash-safe swap.
            protected virtual bool FlushToDisk => true;

            protected static string GetTempFilePath(string path) => path + TempFileExtension;

            // Saves are written here, then swapped in with CommitTempFile, so a crash never leaves a truncated save.
            protected static FileStream CreateTempFile(string path) =>
                new FileStream(GetTempFilePath(path), FileMode.Create, FileAccess.Write, FileShare.None);

            protected void WriteTempFile(string path, string contents)
            {
                byte[] bytes = new UTF8Encoding(false).GetBytes(contents);
                bool written = false;

                try
                {
                    using (FileStream stream = CreateTempFile(path))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush(FlushToDisk);
                    }

                    written = true;
                }
                finally
                {
                    // A failed write (fe disk full) shouldn't leave a partial temp taking up the space.
                    if (!written) TryDeleteTempFile(path);
                }
            }

            protected static void CommitTempFile(string path)
            {
                string tempPath = GetTempFilePath(path);

                if (!File.Exists(path))
                {
                    File.Move(tempPath, path);
                    return;
                }

                try
                {
                    File.Replace(tempPath, path, null);
                }
                catch (System.PlatformNotSupportedException)
                {
                    File.Delete(path);
                    File.Move(tempPath, path);
                }
            }

            protected static void DeleteTempFile(string path)
            {
                string tempPath = GetTempFilePath(path);
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }

            // For cleanup after a failed save, where the original exception is the one worth reporting.
            protected static void TryDeleteTempFile(string path)
            {
                try { DeleteTempFile(path); }
                catch (IOException) { }
                catch (System.UnauthorizedAccessException) { }
            }

        }

    }
}