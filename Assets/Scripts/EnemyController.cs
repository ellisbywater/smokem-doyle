using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Behavior Settings")]
    private bool chasing;
    public float distanceToChase = 10f, distanceToLose = 13f, distanceToStop = 2f;

    private Vector3 targetPoint, startPoint;

    public NavMeshAgent Agent;

    public float keepChasingTime;
    private float chaseCounter;
    
    [Header("Bullet/Shooting Settings")]
    public GameObject bullet;
    public Transform firePoint;
    public float bullseye = 1.2f;
    
    public float shotAngle = 30;
    public float fireRate, shotGap, shotTime = 1f;
    private float _fireCount, _shotGapCounter, _shotTimeCounter;

    [Header("Animation Settings")] public Animator animation;
    
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
        _shotTimeCounter = shotTime;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;
        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                chasing = true;
                _shotTimeCounter = shotTime;
                _shotGapCounter = shotGap;
            }

            if (chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;
                if (chaseCounter <= 0)
                {
                    Agent.destination = startPoint;
                }
            }

            if (Agent.remainingDistance < 0.25f)
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
            if (Vector3.Distance(transform.position, targetPoint) > distanceToStop)
            {
                Agent.destination = targetPoint;  
            }
            else
            {
                Agent.destination = transform.position;
            }
              
            if (Vector3.Distance(transform.position, targetPoint) > distanceToLose)
            {
                chasing = false;
                chaseCounter = keepChasingTime;
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

                    Agent.destination = transform.position;
                } else
                {
                    _shotGapCounter = shotGap;
                }
                animation.SetBool("moving", false);
            }
        }
        
    }
}
