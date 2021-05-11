namespace Sacristan.Ahhnold.IO.Example.Serialized
{
    public static class JSON
    {
        private class Packer : Sacristan.Ahhnold.IO.Serialized.SaveFile.Packer
        {
            protected override string Salt => "Y7fA7nEjt0Jl30M";
            protected override string FileName => "cube";
            protected override string Extension => ".json";
            public override SerializationType SaveFileSerializationType { get; set; } = SerializationType.JSON;
        }

        public class Processor : Sacristan.Ahhnold.IO.Serialized.SaveFile.Processor
        {
            Packer _packer;

            public override Sacristan.Ahhnold.IO.Serialized.SaveFile.Packer SaveFilePacker
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
