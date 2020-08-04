using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    
    private bool _chasing;
    private Vector3 _targetPoint, _startPoint;
    private float _chaseCounter;

    [Header("Behavior Settings")] 
    public float distanceToChase = 10f;
    public float distanceToLose = 13f;
    public float distanceToStop = 2f;
    [FormerlySerializedAs("Agent")] public NavMeshAgent agent;
    public float keepChasingTime;
    
    [Header("Weapon Settings")]
    public GameObject bullet;
    public Transform firePoint;
    public float bullseye = 1.2f;
    public int bulletDamage = 1;
    public int headshotDamageMultiplier = 2;
    
    public float shotAngle = 30;
    public float fireRate, shotGap, shotTime = 1f;
    private float _fireCount, _shotGapCounter, _shotTimeCounter;

    [Header("Animation Settings")] public new Animator animation;
    
    // Start is called before the first frame update
    void Start()
    {
        _startPoint = transform.position;
        _shotTimeCounter = shotTime;
        bullet.GetComponent<BulletController>().damage = bulletDamage;
        bullet.GetComponent<BulletController>().headshotMultiplier = headshotDamageMultiplier;

    }

    // Update is called once per frame
    void Update()
    {
        _targetPoint = PlayerController.instance.transform.position;
        _targetPoint.y = transform.position.y;
        if (!_chasing)
        {
            if (Vector3.Distance(transform.position, _targetPoint) < distanceToChase)
            {
                _chasing = true;
                _shotTimeCounter = shotTime;
                _shotGapCounter = shotGap;
            }

            if (_chaseCounter > 0)
            {
                _chaseCounter -= Time.deltaTime;
                if (_chaseCounter <= 0)
                {
                    agent.destination = _startPoint;
                }
            }

            if (agent.remainingDistance < 0.25f)
            {
                animation.SetBool("moving", false);
            }
            else
            {
                animation.SetBool("moving", true);
            }

        }
        else
        {
            /*transform.LookAt(targetPoint);
            rigidBody.velocity = transform.forward * moveSpeed;*/
            if (Vector3.Distance(transform.position, _targetPoint) > distanceToStop)
            {
                agent.destination = _targetPoint;  
            }
            else
            {
                agent.destination = transform.position;
            }
              
            if (Vector3.Distance(transform.position, _targetPoint) > distanceToLose)
            {
                _chasing = false;
                _chaseCounter = keepChasingTime;
            }

            if (_shotGapCounter > 0)
            {
                
                _shotGapCounter -= Time.deltaTime;

                if (_shotGapCounter <= 0)
                {
                    _shotTimeCounter = shotTime;
                }
                
                animation.SetBool("moving", true);
                
            }
            else
            {
                if (PlayerController.instance.gameObject.activeInHierarchy)
                {
                    _shotTimeCounter -= Time.deltaTime;
               
                
                    if (_shotTimeCounter > 0)
                    {
                        _fireCount -= Time.deltaTime;
            
                        if (_fireCount <= 0)
                        {
                            _fireCount = fireRate;
                        
                            firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0f, bullseye, 0f));
                            // check the angle to the player
                            Vector3 targetDirection = PlayerController.instance.transform.position - transform.position;
                            float angle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

                            if (Mathf.Abs(angle) < shotAngle)
                            {
                                Instantiate(bullet, firePoint.position, firePoint.rotation);
                                animation.SetTrigger("fireShot");
                            }
                            else
                            {
                                _shotTimeCounter = shotTime;
                            }
                        
                        
                        }

                        agent.destination = transform.position;
                    } else
                    {
                        _shotGapCounter = shotGap;
                    }
                }
                animation.SetBool("moving", false);    
                
            }
        }
        
    }
}
