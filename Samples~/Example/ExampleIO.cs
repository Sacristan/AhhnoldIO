using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sacristan.Ahhnold.IO.Example
{
    public class ExampleIO : MonoBehaviour
    {
        public void SaveCubePosition()
        {
            GameIO.SaveGameProcessor.saveFilePosition = transform.position;
            GameIO.SaveGameProcessor.Save();
        }

        public void LoadCubePosition()
        {
            GameIO.SaveGameProcessor.Load();
            transform.position = GameIO.SaveGameProcessor.saveFilePosition;
        }
    }

}
