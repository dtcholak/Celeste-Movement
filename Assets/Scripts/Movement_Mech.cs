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
    private Player_Input control;

    [Space]
    [Header("Stats")]
    [SerializeField] public float maxSpeed = 10; //max horizonal speed  
    [SerializeField] public float acceleration = 10; //x accel constant 
    [SerializeField] public float deceleration = 10; //x decel constant
    [SerializeField] public float MaxJumpHeight = 1; // y jump height (holding jump)
    [SerializeField] public float MinJumpHeight = 0.5f; // y jump height (tap jump)
    [SerializeField] public float TimetoJumpApex = 0.4f; //y jump uptime (holding jump)
    [SerializeField] public float TimetoJumpDrop = 0.3f; //y jump downtime (holding jump)
    public float dashSpeed = 15; //dash speed
    private float speed = 0; //initial velocity
    public float dashdrag = 50; //linear drag after dashing
    private float abs_speed;
    

    [Space]
    [Header("Booleans")]
    public bool groundTouch; //boolean player touching ground 
    public bool jumped; //boolean player recently jumped
    public bool isDashing; //boolean player is Dashing
    public int side = 1; //1 is right, -1 left 

    [Space]
    [Header("Polish")]
    public ParticleSystem jumpParticle;

    float initialJumpVelocity_max; //y jump hold velocity variable creation 
    float initialJumpVelocity_min; //y jump tap velocity variable 
    float upGravity_max; //variable creation for upwards gravity (less than downward for "heavy" feel )
    float downGravity_max; //variable creation for downwards gravity 
    float timer = 0; //timer for checking jump timing
    float dragTimer = 0; //timer for putting linear drag back to normal

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision_Mech>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
        control = GetComponent<Player_Input>();

        //Jump calculations for gravity
        initialJumpVelocity_max = 2 * MaxJumpHeight / TimetoJumpApex; //calculation for initial jump hold velocity (based on max height and time to apex)
        
        upGravity_max = -2 * MaxJumpHeight / Mathf.Pow(TimetoJumpApex, 2.0f); //upward gravity calculated to apex of jump based on 0 velocity at apex and set time to apex  
        downGravity_max = -2 * MaxJumpHeight / Mathf.Pow(TimetoJumpDrop, 2.0f); //downward gravity calculated to apex of jump based on 0 velocity at apex and set time to fall

        initialJumpVelocity_min = Mathf.Sqrt(2 * Mathf.Abs(upGravity_max * MinJumpHeight)); //calculation for initial jump tap velocity (based on upward gravity calc and set jump tap height)

        //Debug.Log(initialJumpVelocity_max);
        //Debug.Log(initialJumpVelocity_min);
        //Debug.Log(upGravity_max);
        //Debug.Log(downGravity_max);
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 dir = control.dir; //vector setting of raw x + y 
        float x = dir.x; //raw x
        float y = dir.y; //raw y

        Vector2 dash_dir = control.dash_dir;
        float dash_x = dash_dir.x;
        float dash_y = dash_dir.y;

        
        anim.SetHorizontalMovement(x, y, rb.velocity.y); //animation for x direction movement based on x, y, current rigid body y velocity
        if (Input.GetButtonDown("Fire3") && !isDashing)   //if left shift button is pressed
        {
            Dash(dash_x,dash_y);    //dash at dashspeed and add linear drag after a delay
        }
        if (isDashing)
        {
            dragTimer += Time.deltaTime;
        }
        if (dragTimer > 0.7)
        {
            rb.drag = 0;
            isDashing = false;
            dragTimer = 0;
        }
        if (!isDashing)
        {   
        
        Walk(dir);
        
        if (Input.GetButtonDown("Jump") && coll.onGround && !jumped) //if jump pressed, and touching ground, and haven't recently jumped
        {
            Jump(initialJumpVelocity_max); //jump at the speed of initial jump velocity max
            anim.SetTrigger("jump"); //set jump trigger to active in animation script
        }
        if (Input.GetButtonUp("Jump")) //if jump released 
        {
            if (rb.velocity.y > initialJumpVelocity_min) //if current rigid body y velocity is greater than initial jump tap velocity 
            {
            rb.velocity = new Vector2(rb.velocity.x, initialJumpVelocity_min); //set current rigid body velocity to current rb x velocity, and initial jump tap velocity as y)
            jumped = false; //reset so no jump has recently happened
            }
        }
        }
            Vector2 Velocity = rb.velocity; //new variable for current velocity

        if(rb.velocity.y > 0) //if current rb y velocity is greater than 0 
        {
            Physics2D.gravity = new Vector2(0, upGravity_max); //set physics gravity to upward gravity max
            //Velocity.y += upGravity_max * Time.deltaTime;
            //rb.velocity = Velocity;
        }else
        {
            Physics2D.gravity = new Vector2(0, downGravity_max); //set physics gravity to downward gravity max
            //Velocity.y += downGravity_max * Time.deltaTime;
            //rb.velocity = Velocity;
        }
        
        

        if (coll.onGround && !groundTouch) //if touching ground and weren't before (landing)
        {
            GroundTouch();
            groundTouch = true; //set groundtouch to true (animation)
            
        }
        timer += Time.deltaTime; //increment timer

        if(!coll.onGround && groundTouch) //if not touching ground and were before (takeoff)
        {
            groundTouch = false; //set groundtouch to false (no animation)
        }
        /*if(coll.onGround && jumped) //if touching ground and recently jumped
        {
            timer += Time.deltaTime; //increment timer
            
        } */
        if(timer > 1.4) //if more than 1.4 seconds have passed
        {
            jumped = false; //impossible to have recently jumped
            timer = 0; //set timer to zero
        }
        //Debug.Log(timer);
        if(x > 0) //if input x right 
        {
            side = 1; //set side to right 
            anim.Flip(side); //correcting animation side 
        }
        if (x < 0) //if input x left
        {
            side = -1; //set side to left
            anim.Flip(side); //corecting animation side
        }


    }

    void GroundTouch() //void because no output, just animation
    {

        side = anim.sr.flipX ? -1 : 1; //set direction facing
        
        jumpParticle.Play(); //play particles at feet for landing
    }



    private void Walk(Vector2 dir) //walk function, with direction input (x raw,y raw)
    {
        speed = rb.velocity.x;
        
        abs_speed = Mathf.Abs(speed);
        Debug.Log(abs_speed);
        if(coll.onLeftWall && dir.x <= 0) //if touching left wall and left input 
        {
            rb.velocity = new Vector2(0, rb.velocity.y); //set x velocity to 0 
            speed = 0; //set speed to 0 
            return;
        }
        if(coll.onRightWall && dir.x >= 0) //if touching right wall and right input 
        {
            rb.velocity = new Vector2(0, rb.velocity.y); //set x velocity to 0 
            speed = 0; // set speed to 0 
            return;
        }
        if(dir.x != 0 && abs_speed <= 10) //if either right or left input (any x direction input)
        {
        speed += dir.x*acceleration*Time.deltaTime; //increment speed by acceleration constant
        speed = Mathf.Clamp(speed,-maxSpeed,maxSpeed); //ensures that speed remains within of maximums 
        }
        else if(dir.x != 0 && abs_speed > 10)
        {
            if(Mathf.Sign(dir.x) != Mathf.Sign(speed))
            {
            speed += dir.x*acceleration*Time.deltaTime; //increment speed by acceleration constant
            }
            speed = Mathf.Clamp(speed,-6*maxSpeed,6*maxSpeed); //ensures that speed remains within of maximum
            //rb.velocity = new Vector2(speed, rb.velocity.y); //set rigid body velocity x to speed and y rigid body to current y rigid body velocity
        }
        else //OTHERWISE
        {
            speed -= Mathf.Sign(speed)*deceleration * Time.deltaTime; //decelerate if no input of x direction
            //rb.velocity = new Vector2(speed,rb.velocity.y); //update rigid body velocity
        }
        rb.velocity = new Vector2(speed, rb.velocity.y); //set rigid body velocity x to speed and y rigid body to current y rigid body velocity
        //Debug.Log(speed);
    }

    private void Jump(float Jump_initialVelocity) //creation of jump initial velocity 
    {

        rb.velocity = new Vector2(rb.velocity.x, Jump_initialVelocity); //set rigid body velocity to current rb x velocity and input to function
        jumped = true; //recently jumped

        jumpParticle.Play(); //jump particle animation
    }

    private void Dash(float x, float y)
    {
     
     
        isDashing = true; //set that player is Dashing
        //anim.SetTrigger("dash");

        //rb.velocity = Vector2.zero; //set rigid body velocity to a zero vector
        Vector2 dash_dir = new Vector2(x, y); //get the input directions and assign them as current
        rb.velocity += dash_dir.normalized * dashSpeed; //make the velocities of magnitude one and multiply by the set dash speed 
        StartCoroutine(DashDrag());

     

    }

    IEnumerator DashDrag()
    {
    yield return new WaitForSeconds(0.2f); //wait half a second for dash to occur
    //Vector2 dir = new Vector2(x, y); //get input directions and assign them as current
    //rb.velocity -= dir.normalized * dashdecay; //make the velocities of magnitude one and subtract by set dash decay to slow 
    rb.drag = dashdrag;
    
    }
}
 

