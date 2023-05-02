using System;
using UnityEngine;

public class AddFuel : MonoBehaviour
{
    private bool fuelAdded = false;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("boop");
        if (other.gameObject.CompareTag("Player") && !fuelAdded)
        {
            Debug.Log("Adding fuel");
            fuelAdded = true;
            GameManager.instance.AddTime(.5f);
            GameManager.instance.AddFuel();
            Destroy(gameObject, 5);
        }
    }
}
    