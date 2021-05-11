using System.IO;
using UnityEngine;

namespace Sacristan.Ahhnold.IO.Example
{
    public class ExampleJSON : MonoBehaviour
    {
        public class CubeData
        {
            [SerializeField] public Vector3 position;
        }

        internal CubeData data = new CubeData();

        public void Save()
        {
            data.position = transform.position;
            Serialized.JSON.SaveGameProcessor.Save(data);
        }

        public void Load()
        {
            if (Serialized.JSON.SaveGameProcessor.IsHashValid())
            {
                data = Serialized.JSON.SaveGameProcessor.Load<CubeData>();
            }
            else
            {
                data = default(CubeData);
            }

            transform.position = data.position;
        }
    }
}
