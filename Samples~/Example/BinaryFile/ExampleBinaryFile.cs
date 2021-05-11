using System.IO;
using UnityEngine;

namespace Sacristan.Ahhnold.IO.Example
{
    public class ExampleBinaryFile : MonoBehaviour
    {
        public void Save()
        {
            Binary.BinaryFile.SaveGameProcessor.cube = transform;
            Binary.BinaryFile.SaveGameProcessor.Save();
        }

        public void Load()
        {
            Binary.BinaryFile.SaveGameProcessor.Load();
        }
    }

}
