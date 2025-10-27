using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Sacristan.Ahhnold.IO.Serialized
{
    public static partial class SaveFile
    {
        public abstract class Packer : IO.SaveFile.PackerBase
        {
            const string HashFileExtension = ".hsh";

            public enum SerializationType
            {
                JSON
            }

            public virtual SerializationType SaveFileSerializationType { get; set; } = SerializationType.JSON;
            private string HashFilePath => SaveFilePath + HashFileExtension;
            public bool HasHashFile => File.Exists(HashFilePath);

            public virtual void Save(object data)
            {
                switch (SaveFileSerializationType)
                {
                    case SerializationType.JSON:
                        SerializeJSON(data);
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }

            public virtual T Load<T>()
            {
                if (!HasSaveFile) return default(T);

                switch (SaveFileSerializationType)
                {
                    case SerializationType.JSON:
                        return DeserializeJSON<T>();
                    default:
                        throw new System.NotImplementedException();
                }
            }

            public virtual void WriteRawData(string data)
            {
                try
                {

                    System.IO.File.WriteAllText(SaveFilePath, data);
                    BuildHashFile(data);
                }
                catch (IOException e)
                {
                    Debug.LogError($"Ahhnold.IO.Serialized {nameof(WriteRawData)} got IO Exception {e.Message}");
                }
            }

            public virtual string ReadRawData()
            {
                return HasSaveFile ? System.IO.File.ReadAllText(SaveFilePath) : string.Empty;
            }

            public override void Delete()
            {
                base.Delete();
                if (HasHashFile) File.Delete(HashFilePath);
            }

            public bool IsHashValid()
            {
                if (!HasHashFile || !HasSaveFile) return false;
                string data;

                switch (SaveFileSerializationType)
                {
                    case SerializationType.JSON:
                        data = File.ReadAllText(SaveFilePath);
                        break;
                    default:
                        return false;
                }

                string hash = File.ReadAllText(HashFilePath);
                return IsHashValid(data: data, hash: hash);
            }

            private void SerializeJSON(object data)
            {
                string json = JsonUtility.ToJson(data);
                WriteRawData(json);
            }

            private T DeserializeJSON<T>()
            {
                string json = ReadRawData();
                return JsonUtility.FromJson<T>(json);
            }

            private void BuildHashFile(string data)
            {
                System.IO.File.WriteAllText(HashFilePath, GetHash(data));
            }
        }
    }
}