using System;
using System.IO;
using System.Text;

namespace Sacristan.Ahhnold.IO
{
    public static class SaveFile
    {
        public abstract class Processor
        {
            public virtual Packer SaveFilePacker => null;
            public virtual void Save() { }
            public virtual void Load() { }
            public virtual void Reset() { }
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

            private string FileNameWithExtension => FileName + Extension;

            protected byte UnpackedVersion { get; private set; } = 0;

            public bool HasSaveFile => File.Exists(GetDataPath(FileNameWithExtension));

            public void Save()
            {
                using (FileStream stream = File.Create(GetDataPath(FileNameWithExtension)))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.ASCII))
                    {
                        writer.Write(Version);
                        string data = PackData(writer);
                        writer.Write(GetHash(data));
                    }
                }
            }

            public void Load()
            {
                string path = GetDataPath(FileNameWithExtension);
                if (!HasSaveFile) return;

                try
                {

                    using (FileStream stream = File.Open(path, FileMode.Open))
                    {
                        using (BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.ASCII))
                        {
                            UnpackedVersion = reader.ReadByte();
                            UnpackData(reader);
                        }
                    }
                }
                catch (EndOfStreamException e) { HandleCorruptedFile(e.ToString()); }
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
                uint data = reader.ReadByte();
                packer.Append(data);
                return data;
            }

            public ulong UnpackULong(BinaryReader reader, StringBuilder packer)
            {
                ulong data = reader.ReadByte();
                packer.Append(data);
                return data;
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