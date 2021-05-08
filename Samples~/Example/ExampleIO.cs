using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sacristan.Ahhnold.IO.Example
{
    public class ExampleIO : MonoBehaviour
    {
        private void Start()
        {
            GameIO.SaveGameProcessor.cube = transform;
        }

        public void SaveCubePosition()
        {
            GameIO.SaveGameProcessor.Save();
        }

        public void LoadCubePosition()
        {
            GameIO.SaveGameProcessor.Load();
        }
    }

}
