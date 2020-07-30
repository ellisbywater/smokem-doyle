using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletController : MonoBehaviour
{
    [FormerlySerializedAs("moveSpeed")] public float bulletSpeed = 18f;
    public float lifetime = 3f;
    public int damage = 1, headshotMultiplier = 2;
    public bool enemyBullet, playerBullet;

    public Rigidbody rigidBody;

    public GameObject impactEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        rigidBody.velocity = transform.forward * bulletSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && playerBullet)
        {
            
                other.gameObject.GetComponent<EnemyHealthController>().DamageEnemy(damage);
        }

        if (other.gameObject.CompareTag("Headshot") && playerBullet)
        {
            other.transform.parent.GetComponent<EnemyHealthController>().DamageEnemy(damage * headshotMultiplier);
            Debug.Log("Headshot!! damage: " + (damage * headshotMultiplier));
        }
        
        if (other.gameObject.CompareTag("Player") && enemyBullet)
        {
            Debug.Log("Hit Player at " + transform.position);
        }
        Destroy(gameObject);
        Instantiate(impactEffect, transform.position + (transform.forward * (-bulletSpeed * Time.deltaTime)), transform.rotation);
    }
}
