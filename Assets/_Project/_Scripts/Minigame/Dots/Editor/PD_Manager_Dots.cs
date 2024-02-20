using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Manager_Dots))]
public class PD_Manager_Dots : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var managerDots = (Manager_Dots) target;

        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        GUILayout.Label("Dots");

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear"))
            managerDots.ClearDots();

        if (GUILayout.Button("Create"))
            managerDots.CreateDots();

        GUILayout.EndHorizontal();
    }
    
}
