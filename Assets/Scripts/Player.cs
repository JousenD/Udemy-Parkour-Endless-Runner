
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isDead;
    [HideInInspector] public bool playerUnlocked;
    [HideInInspector] public bool extraLife;

    [Header("VFX")]
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private ParticleSystem bloodFx;

    [Header("Knockback info")]
    [SerializeField] private Vector2 knockbackDir;
    private bool isKnocked;
    private bool canBeKnocked = true;

    [Header("Move Info")]
    [SerializeField] private float speedToSurvive = 18;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMultiplier;
    private float defaultSpeed;
    [Space]
    [SerializeField] private float milestoneIncreaser;
    private float defaultMilestoneIncrease;
    private float speedMilestone;

    private bool readyToLand;

    [Header("Jump Info")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce; 
    private bool canDoubleJump;


    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    [HideInInspector] public float slideCooldownCounter;
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

    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncrease = milestoneIncreaser;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        AnimatorControllers();


        slideTimeCounter -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        extraLife = moveSpeed >= speedToSurvive;

        if (Input.GetKeyDown(KeyCode.K))
            Knockback();

        if (Input.GetKeyDown(KeyCode.O) && !isDead)
            StartCoroutine(Die());

        if (isDead)
            return;

        if (isKnocked)
            return;

        if (playerUnlocked)
            SetupMovement();

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        SpeedController();
        CheckForLanding();

        CheckForLedge();
        CheckForSlideCancel();
        CheckInput();
    }

    private void CheckForLanding()
    {
        if (rb.velocity.y < -5 && !isGrounded)
            readyToLand = true;

        if (readyToLand && isGrounded)
        {
            dustFx.Play();
            readyToLand = false;
        }
    }

    public void Damage()
    {
        bloodFx.Play();

        if (extraLife)
            Knockback();
        else
        {
            StartCoroutine(Die());        
        }
    }

    private IEnumerator Die()
    {
        AudioManager.instance.PlaySFX(3);
        isDead = true;
        canBeKnocked = false;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        //Slow motion activated
        Time.timeScale = 0.6f;

        yield return new WaitForSeconds(0.5f);

        Time.timeScale = 1f;
        rb.velocity = new Vector2(0, 0);
        GameManager.instance.GameEnded();
    }

    private IEnumerator Invincivility()
    {
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);

        canBeKnocked = false;
        sr.color = darkenColor;
        yield return new WaitForSeconds(0.1f);
        
        sr.color = originalColor;
        yield return new WaitForSeconds(0.1f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(0.15f);
        
        sr.color = originalColor;
        yield return new WaitForSeconds(0.15f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(0.25f);
        
        sr.color = originalColor;
        yield return new WaitForSeconds(0.25f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(0.3f);

        sr.color = originalColor;
        yield return new WaitForSeconds(0.35f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(0.4f);
        
        sr.color = originalColor;
        canBeKnocked = true;
    }
    
    #region KnockBack
    private void Knockback()
    {
        if(!canBeKnocked)
            return;

        StartCoroutine(Invincivility());
        isKnocked = true;
        rb.velocity = knockbackDir;
    }

    private void CancelKnockback() => isKnocked = false;
    #endregion

    #region SpeedControll
    private void SpeedReset()
    {
        if(isSliding)
            return;

        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaultMilestoneIncrease;
    }
    
    private void SpeedController()
    {
        if(moveSpeed == maxSpeed)
            return;
        
        if(transform.position.x > speedMilestone)
        {
            speedMilestone = speedMilestone + milestoneIncreaser;

            moveSpeed = moveSpeed*speedMultiplier;
            milestoneIncreaser = milestoneIncreaser * speedMultiplier;

            if(moveSpeed > maxSpeed)
                moveSpeed = maxSpeed;
        }
    }
    #endregion

    #region LedgeRegion
    private void CheckForLedge()
    {
        if(isSliding)
            return;
        if(ledgeDetected && canGrabLedge)
        {
            canGrabLedge = false;
            rb.gravityScale = 0;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;

            canClimb = true;
        }

        if(canClimb)
            transform.position = climbBegunPosition;
    }

    private void LedgeClimbStart()
    {
        Debug.Log("triggered");
        canClimb = false;
    }
    private void LedgeClimbOver()
    {
        canClimb = false;
        rb.gravityScale = 5;
        transform.position = climbOverPosition;
        canGrabLedge = false;
        Invoke("AllowLedgeGrab", 1f);
        
    }

    private void AllowLedgeGrab() => canGrabLedge = true;
    #endregion

    private void TemporaryDeactivateClimb()
    {
        canGrabLedge = false;
        Invoke("AllowLedgeGrab", 1f);
    }

    private void CheckForSlideCancel()
    {
        if(slideTimeCounter < 0 && !ceillingDetected)
            isSliding = false;
    }

    /* private void SetCanGrabLedgeFalse()
    {
        canGrabLedge = false;
    }

    private void SetCanGrabLedgeTrue()
    {
        canGrabLedge = true;
    } */
    private void SetupMovement()
    {
        if (wallDetected)
        {
            SpeedReset();
            return;
        }

        if(isSliding)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }



    #region Inputs
    public void SlideButton()
    {
        if (isDead)
            return;

        if (rb.velocity.x!=0 && slideCooldownCounter <0)
        {
            dustFx.Play();
            isSliding = true;
            slideTimeCounter = slideTime;
            slideCooldownCounter = slideCooldown;
        }
    }

    public void JumpButton()
    {
        if(isSliding || isDead)
            return;

        if (isGrounded)
        {
            Jump(jumpForce);
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            Jump(doubleJumpForce); 
        }
    }

    private void Jump(float force)
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(Random.Range(1,2));
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    private void CheckInput()
    {
        // if (Input.GetButtonDown("Fire2"))
        //     playerUnlocked = true;

        if (Input.GetButtonDown("Jump"))
            JumpButton();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();
    }
    #endregion

    #region Animations
    private void AnimatorControllers()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetFloat("xVelocity", rb.velocity.x);
        
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);
        anim.SetBool("isKnocked", isKnocked);

        if (rb.velocity.y < -20)
            anim.SetBool("canRoll", true);
    }

    private void RollAnimFinished() => anim.SetBool("canRoll",false);
    #endregion

    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        ceillingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceillingCheckDistance, whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize,0,Vector2.zero,0,whatIsGround);

        //Debug.Log(ledgeDetected);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceillingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
