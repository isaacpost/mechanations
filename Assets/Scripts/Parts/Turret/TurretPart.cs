using UnityEngine;

// Logic behind turret part
public class TurretPart : Part
{
    [SerializeField] 
    private GameObject projectilePrefab; // Yellow projectile

    [SerializeField] 
    private float shootInterval = 1f; // How often it shoots

    [SerializeField] 
    private string startHex = "#7E7E7E"; // The sprite color it starts as

    [SerializeField] 
    private string endHex = "#FFFFFF"; // The sprite color it ends as

    [SerializeField] 
    private GameObject explosionPrefab;

    private Color startColor;
    private Color endColor;
    private Color turretColor;

    private DashedLineDrawer dashedLine;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f; // Time elapsed since last shoot

    private void Start()
    {
        ColorUtility.TryParseHtmlString(startHex, out startColor);
        ColorUtility.TryParseHtmlString(endHex, out endColor);
        ColorUtility.TryParseHtmlString("#FDFF00", out turretColor);

        timer = shootInterval;

        dashedLine = GetComponent<DashedLineDrawer>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        dashedLine.SetActive(false);
    }

    void Update()
    {
        if (CanShoot())
        {
            dashedLine.SetActive(true);

            timer += Time.deltaTime;

            Color newColor = Color.Lerp(startColor, endColor, timer / shootInterval);

            spriteRenderer.color = newColor;
            dashedLine.SetColor(newColor * turretColor);

            if (timer >= shootInterval)
            {
                timer = 0f;

                SFXManager.Instance.PlaySound("TurretShoot");

                Shoot();
            }
        }
        else
        {
            spriteRenderer.color = startColor;
            dashedLine.SetColor(startColor * turretColor);
            dashedLine.SetActive(false);
            timer = 0f;
        }
    }

    public override void Rotate(float degrees)
    {
        transform.RotateAround(transform.position, Vector3.forward, degrees);
    }

    void Shoot()
    {
        Instantiate(projectilePrefab,transform.position, transform.rotation, transform);
    }

    private bool CanShoot()
    {
        return FindAdjacentGears().Count != 0;
    }
}
