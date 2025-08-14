#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;

public class TMPSceneMaterialRestorer
{
    [MenuItem("Tools/TMP/Restore Materials In Scene")]
    public static void RestoreTMPMaterials()
    {
        var allTMP = Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        int fixedCount = 0;

        Undo.RecordObjects(allTMP, "Restore TMP Materials");

        foreach (var tmp in allTMP)
        {
            if (tmp == null || tmp.font == null)
                continue;

            // Force assign a fresh material from the font asset
            tmp.fontMaterial = tmp.font.material;
            tmp.SetMaterialDirty();
            tmp.SetVerticesDirty();

            fixedCount++;
        }

        Debug.Log($"Restored TMP materials on {fixedCount} objects.");
    }
}
#endif
