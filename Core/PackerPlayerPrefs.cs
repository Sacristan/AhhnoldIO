using UnityEngine;

namespace Sacristan.Ahhnold.IO.PlayerPrefs
{
    public static partial class SaveFile
    {
        public abstract class Packer : Serialized.SaveFile.Packer
        {
            private string HashKey => FileName + ".hash";

            public override bool HasSaveFile => UnityEngine.PlayerPrefs.HasKey(FileName);
            public new bool HasHashFile => UnityEngine.PlayerPrefs.HasKey(HashKey);

            public override void WriteRawData(string data)
            {
                UnityEngine.PlayerPrefs.SetString(FileName, data);
                UnityEngine.PlayerPrefs.SetString(HashKey, GetHash(data));
                UnityEngine.PlayerPrefs.Save();
            }

            public override string ReadRawData()
                => UnityEngine.PlayerPrefs.GetString(FileName, string.Empty);

            public override void Delete()
            {
                UnityEngine.PlayerPrefs.DeleteKey(FileName);
                UnityEngine.PlayerPrefs.DeleteKey(HashKey);
                UnityEngine.PlayerPrefs.Save();
            }

            public new bool IsHashValid()
            {
                if (!HasHashFile || !HasSaveFile) return false;
                return IsHashValid(data: ReadRawData(), hash: UnityEngine.PlayerPrefs.GetString(HashKey));
            }
        }
    }
}
