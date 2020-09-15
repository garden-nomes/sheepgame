using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeartSpawner))]
public class HeartSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Test"))
        {
            (target as HeartSpawner).ShowHearts(5);
        }
    }
}
