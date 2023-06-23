using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator anim;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce; 

    private bool canDoubleJump;

    private bool playerUnlocked;
    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    private float slideCooldownCounter;
    private float slideTimeCounter;
    private bool isSliding;

    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceillingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;
    private bool wallDetected;
    private bool ceillingDetected;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        AnimatorControllers();

        slideTimeCounter-=Time.deltaTime;
        slideCooldownCounter-=Time.deltaTime;

        if (playerUnlocked)
            Movement();

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        CheckForSlide();
        CheckInput();
    }

    private void CheckForSlide()
    {
        if(slideTimeCounter < 0 && !ceillingDetected)
            isSliding = false;
    }

    private void Movement()
    {
        if (wallDetected)
            return;

        if(isSliding)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }




    private void SlideButton()
    {
        if (rb.velocity.x!=0 && slideCooldownCounter <0)
        {
            isSliding = true;
            slideTimeCounter = slideTime;
            slideCooldownCounter = slideCooldown;
        }
    }

    private void JumpButton()
    {
        if(isSliding)
            return;

        if (isGrounded)
        {

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
        }
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Fire2"))
            playerUnlocked = true;

        if (Input.GetButtonDown("Jump"))
            JumpButton();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();
    }

    private void AnimatorControllers()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetFloat("xVelocity", rb.velocity.x);
        
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
    }
    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        ceillingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceillingCheckDistance, whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize,0,Vector2.zero,0,whatIsGround);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceillingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
