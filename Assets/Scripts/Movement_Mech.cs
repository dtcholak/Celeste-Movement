using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement_Mech : MonoBehaviour
{
    private Collision_Mech coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;

    [Space]
    [Header("Stats")]
    [SerializeField] public float maxSpeed = 10;
    [SerializeField] public float acceleration = 10;
    [SerializeField] public float deceleration = 10;
    [SerializeField] public float MaxJumpHeight = 1;
    [SerializeField] public float MinJumpHeight = 0.5f;
    [SerializeField] public float TimetoJumpApex = 0.4f;
    [SerializeField] public float TimetoJumpDrop = 0.3f;
    private float speed = 0;

    [Space]
    [Header("Booleans")]
    public bool groundTouch;
    public bool jumped;
    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem jumpParticle;

    float initialJumpVelocity_max;
    float initialJumpVelocity_min;
    float upGravity_max;
    float downGravity_max;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision_Mech>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();

        //Jump calculations for gravity
        /*
        initialJumpVelocity_max = 2 * MaxJumpHeight / TimetoJumpApex;
        
        upGravity_max = -2 * MaxJumpHeight / Mathf.Pow(TimetoJumpApex, 2.0f);
        downGravity_max = -2 * MaxJumpHeight / Mathf.Pow(TimetoJumpDrop, 2.0f);

        initialJumpVelocity_min = Mathf.Sqrt(2 * Mathf.Abs(upGravity_max * MinJumpHeight));
        */

        //Debug.Log(initialJumpVelocity_max);
        //Debug.Log(initialJumpVelocity_min);
        //Debug.Log(upGravity_max);
        //Debug.Log(downGravity_max);
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(xRaw, yRaw);

        initialJumpVelocity_max = 2 * MaxJumpHeight / TimetoJumpApex;
        
        upGravity_max = -2 * MaxJumpHeight / Mathf.Pow(TimetoJumpApex, 2.0f);
        downGravity_max = -2 * MaxJumpHeight / Mathf.Pow(TimetoJumpDrop, 2.0f);

        initialJumpVelocity_min = Mathf.Sqrt(2 * Mathf.Abs(upGravity_max * MinJumpHeight));

        Walk(dir);
        anim.SetHorizontalMovement(x, y, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && coll.onGround && !jumped)
        {
            Jump(initialJumpVelocity_max);
            anim.SetTrigger("jump");
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (rb.velocity.y > initialJumpVelocity_min)
            {
            rb.velocity = new Vector2(rb.velocity.x, initialJumpVelocity_min);
            jumped = false;
            }
        }

            Vector2 Velocity = rb.velocity;

        if(rb.velocity.y > 0)
        {
            Physics2D.gravity = new Vector2(0, upGravity_max);
            //Velocity.y += upGravity_max * Time.deltaTime;
            //rb.velocity = Velocity;
        }else
        {
            Physics2D.gravity = new Vector2(0, downGravity_max);
            //Velocity.y += downGravity_max * Time.deltaTime;
            //rb.velocity = Velocity;
        }

        

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
            
        }
        timer += Time.deltaTime;

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }
        if(coll.onGround && jumped)
        {
            timer += Time.deltaTime;
            
        }
        if(timer > 1.4)
        {
            jumped = false;
            timer = 0;
        }
        //Debug.Log(timer);
        if(x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }


    }

    void GroundTouch()
    {

        side = anim.sr.flipX ? -1 : 1;
        
        jumpParticle.Play();
    }



    private void Walk(Vector2 dir)
    {
        if(coll.onLeftWall && dir.x <= 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            speed = 0;
            return;
        }
        if(coll.onRightWall && dir.x >= 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            speed = 0;
            return;
        }
        if(dir.x != 0)
        {
        speed += dir.x*acceleration * Time.deltaTime;
        speed = Mathf.Clamp(speed,-maxSpeed,maxSpeed);
        rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else
        {
            speed -= Mathf.Sign(speed)*deceleration * Time.deltaTime;
            rb.velocity = new Vector2(speed,rb.velocity.y);
            
        }
        //Debug.Log(speed);
    }

    private void Jump(float Jump_initialVelocity)
    {

        rb.velocity = new Vector2(rb.velocity.x, Jump_initialVelocity);
        jumped = true;

        jumpParticle.Play();
    }


}
