using UnityEngine;
using TMPro;

// Class that interacts with the Ammo amount UI element
public class AmmoDisplay : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI ammoText; // Reference to the UI Text element

    private int currentAmmo = 3; // Example ammo count
    private int maxAmmo = 3; // Max ammo count

    // Start is called before the first frame update
    void Start()
    {
        UpdateAmmoDisplay();
    }

    // Sets the text to the Ammo Amount object
    void UpdateAmmoDisplay()
    {
        ammoText.text = "Ammo\n" + currentAmmo + "/" + maxAmmo;
    }

    // Decreases ammo by a certain amount and updates UI
    public void DecreaseAmmo(int amount)
    {
        currentAmmo = Mathf.Max(0, currentAmmo - amount); // Prevent going below 0
        UpdateAmmoDisplay();
    }

    // Resets the Ammo amount on the display to the max
    public void ResetAmmo()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay();
    }
}
