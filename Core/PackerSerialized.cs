using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Sacristan.Ahhnold.IO
{
    public static partial class SaveFile
    {
        public abstract class PackerSerialized : Packer
        {
            const string HashFileExtension = ".hsh";

            public enum SerializationType
            {
                JSON,
                BinaryFormatter
            }

            public SerializationType SaveFileSerializationType { get; set; } = SerializationType.JSON;
            private string HashFilePath => SaveFilePath + HashFileExtension;
            public bool HasHashFile => File.Exists(HashFilePath);

            public virtual void Save(object data)
            {
                switch (SaveFileSerializationType)
                {
                    case SerializationType.JSON:
                        SerializeJSON(data);
                        break;
                    case SerializationType.BinaryFormatter:
                        SerializeBinaryFormatter(data);
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
                    case SerializationType.BinaryFormatter:
                        DerializeBinaryFormatter<T>(ref data);
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
                    case SerializationType.BinaryFormatter:
                        data = ByteArrayToString(File.ReadAllBytes(SaveFilePath));
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

            private void SerializeBinaryFormatter(object data)
            {
                using (FileStream fileStream = File.Create(SaveFilePath))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, data);
                }

                string stringData = ByteArrayToString(File.ReadAllBytes(SaveFilePath));
                BuildHashFile(stringData);
            }

            private void DerializeBinaryFormatter<T>(ref object data)
            {
                using (FileStream fileStream = File.Create(SaveFilePath))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    data = (T)binaryFormatter.Deserialize(fileStream);
                }
            }

            private void BuildHashFile(string data)
            {
                System.IO.File.WriteAllText(HashFilePath, GetHash(data));
            }

            private static string ByteArrayToString(byte[] bytes)
            {
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
        }
    }
}