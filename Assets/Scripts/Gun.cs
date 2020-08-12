using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletOrigin;

    public bool automaticWeapon;
    public float fireRate;
    public int currentAmmo, pickupAmmount, lowAmmo;
    public float zoomAmount;

    public string gunName;

    public Color lowAmmoColor = new Color(236f, 99f, 109f, 255f);
    
    [HideInInspector]
    public float fireCounter;
    
    // Start is called before the first frame update
    void Start()
    {
        UiUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;    
        }
        UiUpdate();
    }

    private void UiUpdate()
    {
        UIController.instance.ammoText.text = "" + currentAmmo + "";
        if (currentAmmo <= lowAmmo)
        {
            UIController.instance.ammoText.color = lowAmmoColor;
        }
    }

    public void GetAmmo()
    {
        currentAmmo += pickupAmmount;
    }
}
