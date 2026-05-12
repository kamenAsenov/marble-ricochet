#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class RicochetPopSceneBuilder
{
    [MenuItem("Tools/Ricochet Pop/OLD - Disabled")]
    public static void OldBuilderDisabled()
    {
        EditorUtility.DisplayDialog(
            "Old Builder Disabled",
            "Ricochet Pop was replaced by Marble Ricochet.\nUse Marble Ricochet tools only.",
            "OK"
        );

        Debug.Log("Old Ricochet Pop builder disabled.");
    }
}
#endif