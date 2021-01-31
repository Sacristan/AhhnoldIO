using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;

namespace Sacristan.Ahhnold.IO
{
    public static class SaveFile
    {
        public abstract class Processor
        {
            public virtual Packer SaveFilePacker => null;
            public virtual void Save() => SaveFilePacker?.Save();
            public virtual IEnumerator SaveAsync() => SaveFilePacker?.SaveAsync();
            public virtual void Load() => SaveFilePacker?.Load();
            public virtual IEnumerator LoadAsync() => SaveFilePacker?.LoadAsync();
            public virtual void Reset() => Delete();
            public virtual void Delete() => SaveFilePacker?.Delete();
        }

        #region Packers
        private static string GetDataPath(string fileName)
        {
            return Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
        }
        public abstract class Packer
        {
            const string InvalidHashMessage = "InvalidHash";

            protected virtual byte Version { get; }
            protected virtual string FileName { get; }
            protected virtual string Extension => ".dat";
            protected virtual string Salt => "17t5j010Z611KIx";
            protected virtual System.Text.Encoding Encoding => System.Text.Encoding.ASCII;

            private string FileNameWithExtension => FileName + Extension;

            protected byte UnpackedVersion { get; private set; } = 0;

            public bool HasSaveFile => File.Exists(GetDataPath(FileNameWithExtension));

            public bool ReachedEndOfSaveFileData(BinaryReader reader) => reader.BaseStream.Position >= (reader.BaseStream.Length - 64 - 1); // HASH 64 - 1 pos

