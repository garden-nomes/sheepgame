using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteRandomizer))]
[CanEditMultipleObjects]
public class SpriteRandomizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Randomize"))
        {
            foreach (var target in targets)
            {
                (target as SpriteRandomizer).Randomize();
            }
        }
    }
}
