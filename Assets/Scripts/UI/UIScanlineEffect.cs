using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UIScanlineEffect : MonoBehaviour
{
    [SerializeField] private Material scanlineMaterial;
    [SerializeField] private float density = 400f;
    [SerializeField, Range(0f, 1f)] private float intensity = 0.6f;

    private RawImage image;

    void Awake()
    {
        image = GetComponent<RawImage>();
        image.material = new Material(scanlineMaterial);
    }

    void Update()
    {
        if (image != null && image.material != null)
        {
            image.material.SetFloat("_ScanlineDensity", density);
            image.material.SetFloat("_ScanlineIntensity", intensity);
        }
    }
}
