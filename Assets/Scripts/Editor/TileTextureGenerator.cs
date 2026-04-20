using UnityEngine;
using UnityEditor;
using System.IO;

public static class TileTextureGenerator
{
    [MenuItem("TacticalBreach/CI/Generate Flat Textures")]
    public static void Generate()
    {
        GenerateFlat(1024, "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png", new Color32(150, 150, 150, 255), false, true);
        GenerateFlat(1024, "Assets/Resources/Graphics/Textures/Tex_Floor_Room_Dark.png", new Color32(170, 170, 170, 255), false, true);
        GenerateFlat(1024, "Assets/Resources/Graphics/Textures/Tex_Wall_Concrete_Dark.png", new Color32(10, 10, 10, 255), true);
        GenerateFlat(1024, "Assets/Resources/Graphics/Textures/Tex_Door_Dark.png", new Color32(180, 110, 30, 255), true);
        GenerateFlat(1024, "Assets/Resources/Graphics/Textures/Tex_Window_Dark.png", new Color32(60, 150, 220, 255), true);
        GenerateFlat(1024, "Assets/Resources/Graphics/Textures/Tex_Extract_Dark.png", new Color32(60, 180, 60, 255), true);
        Debug.Log("[CI] Flat textures generated.");
    }

    private static void GenerateFlat(int size, string path, Color32 color, bool drawBorder, bool drawGrid2x2 = false)
    {
        var tex = new Texture2D(size, size);
        var pixels = new Color32[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (drawBorder && (x < 16 || y < 16 || x >= size - 16 || y >= size - 16))
                {
                    pixels[y * size + x] = new Color32(0, 0, 0, 255);
                }
                else if (drawGrid2x2 && (x % 256 == 0 || x % 256 == 255 || y % 256 == 0 || y % 256 == 255))
                {
                    pixels[y * size + x] = new Color32(40, 40, 40, 255); // Dark grid line
                }
                else
                {
                    pixels[y * size + x] = color;
                }
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        
        string fullPath = Path.Combine(Application.dataPath, path.Substring(7));
        File.WriteAllBytes(fullPath, tex.EncodeToPNG());
    }
}
