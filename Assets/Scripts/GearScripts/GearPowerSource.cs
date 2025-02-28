using UnityEngine;

// Logic for a gear that is always powered and not a part that can be picked up
public class GearPowerSource : MonoBehaviour, IGear
{
    public bool IsPowered()
    {
        return true;
    }
}
