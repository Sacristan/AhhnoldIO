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
            public enum SerializationType
            {
                JSON,
                BinaryFormatter
            }

            public SerializationType SaveFileSerializationType { get; set; } = SerializationType.JSON;

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

            private void SerializeJSON(object data)
            {
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(SaveFilePath, json);
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
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fileStream, data);
                }
            }

            private void DerializeBinaryFormatter<T>(ref object data)
            {
                using (FileStream fileStream = File.Create(SaveFilePath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (T)bf.Deserialize(fileStream);
                }
            }
        }
    }
}