using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Sacristan.Ahhnold.IO.Serialized
{
    public static partial class SaveFile
    {
        public abstract class Packer : IO.SaveFile.Packer
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

            public virtual void Load<T>(ref object data)
            {
                if (!HasSaveFile) return;

                switch (SaveFileSerializationType)
                {
                    case SerializationType.JSON:
                        DeserializeJSON<T>(ref data);
                        break;
                    default:
                        throw new System.NotImplementedException();
                }

            }

            protected bool ValidateHash()
            {
                if (!HasHashFile) return false;
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
                return IsValidHash(data: data, hash: hash);
            }

            private void SerializeJSON(object data)
            {
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(SaveFilePath, json);
                BuildHashFile(json);
            }

            private void DeserializeJSON<T>(ref object data)
            {
                string json = System.IO.File.ReadAllText(SaveFilePath);
                data = JsonUtility.FromJson<T>(json);
            }

            private void BuildHashFile(string data)
            {
                System.IO.File.WriteAllText(HashFilePath, GetHash(data));
            }
        }
    }
}