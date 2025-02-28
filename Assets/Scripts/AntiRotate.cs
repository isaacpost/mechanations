using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that makes a gameobject always appear face up to the camera
public class AntiRotate : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
