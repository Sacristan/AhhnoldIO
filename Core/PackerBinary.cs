using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;

namespace Sacristan.Ahhnold.IO
{
    public static partial class SaveFile
    {
        public abstract class PackerBinary : Packer, IPacker
        {
            protected virtual byte Version { get; }
            protected byte UnpackedVersion { get; private set; } = 0;
            protected virtual System.Text.Encoding Encoding => System.Text.Encoding.ASCII;
            public bool ReachedEndOfSaveFileData(BinaryReader reader) => reader.BaseStream.Position >= (reader.BaseStream.Length - 64 - 1); // HASH 64 - 1 pos
            protected readonly StringBuilder packer;
            protected readonly StringBuilder unpacker;

            public PackerBinary() : base()
            {
                packer = new StringBuilder();
                unpacker = new StringBuilder();
            }

            public virtual void Save()
            {
                using (FileStream stream = File.Create(GetDataPath(FileNameWithExtension)))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, Encoding))
                    {
                        writer.Write(Version);
                        PackData(writer);
                        writer.Write(GetHash(packer.ToString()));
                        packer.Clear();
                    }
                }
            }

            public virtual IEnumerator SaveAsync()
            {
                using (FileStream stream = File.Create(GetDataPath(FileNameWithExtension)))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, Encoding))
                    {
                        writer.Write(Version);

                        string data = string.Empty;
                        yield return PackDataAsync(writer);
                        writer.Write(GetHash(unpacker.ToString()));

                        packer.Clear();
                    }
                }
            }

            public virtual void Load()
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

                unpacker.Clear();
            }

            public virtual IEnumerator LoadAsync()
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

                unpacker.Clear();
            }

            protected bool ValidateHash(BinaryReader reader)
            {
                string fileHash = reader.ReadString();
                string decodedHash = GetHash(unpacker.ToString());
                return fileHash.Equals(decodedHash);
            }

            protected virtual void PackData(BinaryWriter writer)
            {
                throw new System.NotImplementedException();
            }

            protected virtual void UnpackData(BinaryReader reader)
            {
                throw new System.NotImplementedException();
            }

            protected virtual IEnumerator PackDataAsync(BinaryWriter writer)
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
            public void Pack(BinaryWriter writer, bool data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, string data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, float data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, short data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, int data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, long data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, sbyte data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, byte data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, ushort data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, uint data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, ulong data)
            {
                writer.Write(data);
                this.packer.Append(data);
            }

            public void Pack(BinaryWriter writer, Vector2 data)
            {
                Pack(writer, data.x);
                Pack(writer, data.y);
            }

            public void Pack(BinaryWriter writer, Vector3 data)
            {
                Pack(writer, data.x);
                Pack(writer, data.y);
                Pack(writer, data.z);
            }

            public void Pack(BinaryWriter writer, Vector4 data)
            {
                Pack(writer, data.x);
                Pack(writer, data.y);
                Pack(writer, data.z);
                Pack(writer, data.w);
            }

            public void Pack(BinaryWriter writer, Quaternion data)
            {
                Pack(writer, data.x);
                Pack(writer, data.y);
                Pack(writer, data.z);
                Pack(writer, data.w);
            }

            #endregion

            #region Unpackers
            public bool UnpackBool(BinaryReader reader)
            {
                bool data = reader.ReadBoolean();
                this.unpacker.Append(data);
                return data;
            }

            public string UnpackString(BinaryReader reader)
            {
                string data = reader.ReadString();
                this.unpacker.Append(data);
                return data;
            }

            public float UnpackFloat(BinaryReader reader)
            {
                float data = reader.ReadSingle();
                this.unpacker.Append(data);
                return data;
            }

            public short UnpackShort(BinaryReader reader)
            {
                short data = reader.ReadInt16();
                this.unpacker.Append(data);
                return data;
            }

            public int UnpackInt(BinaryReader reader)
            {
                int data = reader.ReadInt32();
                this.unpacker.Append(data);
                return data;
            }

            public long UnpackLong(BinaryReader reader)
            {
                long data = reader.ReadInt64();
                this.unpacker.Append(data);
                return data;
            }

            public sbyte UnpackSByte(BinaryReader reader)
            {
                sbyte data = reader.ReadSByte();
                this.unpacker.Append(data);
                return data;
            }

            public byte UnpackByte(BinaryReader reader)
            {
                byte data = reader.ReadByte();
                this.unpacker.Append(data);
                return data;
            }

            public ushort UnpackUShort(BinaryReader reader)
            {
                ushort data = reader.ReadUInt16();
                this.unpacker.Append(data);
                return data;
            }

            public uint UnpackUInt(BinaryReader reader)
            {
                uint data = reader.ReadUInt32();
                this.unpacker.Append(data);
                return data;
            }

            public ulong UnpackULong(BinaryReader reader)
            {
                ulong data = reader.ReadUInt64();
                this.unpacker.Append(data);
                return data;
            }

            public Vector2 UnpackVector2(BinaryReader reader)
            {
                float x = UnpackFloat(reader);
                float y = UnpackFloat(reader);

                return new Vector2(x, y);
            }

            public Vector3 UnpackVector3(BinaryReader reader)
            {
                float x = UnpackFloat(reader);
                float y = UnpackFloat(reader);
                float z = UnpackFloat(reader);

                return new Vector3(x, y, z);
            }

            public Vector4 UnpackVector4(BinaryReader reader)
            {
                float x = UnpackFloat(reader);
                float y = UnpackFloat(reader);
                float z = UnpackFloat(reader);
                float w = UnpackFloat(reader);

                return new Vector4(x, y, z, w);
            }

            public Quaternion UnpackQuaternion(BinaryReader reader)
            {
                float x = UnpackFloat(reader);
                float y = UnpackFloat(reader);
                float z = UnpackFloat(reader);
                float w = UnpackFloat(reader);

                return new Quaternion(x, y, z, w);
            }

            #endregion
        }
    }
}