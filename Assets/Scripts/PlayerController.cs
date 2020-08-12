using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float moveSpeed = 5f,
                 gravityModifier = 1,
                 jumpPower = 3,
                 runSpeed = 12;

    private bool sprinting = false;
    
    public CharacterController characterController;
    public Transform cameraTransform;

    private Vector3 moveInput;

    public float mouseSensitivity = 2f;
    public bool invertX;
    public bool invertY;

    private bool canJump, canDoubleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> unlockableGuns = new List<Gun>();
    public int currentGun;
    public Gun activeGun;
    public GameObject bullet;
    public Transform bulletOrigin;

    public Transform gunHolder;
    public float adsSpeed = 2f;
    
    // Animations
    public Animator anim;

    public void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        currentGun--;
        SwitchGun();
        
    }

    // Update is called once per frame
    void Update()
    {
        //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //moveInput.y = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        
        //store y velocity
        float yStore = moveInput.y;

        Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horzMove = transform.right * Input.GetAxis("Horizontal");

        moveInput = horzMove + vertMove;
        moveInput.Normalize();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            
            moveInput = moveInput * runSpeed;
        }
        else
        {
            sprinting = false;
            moveInput *= moveSpeed;
        }

        moveInput.y = yStore;
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (characterController.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;

        if (canJump)
        {
            canDoubleJump = false;
        }
        
        // Handle Jumping
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            moveInput.y = jumpPower;
        } else if (Input.GetKeyDown(KeyCode.Space) && canDoubleJump)
        {
            moveInput.y = jumpPower;
            canDoubleJump = false;
        }
        characterController.Move(moveInput * Time.deltaTime);
        
        // Control Camera Rotation
        Vector2  mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        if (invertX)
        {
            mouseInput.x = -mouseInput.x;
        }

        if (invertY)
        {
            mouseInput.y = -mouseInput.y;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x,transform.rotation.eulerAngles.z );
        cameraTransform.rotation = Quaternion.Euler(cameraTransform.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
        
        // Handles Shooting
        //Single shot
        if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 50f))
            {
                if (Vector3.Distance(cameraTransform.position, hit.point) > 2f)
                {
                    bulletOrigin.LookAt(hit.point);
                }
                
            }
            else
            {
                bulletOrigin.LookAt(cameraTransform.position + (cameraTransform.forward * 30f));
            }
            FireShot();
        }
        
        // automatic firing
        if (Input.GetMouseButton(0) && activeGun.automaticWeapon)
        {
            if (activeGun.fireCounter <= 0)
            {
                FireShot();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchGun();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CameraController.instance.ZoomIn(activeGun.zoomAmount);
        }
        if (Input.GetMouseButtonUp(1))
        {
            CameraController.instance.ZoomOut();
            
        }
        
        // Set animations
        anim.SetFloat("moveSpeed", moveInput.magnitude);
        anim.SetBool("onGround", canJump);
        
    }

    public void FireShot()
    {
        if (activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;
            Instantiate(activeGun.bullet, bulletOrigin.position, bulletOrigin.rotation);
            activeGun.fireCounter = activeGun.fireRate;
        }
    }

    public void SwitchGun()
    {
        if(activeGun)
            activeGun.gameObject.SetActive(false);
        currentGun++;
        Debug.Log("Current Gun Index: " + currentGun);
        if (currentGun >= allGuns.Count)
        {
            currentGun = 0;
        }
        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);
        
        Debug.Log("Active Gun: " + activeGun);

        bulletOrigin.position = activeGun.bulletOrigin.position;
    }

    public void AddGun(string gunName)
    {
        bool gunUnlocked = false;

        if (unlockableGuns.Count > 0)
        {
            for (int i = 0; i < unlockableGuns.Count; i++)
            {
                if (unlockableGuns[i].gunName == gunName)
                {
                    gunUnlocked = true;
                    allGuns.Add(unlockableGuns[i]);
                    unlockableGuns.RemoveAt(i);

                    i = unlockableGuns.Count;
                }
            }

            if (gunUnlocked)
            {
                currentGun = allGuns.Count - 2;
                SwitchGun();
            }
        }
    }
}
