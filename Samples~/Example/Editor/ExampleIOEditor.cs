using UnityEditor;
using UnityEngine;

namespace Sacristan.Ahhnold.IO.Example
{
    [CustomEditor(typeof(ExampleIO))]
    public class ExampleIOEditor : Editor
    {
        ExampleIO t;
        private void OnEnable()
        {
            t = (ExampleIO)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Save Cube Pos")) t.SaveCubePosition();
            if (GUILayout.Button("Load Cube Pos")) t.LoadCubePosition();
        }
    }
}