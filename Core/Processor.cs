using System.Collections;

namespace Sacristan.Ahhnold.IO
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