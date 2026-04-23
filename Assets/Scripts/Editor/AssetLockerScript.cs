using UnityEditor;
using UnityEngine;

public static class AssetLockerScript
{
    [MenuItem("TacticalBreach/Lock Approved Assets")]
    public static void Lock()
    {
        string folderPath = "Assets/Resources/Graphics/Approved";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources/Graphics", "Approved");
        }

        string doorAsset = "Assets/Resources/Graphics/Textures/Tex_Door_Tactical.png";
        if (System.IO.File.Exists(doorAsset))
        {
            AssetDatabase.MoveAsset(doorAsset, folderPath + "/Tex_Door_Tactical.png");
            Debug.Log("Moved Door Texture to Approved folder.");
        }
    }
}
