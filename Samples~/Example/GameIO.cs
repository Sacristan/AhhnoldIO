using UnityEngine;
using System.IO;
using Sacristan.Ahhnold.IO;
using System.Text;

namespace Sacristan.Ahhnold.IO.Example
{
    public static class GameIO
    {
        private class Packer : SaveFile.Packer
        {
            protected override byte Version => 1;
            protected override string Salt => "Y7fA7nEjt0Jl30M";
            protected override string FileName => "cube";
            protected override string Extension => ".sav";

            protected override string PackData(BinaryWriter writer)
            {
                StringBuilder str = new StringBuilder();

                Vector3 pos = SaveGameProcessor.saveFilePosition;

                Pack(writer, str, pos.x);
                Pack(writer, str, pos.y);
                Pack(writer, str, pos.z);

                return str.ToString();
            }

            protected override void UnpackData(BinaryReader reader)
            {
                StringBuilder str = new StringBuilder();

                Vector3 pos = new Vector3(
                    UnpackFloat(reader, str), //UNPACK X
                    UnpackFloat(reader, str), //UNPACK Y 
                    UnpackFloat(reader, str) //UNPACK Z
                );

                if (ValidateHash(reader, str.ToString())) //VALIDATES WHETHER LOADED DATA MATCHES SAVED DATA HASH 
                {
                    SaveGameProcessor.saveFilePosition = pos;
                }
                else HandleCorruptedFile();
            }
        }


        public class Processor : SaveFile.Processor
        {
            Packer _packer;

            public Vector3 saveFilePosition;

            public override SaveFile.Packer SaveFilePacker
            {
                get
                {
                    if (_packer == null) _packer = new Packer();
                    return _packer;
                }
            }

            public override void Save()
            {
                SaveFilePacker.Save();
            }

            public override void Load()
            {
                SaveFilePacker.Load();
            }

            public override void Reset()
            {
                SaveFilePacker.Delete();
            }

        }

        public static Processor SaveGameProcessor = new Processor();
    }
}