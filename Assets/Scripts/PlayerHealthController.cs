﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;
    
    public int maxHealth = 50, currentHealth;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = " " + currentHealth + " / " + maxHealth + " ";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamagePlayer(int damageAmount)    
    {
        currentHealth -= damageAmount;
        UIController.instance.ShowDamage();
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            currentHealth = 0;
            GameManager.instance.PlayerDied();
        }
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = " " + currentHealth + " / " + maxHealth + " ";
    }

    public void HealPlayer(int healthAmount)
    {
        currentHealth += healthAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = " " + currentHealth + " / " + maxHealth + " ";
    }
}
