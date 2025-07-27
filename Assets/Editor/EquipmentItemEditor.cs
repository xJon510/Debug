// Assets/Editor/EquipmentItemEditor.cs
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EquipmentItem))]
public class EquipmentItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EquipmentItem item = (EquipmentItem)target;

        if (GUILayout.Button("Roll Stats (Debug)"))
        {
            var rollMethod = typeof(EquipmentItem).GetMethod("RollStats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            rollMethod.Invoke(item, null);
            EditorUtility.SetDirty(item); // Mark it dirty so Unity saves the changes
        }
    }
}
