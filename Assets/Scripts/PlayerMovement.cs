using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;

    
    
    //Movement
    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    //Double Jump 
    [SerializeField] bool canDoubleJump;
    bool doubleJump;

    //Animm
    private enum MovementState { idle, running, jumping, falling }



    [SerializeField] private AudioSource jumpSoundEffect;

    //Dash
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashingPower =64f;
    [SerializeField] private float dashingCoolDown = 1f;
    [SerializeField] private float dashingTime = 0.2f;

    //Gliding 
    [SerializeField] private bool canGlide;
    [SerializeField] private bool isGliding = false;
    [SerializeField] float glidingSpeed;
    private float initGravityScale;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        this.initGravityScale = rb.gravityScale; 
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDashing) return;


        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        
        if(!IsGrounded() && isGliding && rb.velocity.y < 0.1f)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2 (rb.velocity.x, -glidingSpeed);
        } else
        {
            rb.gravityScale = initGravityScale;
        }

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded())
            {
                doubleJump = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (canDoubleJump && doubleJump)
            {
                doubleJump = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            //jumpSoundEffect.Play();
        }
        
        //Gliding 
        if (Input.GetKeyDown(KeyCode.F) && canGlide)
        {
            isGliding = !isGliding;
        }
        if (IsGrounded())
        {
            isGliding = false;
        }

        //Dash
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }



        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale= 0f;
        rb.velocity = new Vector2(dirX * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }
}