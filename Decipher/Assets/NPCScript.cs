using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class NPCScript : MonoBehaviour
{

    public CharacterController controller;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float smoothVelocity;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Vector3 cameraMovResult;
    private float targetAngle;
    private float angleDeg;
    private float smoothAngle;

    [Header("Jump")]
    [SerializeField] private float jumpHeight;


    [Header("Gravity")]
    [SerializeField] private bool gravityCheck;
    [SerializeField] private float gravityCheckDistance;
    [SerializeField] private float gravity;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] Vector3 velocity;



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveNPC();
    }


    private void moveNPC()
    {

        //gravityCheck = Physics.CheckSphere(transform.position, gravityCheckDistance, groundMask);
        gravityCheck = controller.isGrounded;

        if (gravityCheck && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        moveDirection = new Vector3(horizontal, 0, vertical);


        if (gravityCheck)
        {
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     playerJump();
            // }

            // if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
            // {
            //     //walk
            //     playerWalk(1f);
            //     cameraMovResult = Quaternion.Euler(0f, angleDeg, 0f) * Vector3.forward;

            // }
            // else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
            // {
            //     //run
            //     playerRun(1f);
            //     cameraMovResult = Quaternion.Euler(0f, angleDeg, 0f) * Vector3.forward;

            // }
            // else if (moveDirection == Vector3.zero)
            // {
            //     //idle
            //     playerIdle();
            // }

            // moveDirection *= moveSpeed;


        }
        else
        {
            // if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
            // {
            //     //walk
            //     playerWalk(0.6f);
            //     cameraMovResult = Quaternion.Euler(0f, angleDeg, 0f) * Vector3.forward;

            // }
            // else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
            // {
            //     //run
            //     playerRun(0.6f);
            //     cameraMovResult = Quaternion.Euler(0f, angleDeg, 0f) * Vector3.forward;

            // }
            // else if (moveDirection == Vector3.zero)
            // {
            //     //idle
            //     playerIdle();
            // }

        }

        moveDirection = cameraMovResult;
        moveDirection *= moveSpeed;

        velocity.y += gravity * Time.deltaTime;
        controller.Move((moveDirection + velocity) * Time.deltaTime);
    }

    private void playerRun(float multiplier)
    {
        moveSpeed = runSpeed * multiplier;
        anim.SetFloat("Blend", 2f, 0.15f, Time.deltaTime);
    }

    private void playerWalk(float multiplier)
    {
        moveSpeed = walkSpeed * multiplier;
        anim.SetFloat("Blend", 1f, 0.15f, Time.deltaTime);
    }

    private void playerIdle()
    {
        moveSpeed = 0;
        anim.SetFloat("Blend", 0f, 0.15f, Time.deltaTime);
    }

    private void playerJump()
    {
        anim.SetTrigger("Jump");
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

    }

}
