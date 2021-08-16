using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class MarioController : MonoBehaviour
{
    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;
    public bool spin = false;
    public Camera mainCamera;
    public Animator animator;

    bool facingRight = true;
    float moveDirection = 0;
    public bool isGrounded = false;
    bool dead = false;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    CapsuleCollider2D mainCollider;
    Transform t;
    bool lookDown = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.tag == "Buraco" )
        {
            dead = true;
            Application.LoadLevel (Application.loadedLevel);
        }
        else if(col.collider.tag == "Enemy" && isGrounded == true && spin == false){
            dead = true;
            Application.LoadLevel (Application.loadedLevel);
        }
        else if(col.collider.tag == "Enemy" && (isGrounded == false || spin == true)){
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight/2);
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
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            if(!lookDown){
                lookDown = true;
                animator.Play("Look Down");
            }
        }
        if(Input.GetKeyUp(KeyCode.DownArrow)){
            spin = false;
            lookDown = false;
        }


        // Movement controls
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            lookDown = false;
            if(dead == false)
            {
                if (isGrounded)
                {
                    animator.Play("Run");
                }
                moveDirection = Input.GetKey(KeyCode.LeftArrow) ? -1 : 1;
            }
        }
        else
        {
            if (isGrounded || r2d.velocity.magnitude < 0.01f)
            {
                moveDirection = 0;
                if(dead == false)
                {
                    if(lookDown == false && r2d.velocity.magnitude < 0.01f)
                        animator.Play("Idle");
                }
                else 
                {
                    animator.Play("Dying");
                }
            }
        }
        
        if (isGrounded == false)
        {
            animator.Play("Roll");
        }

        // Change facing direction
        if (moveDirection != 0)
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
        
        if ((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)) && isGrounded && dead == false)
        {
            if(lookDown == true){
                spin = true;
            }
        }

        // Jumping
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && isGrounded && dead == false)
        {
            if(lookDown == false){
                animator.Play("Roll");
                r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
            } else animator.Play("Spindash");
        }

        // Camera follow
        if (mainCamera)
        {   
            float xpos;
            if(transform.position.x > 26f){
                xpos = 26f;
            }else if(transform.position.x < -3.834f){
                xpos = -3.834f;
            } else xpos = t.position.x;
            mainCamera.transform.position = new Vector3(xpos, cameraPos.y, cameraPos.z);
        }
    }

    void FixedUpdate()
    {
        if(spin)
        {
            moveDirection = facingRight ? 1 : -1;
            r2d.AddForce(200f * moveDirection * new Vector2(1,0), 0);
        }
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
        r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);

        // Simple debug
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, colliderRadius, 0), isGrounded ? Color.green : Color.red);
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(colliderRadius, 0, 0), isGrounded ? Color.green : Color.red);
    }
}