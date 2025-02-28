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

    private SpriteRenderer spriteRenderer;
    private float timer = 0f; // Time elapsed since last shoot

    private void Start()
    {
        ColorUtility.TryParseHtmlString(startHex, out startColor);
        ColorUtility.TryParseHtmlString(endHex, out endColor);

        timer = shootInterval;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (CanShoot())
        {

            timer += Time.deltaTime;

            spriteRenderer.color = Color.Lerp(startColor, endColor, timer / shootInterval);

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
        }
    }

    public override void Rotate(float degrees)
    {
        transform.RotateAround(transform.position, Vector3.forward, degrees);
    }

    void Shoot()
    {
        Instantiate(projectilePrefab,transform.position, transform.rotation);
    }

    private bool CanShoot()
    {
        return FindAdjacentGears().Count != 0;
    }
}
