using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CRTScanlineEffect : MonoBehaviour
{
    [SerializeField]
    private Material scanlineMaterial;

    private readonly float scanlineDensity = 250f;

    private readonly float scanlineIntensity = 0.15f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (scanlineMaterial != null)
        {
            // Pass parameters to the shader
            scanlineMaterial.SetFloat("_ScanlineDensity", scanlineDensity);
            scanlineMaterial.SetFloat("_ScanlineIntensity", scanlineIntensity);

            // Apply the shader effect
            Graphics.Blit(source, destination, scanlineMaterial);
        }
        else
        {
            // If no material is set, just copy the source to destination
            Graphics.Blit(source, destination);
        }
    }
}
