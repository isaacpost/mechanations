using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Script to unlock the secret boss when players press the buttons
public class PartsButtonTracker : MonoBehaviour
{
    private Queue<string> queue = new Queue<string>(); // the recently pressed buttons
    private const int maxSize = 3; // Maximum size of the queue

    public void ButtonPressed(string buttonName)
    {
        SFXManager.Instance.PlaySound("PickUpPart");

        if (queue.Count >= maxSize)
        {
            queue.Dequeue(); // Remove the oldest item
        }

        queue.Enqueue(buttonName); // Add the new item

        CheckedIfCorrectSequence();
    }

    void CheckedIfCorrectSequence()
    {
        List<string> otherList = new List<string> { "AutoGear", "Turret", "Wall" };

        // Convert the queue to an array for comparison
        var queueArray = queue.ToArray();

        // Check if the lengths are equal before comparing
        if (queueArray.Length == otherList.Count && queueArray.SequenceEqual(otherList))
        {
            MenuManager.Instance.SwitchToBossFourScene();
        }
    }
}
