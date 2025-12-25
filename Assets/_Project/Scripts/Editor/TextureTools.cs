using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class TextureTools : EditorWindow
{
    [MenuItem("Asgard Foundry/Tools/Fix Icon Transparency (Circular Mask)")]
    public static void FixCircularMask()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                ApplyCircularMask(texture);
            }
        }
        AssetDatabase.Refresh();
    }

    private static void ApplyCircularMask(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null) return;

        bool wasReadable = importer.isReadable;
        if (!wasReadable || importer.textureCompression != TextureImporterCompression.Uncompressed)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.SaveAndReimport();
        }

        Texture2D loadedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        Texture2D rwTex = new Texture2D(loadedTex.width, loadedTex.height, TextureFormat.RGBA32, false);
        rwTex.SetPixels32(loadedTex.GetPixels32());
        rwTex.Apply();

        Color32[] pixels = rwTex.GetPixels32();
        int w = rwTex.width;
        int h = rwTex.height;
        float centerX = w / 2f;
        float centerY = h / 2f;
        // Ultra Aggressive cut (42%)
        float radiusSqr = (w * 0.42f) * (w * 0.42f);
        float featherRadiusSqr = (w * 0.45f) * (w * 0.45f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = x - centerX;
                float dy = y - centerY;
                float distSqr = dx*dx + dy*dy;

                // 1. Hard Cut: If strictly outside the circle, transparent
                if (distSqr > radiusSqr)
                {
                    pixels[y * w + x] = new Color32(0, 0, 0, 0);
                }
                // 2. Color Kill: If inside but near edge AND white, kill it
                // This prevents white halo pixels that survived the radius cut
                else if (distSqr > radiusSqr * 0.8f) // Check outer rim of the remaining circle
                {
                    Color32 c = pixels[y * w + x];
                    // If pixel is whitish (anti-aliasing residue)
                    if (c.r > 200 && c.g > 200 && c.b > 200)
                    {
                        pixels[y * w + x] = new Color32(0, 0, 0, 0);
                    }
                }
            }
        }

        rwTex.SetPixels32(pixels);
        rwTex.Apply();

        byte[] bytes = rwTex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        Debug.Log($"[TextureTools] Applied Circular Mask to {path}");
    }

    [MenuItem("Asgard Foundry/Tools/Fix Icon Transparency (Flood Fill)")]
    public static void FixTransparency()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                ProcessTexture(texture);
            }
        }
        AssetDatabase.Refresh();
    }

    private static void ProcessTexture(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null) return;

        // Force Uncompressed settings to ensure we can read/write data correctly
        bool settingsChanged = false;
        if (!importer.isReadable || importer.textureCompression != TextureImporterCompression.Uncompressed || importer.alphaSource != TextureImporterAlphaSource.FromInput)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.SaveAndReimport();
            settingsChanged = true;
        }

        // Reload texture to get the new settings
        Texture2D loadedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        
        // Create a copy to modify
        Texture2D rwTex = new Texture2D(loadedTex.width, loadedTex.height, TextureFormat.RGBA32, false);
        rwTex.SetPixels32(loadedTex.GetPixels32());
        rwTex.Apply();

        Color32[] pixels = rwTex.GetPixels32();
        int w = rwTex.width;
        int h = rwTex.height;

        // Flood Fill from (0,0) assuming top-left is background
        FloodFillTransparency(pixels, w, h, 0, 0);
        // Also try corners if they are disjoint
        FloodFillTransparency(pixels, w, h, w - 1, 0);
        FloodFillTransparency(pixels, w, h, 0, h - 1);
        FloodFillTransparency(pixels, w, h, w - 1, h - 1);

        rwTex.SetPixels32(pixels);
        rwTex.Apply();

        // Save back
        byte[] bytes = rwTex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        Debug.Log($"[TextureTools] Fixed transparency for {path} using Flood Fill");
    }

    private static void FloodFillTransparency(Color32[] pixels, int w, int h, int startX, int startY)
    {
        int startIdx = startY * w + startX;
        Color32 startColor = pixels[startIdx];

        // Only start if it looks like white background
        if (startColor.r < 230 || startColor.g < 230 || startColor.b < 230) return;

        Queue<int> queue = new Queue<int>();
        queue.Enqueue(startIdx);

        // Mark as transparent immediate to avoid loops? 
        // Better: use a visited array or check if already transparent
        
        while (queue.Count > 0)
        {
            int idx = queue.Dequeue();
            int x = idx % w;
            int y = idx / w;

            Color32 c = pixels[idx];
            if (c.a == 0) continue; // Already processed

            // Replace with transparent
            pixels[idx] = new Color32(0, 0, 0, 0);

            // Neighbors
            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < w && ny >= 0 && ny < h)
                {
                    int nIdx = ny * w + nx;
                    Color32 nc = pixels[nIdx];
                    
                    // Threshold check for white-ish
                    if (nc.a != 0 && nc.r > 200 && nc.g > 200 && nc.b > 200)
                    {
                        queue.Enqueue(nIdx);
                        // Mark transparent immediately to prevent re-enqueueing?
                        // Actually, let's just mark it transparent when dequeued, but we need to avoid adding duplicates.
                        // For simplicity, we can set alpha to 0 HERE, but we need strictly correct logic.
                        // Let's rely on checking c.a == 0 at start of loop. 
                        // But we might add neighbors multiple times. 
                        // For this simple script, it's okay, or use a HashSet.
                    }
                }
            }
        }
    }
}
