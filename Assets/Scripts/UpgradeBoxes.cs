using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBoxes : MonoBehaviour
{
    private bool upgraded = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !upgraded)
        {
            Debug.Log("Upgrading");
            upgraded = true;
            GameManager.instance.AddTime(.5f);
            GameManager.instance.GetUpgrade();
            Destroy(gameObject, 5);
        }
    }
}
