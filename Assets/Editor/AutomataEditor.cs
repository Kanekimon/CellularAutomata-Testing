using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellularAutomateEngine))]
public class AutomataEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CellularAutomateEngine caEngine = (CellularAutomateEngine)target;

        if(GUILayout.Button("Re-Create Map"))
        {
            caEngine.GenerateGrid();
        }

        if (GUILayout.Button("Tick"))
        {
            caEngine.Tick();
        }

    }

}
