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

            protected override void PackData(BinaryWriter writer)
            {
                Vector3 pos = SaveGameProcessor.cube.transform.position;
                Pack(writer, pos);
            }

            protected override void UnpackData(BinaryReader reader)
            {
                Vector3 pos = UnpackVector3(reader);

                if (ValidateHash(reader)) //VALIDATES WHETHER LOADED DATA MATCHES SAVED DATA HASH 
                {
                    SaveGameProcessor.cube.transform.position = pos;
                }
                else HandleCorruptedFile();
            }
        }


        public class Processor : SaveFile.Processor
        {
            Packer _packer;

            public Transform cube;

            public override SaveFile.Packer SaveFilePacker
            {
                get
                {
                    if (_packer == null) _packer = new Packer();
                    return _packer;
                }
            }
        }

        public static Processor SaveGameProcessor = new Processor();
    }
}