using UnityEngine;
using UnityEngine.UI;

// Attached to a gameobject to keep track of its health
// Mainly used for bosses and the player
public class Health : MonoBehaviour
{
    [SerializeField] 
    private float maxHealth;

    [SerializeField] 
    private Slider healthBar; // Assign the UI slider in the Inspector.

    private float currentHealth;
    private IDamagable damageable; // Damageable script of the object the health is for

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Gets damageable component, usually a controller of some kind
        damageable = GetComponent<IDamagable>();
    }

    // Assign slider to taken damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        // Dies if health is less than 0
        if (currentHealth <= 0)
        {
            damageable.Died();
        }
    }

    // Sets the value of the bar based on ratio
    void UpdateHealthBar()
    {
        healthBar.value = currentHealth / maxHealth;
    }

    // Calculates a scale factor for bosses to use to become more difficult depending on health
    public float CalculateScaleFactor()
    {
        return (maxHealth - currentHealth) / maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TurnOnHealthBar()
    {
        healthBar.gameObject.SetActive(true);
    }
}
