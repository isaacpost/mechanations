using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public int maxPower;

    public static PowerManager Instance;
    
    private Health powerHealth; // Players health
    private float timer = 0f;

    // Ensure the GameManager persists between scenes
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        powerHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            powerHealth.TakeDamage(1);
            timer = 0f;
        }
    }

    public bool IsNotEmpty()
    {
        return powerHealth.GetCurrentHealth() > 0;
    }
}
