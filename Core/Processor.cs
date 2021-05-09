using System.Collections;

namespace Sacristan.Ahhnold.IO.Binary
{
    public static partial class SaveFile
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

    }
}

namespace Sacristan.Ahhnold.IO.Serialized
{
    public static partial class SaveFile
    {
        public abstract class Processor
        {
            public virtual Packer SaveFilePacker => null;
            public void Save(object data) => SaveFilePacker?.Save(data);
            public void Load<T>(ref object data) => SaveFilePacker?.Load<T>(ref data);
        }
    }
}