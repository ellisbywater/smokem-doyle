using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject bullet;
    public Transform bulletOrigin;
    
    
    // Animations
    public Animator anim;

    public void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
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
        
        // Shooting
        if (Input.GetMouseButtonDown(0))
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
            Instantiate(bullet, bulletOrigin.position, bulletOrigin.rotation);
        }
        
        // Set animations
        anim.SetFloat("moveSpeed", moveInput.magnitude);
        anim.SetBool("onGround", canJump);
        
    }
}
