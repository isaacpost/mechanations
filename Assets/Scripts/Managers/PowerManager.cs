using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{


    public int maxPower;
    public GearPart endGear;
    public Health powerHealth; // Players health
    public Health goalPowerHealth; // Target power fill health

    public static PowerManager Instance;

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

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            if (endGear.IsPowered())
            {
                goalPowerHealth.TakeDamage(-1);
            }
            else
            {
                powerHealth.TakeDamage(1);
                timer = 0f;   
            }
        }
    }

    public bool IsNotEmpty()
    {
        return powerHealth.GetCurrentHealth() > 0;
    }
}
