
using System.Collections.Generic;
using UnityEngine;


// Script set on the Wall
public class WallPart : Part, IDamagable
{
    public override void Rotate(float degrees)
    {
        transform.RotateAround(transform.position, Vector3.forward, degrees);
    }
}
