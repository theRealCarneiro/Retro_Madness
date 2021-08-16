using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class MarioController : MonoBehaviour
{
    // Move player in 2D space
    float Speed = 0f;
    float spinSpeed;
    public float MaxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float Acceleration = 10f;
    public float Deceleration = 10f;
    public float gravityScale = 1.5f;
    public AudioSource dash;
    public AudioSource jump;
    public AudioSource stop;
    public bool spin = false;
    public bool charge = false;
    public Camera mainCamera;
    public Animator animator;
    public Text deathText;

    bool facingRight = true;
    float moveDirection = 1;
    public bool isGrounded = false;
    bool dead = false;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    CapsuleCollider2D mainCollider;
    Transform t;
    bool lookDown = false;
    bool cameraLock;
    public bool isOver = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.tag == "Buraco" )
        {
            dead = true;
            GameObject.FindGameObjectWithTag("DeadIndicator").GetComponent<SpriteRenderer>().enabled = true;
            deathText.enabled = true;
            Time.timeScale = 0;
            //Application.LoadLevel (Application.loadedLevel);
        }
        else if(col.collider.tag == "Enemy" && isGrounded == true && spin == false && charge == false){
            dead = true;
            GameObject.FindGameObjectWithTag("DeadIndicator").GetComponent<SpriteRenderer>().enabled = true;
            deathText.enabled = true;
            Time.timeScale = 0;
            //Application.LoadLevel (Application.loadedLevel);
        }
        else if(col.collider.tag == "Enemy" && (isGrounded == false || spin == true)){
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight/1.5f);
        }
        else if(col.collider.tag == "PlacaFim"){
            isOver = true;
        }
    } 

    // Use this for initialization
    void Start()
    {
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;

        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(dead){
            if(Input.GetKeyDown(KeyCode.R)){
                GameObject.FindGameObjectWithTag("DeadIndicator").GetComponent<SpriteRenderer>().enabled = false;
                Time.timeScale = 1;
                Application.LoadLevel (Application.loadedLevel);
            }
            
        }

        if(isOver){
            Speed = 0f;
            animator.Play("Victory");
            deathText.enabled = true;
            if(Input.GetKeyDown(KeyCode.R))
                Application.LoadLevel (Application.loadedLevel);
        }

        if (isGrounded && !isOver)
        {
            if(dead)
                animator.Play("Dying");
            else if(charge)
                animator.Play("Spindash");
            else if(lookDown && !spin)
                animator.Play("Look Down");
            else if(spin)
                animator.Play("Roll");
            else if(Speed == 0f)
                animator.Play("Idle");
            else if(Speed < MaxSpeed * 0.95f && Speed > -MaxSpeed * 0.95f)
                animator.Play("Walk");
            else if(Speed > MaxSpeed * 0.95f || Speed < - MaxSpeed * 0.95f)
                animator.Play("Run");
        }
        else if(!isOver)
            animator.Play("Roll");
        

        if(Input.GetKeyDown(KeyCode.DownArrow)){
            if(!lookDown)
                lookDown = true;
            if(Speed < 0.1f && Speed > -0.1f){
                spin = false;
            }
            else spin = true;
        }

        //if ((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)) && isGrounded && dead == false)
        //{
            //if(lookDown == true && !spin){
                //spin = true;
            //}
        //}


        if(Input.GetKeyUp(KeyCode.DownArrow) && !isOver){
            if(charge && (Speed < 0.1f && Speed > -0.1f)){
                charge = false;
                spin = true;
                Speed = MaxSpeed * 3 * moveDirection;
            }
            lookDown = false;
        }

        if(spin  && !isOver){
            RaycastHit2D hit;
            if(moveDirection == 1)
                hit = Physics2D.Raycast(transform.position, Vector2.right);
            else hit = Physics2D.Raycast(transform.position, Vector2.left);
            float distance = 1;
            if (hit.collider != null && hit.collider.tag != "Enemy")
                distance = Mathf.Abs(hit.point.x - transform.position.x);
            if(distance > 0.1){
                if(Speed < 0.1f && Speed > -0.1f && charge == false)
                    spin = false;
                else if(moveDirection < 0 && Speed < 0)
                    Speed = Speed + (Acceleration * Time.deltaTime);
                else if (Speed > 0) 
                    Speed = Speed - (Acceleration * Time.deltaTime);
            } else{
                Speed = 0;
                spin = false;
            }
        }

        // Jumping
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && isGrounded && dead == false && !isOver)
        {
            if(lookDown == false){

                jump.Play();
                r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
            } 
            else if(!spin){
                //spin = true;
                dash.Play();
                charge = true;
            }
        }

        // Movement controls
        if(!dead && !lookDown && !isOver)
        {
            if (Input.GetKey(KeyCode.LeftArrow)){
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left);
                float distance = 1;
                if (hit.collider != null && hit.collider.tag != "Level")
                    distance = Mathf.Abs(hit.point.x - transform.position.x);
                if(distance > 0.1){
                    if(!spin) moveDirection = -1;
                    float acc = Acceleration;
                    if(Speed > 0f){
                        //stop.PlayOneShot();
                        acc = Deceleration;
                    }
                    Speed = Speed - (acc * Time.deltaTime);
                }else Speed = 0;
            } else if (Input.GetKey(KeyCode.RightArrow)){ 
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right);
                float distance = 1;
                if (hit.collider != null && hit.collider.tag != "Level")
                    distance = Mathf.Abs(hit.point.x - transform.position.x);
                if(distance > 0.1){
                    if(!spin) moveDirection = 1;
                    float acc = Acceleration;
                    if(Speed < 0f)
                        acc = Deceleration;
                    Speed = Speed + (acc * Time.deltaTime);
                } else Speed = 0;
            } else if(!spin){
                if(Speed > Deceleration * Time.deltaTime)
                    Speed = Speed - (Deceleration * Time.deltaTime);
                else if(Speed < -Deceleration * Time.deltaTime)
                    Speed = Speed + (Deceleration * Time.deltaTime);
                else
                    Speed = 0;
            }
        }
        

        // Change facing direction
        if (moveDirection != 0 && !spin)
        {
            if (moveDirection > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        }
        


        // Camera follow
        if (mainCamera)
        {   
            float xpos;
            if(transform.position.x > 26f){
                cameraLock = true;
                xpos = 26f;
            }else if(transform.position.x < -3.834f){
                xpos = -3.834f;
            } else xpos = t.position.x;
            if(cameraLock) xpos = 26f;
            mainCamera.transform.position = new Vector3(xpos, cameraPos.y, cameraPos.z);
        }
    }

    void FixedUpdate()
    {
        //if(spin)
        //{
            //Debug.Log(spinSpeed);
            //moveDirection = facingRight ? 1 : -1;
            //r2d.AddForce(200f * moveDirection * new Vector2(1,0), 0);
            //r2d.velocity = new Vector2(100f, r2d.velocity.y);
        //}
        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        // Check if player is grounded
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);
        //Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true
        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider)
                {
                    isGrounded = true;
                    break;
                }
            }
        }


        // Apply movement velocity
        //Vector3 t = transform.position;
        //t.x = transform.position.x + Speed * Time.deltaTime;
        //transform.position = t;
        r2d.velocity = new Vector2(Speed, r2d.velocity.y);
        //r2d.AddRelativeForce(5f * moveDirection * new Vector2(1,0), 0);

        // Simple debug
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, colliderRadius, 0), isGrounded ? Color.green : Color.red);
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(colliderRadius, 0, 0), isGrounded ? Color.green : Color.red);
    }
}