            public void Save()
            {
                using (FileStream stream = File.Create(GetDataPath(FileNameWithExtension)))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, Encoding))
                    {
                        writer.Write(Version);
                        string data = PackData(writer);
                        writer.Write(GetHash(data));
                    }
                }
            }

            public IEnumerator SaveAsync()
            {
                using (FileStream stream = File.Create(GetDataPath(FileNameWithExtension)))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, Encoding))
                    {
                        writer.Write(Version);

                        string data = string.Empty;
                        yield return PackDataAsync(writer, (string x) => data = x);
                        writer.Write(GetHash(data));
                    }
                }
            }

            public void Load()
            {
                if (!HasSaveFile) return;

                try
                {
                    using (FileStream stream = File.Open(GetDataPath(FileNameWithExtension), FileMode.Open))
                    {
                        using (BinaryReader reader = new BinaryReader(stream, Encoding))
                        {  
                            UnpackedVersion = reader.ReadByte();
                            UnpackData(reader);
                        }
                    }
                }
                catch (EndOfStreamException e) { HandleCorruptedFile(e.ToString()); }
            }

            public IEnumerator LoadAsync()
            {
                if (!HasSaveFile) yield break;

                using (FileStream stream = File.Open(GetDataPath(FileNameWithExtension), FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding))
                    {  
                        UnpackedVersion = reader.ReadByte();
                        yield return UnpackDataAsync(reader);
                    }
                }
            }

            public void Delete()
            {
                string path = GetDataPath(FileNameWithExtension);
                if (HasSaveFile) File.Delete(path);
            }

            protected bool ValidateHash(BinaryReader reader, string data)
            {
                string fileHash = reader.ReadString();
                string decodedHash = GetHash(data);
                return fileHash.Equals(decodedHash);
            }

            protected virtual string PackData(BinaryWriter writer)
            {
                throw new System.NotImplementedException();
            }

            protected virtual void UnpackData(BinaryReader reader)
            {
                throw new System.NotImplementedException();
            }

            protected virtual IEnumerator PackDataAsync(BinaryWriter writer, System.Action<string> dataCallback)
            {
                throw new System.NotImplementedException();
            }

            protected virtual IEnumerator UnpackDataAsync(BinaryReader reader)
            {
                throw new System.NotImplementedException();
            }

            protected void HandleCorruptedFile(string msg = InvalidHashMessage)
            {
                UnityEngine.Debug.LogErrorFormat("ERROR: {0} Corrupted file detected. Default values will be loaded: {1}", msg, FileNameWithExtension);
            }

            #region Packers
            public void Pack(BinaryWriter writer, StringBuilder packer, bool data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, string data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, float data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, short data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, int data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, long data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, sbyte data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, byte data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, ushort data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, uint data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, ulong data)
            {
                writer.Write(data);
                packer.Append(data);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, Vector2 data)
            {
                Pack(writer, packer, data.x);
                Pack(writer, packer, data.y);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, Vector3 data)
            {
                Pack(writer, packer, data.x);
                Pack(writer, packer, data.y);
                Pack(writer, packer, data.z);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, Vector4 data)
            {
                Pack(writer, packer, data.x);
                Pack(writer, packer, data.y);
                Pack(writer, packer, data.z);
                Pack(writer, packer, data.w);
            }

            public void Pack(BinaryWriter writer, StringBuilder packer, Quaternion data)
            {
                Pack(writer, packer, data.x);
                Pack(writer, packer, data.y);
                Pack(writer, packer, data.z);
                Pack(writer, packer, data.w);
            }

            #endregion

            #region Unpackers
            public bool UnpackBool(BinaryReader reader, StringBuilder packer)
            {
                bool data = reader.ReadBoolean();
                packer.Append(data);
                return data;
            }

            public string UnpackString(BinaryReader reader, StringBuilder packer)
            {
                string data = reader.ReadString();
                packer.Append(data);
                return data;
            }

            public float UnpackFloat(BinaryReader reader, StringBuilder packer)
            {
                float data = reader.ReadSingle();
                packer.Append(data);
                return data;
            }

            public short UnpackShort(BinaryReader reader, StringBuilder packer)
            {
                short data = reader.ReadInt16();
                packer.Append(data);
                return data;
            }

            public int UnpackInt(BinaryReader reader, StringBuilder packer)
            {
                int data = reader.ReadInt32();
                packer.Append(data);
                return data;
            }

            public long UnpackLong(BinaryReader reader, StringBuilder packer)
            {
                long data = reader.ReadInt64();
                packer.Append(data);
                return data;
            }

            public sbyte UnpackSByte(BinaryReader reader, StringBuilder packer)
            {
                sbyte data = reader.ReadSByte();
                packer.Append(data);
                return data;
            }

            public byte UnpackByte(BinaryReader reader, StringBuilder packer)
            {
                byte data = reader.ReadByte();
                packer.Append(data);
                return data;
            }

            public ushort UnpackUShort(BinaryReader reader, StringBuilder packer)
            {
                ushort data = reader.ReadUInt16();
                packer.Append(data);
                return data;
            }

            public uint UnpackUInt(BinaryReader reader, StringBuilder packer)
            {
                uint data = reader.ReadUInt32();
                packer.Append(data);
                return data;
            }

            public ulong UnpackULong(BinaryReader reader, StringBuilder packer)
            {
                ulong data = reader.ReadUInt64();
                packer.Append(data);
                return data;
            }

            public Vector2 UnpackVector2(BinaryReader reader, StringBuilder packer)
            {
                float x = UnpackFloat(reader, packer);
                float y = UnpackFloat(reader, packer);

                return new Vector2(x, y);
            }

            public Vector3 UnpackVector3(BinaryReader reader, StringBuilder packer)
            {
                float x = UnpackFloat(reader, packer);
                float y = UnpackFloat(reader, packer);
                float z = UnpackFloat(reader, packer);

                return new Vector3(x, y, z);
            }

            public Vector4 UnpackVector4(BinaryReader reader, StringBuilder packer)
            {
                float x = UnpackFloat(reader, packer);
                float y = UnpackFloat(reader, packer);
                float z = UnpackFloat(reader, packer);
                float w = UnpackFloat(reader, packer);

                return new Vector4(x, y, z, w);
            }

            public Quaternion UnpackQuaternion(BinaryReader reader, StringBuilder packer)
            {
                float x = UnpackFloat(reader, packer);
                float y = UnpackFloat(reader, packer);
                float z = UnpackFloat(reader, packer);
                float w = UnpackFloat(reader, packer);

                return new Quaternion(x, y, z, w);
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

            #endregion
        }

        #endregion
    }
}