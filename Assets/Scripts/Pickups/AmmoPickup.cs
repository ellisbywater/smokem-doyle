using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private bool collected = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            // give ammo
            PlayerController.instance.activeGun.GetAmmo();
            
            Destroy(gameObject);
            collected = true;
        }
    }
}
