using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

[InitializeOnLoad]
public class PixelPerfectCameraEditor
{
    static PixelPerfectCameraEditor()
    {
        // This ensures that the "Run in Edit Mode" is always checked
        EditorApplication.hierarchyChanged += SetRunInEditMode;
    }

    private static void SetRunInEditMode()
    {
        // Find all PixelPerfectCamera components in the scene
        PixelPerfectCamera[] cameras = GameObject.FindObjectsOfType<PixelPerfectCamera>();

        foreach (PixelPerfectCamera camera in cameras)
        {
            // Set RunInEditMode to true
            if (!camera.runInEditMode)
            {
                camera.runInEditMode = true;
                EditorUtility.SetDirty(camera); // Marks the object as dirty to save changes
            }
        }
    }
}
