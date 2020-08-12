using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public string gunName;
    
    private bool collected = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            // give ammo
            PlayerController.instance.AddGun(gunName);
            
            Destroy(gameObject);
            collected = true;
        }
    }
}
