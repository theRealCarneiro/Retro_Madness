using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBehaviour : MonoBehaviour
{
    public Transform sonic;
    public float maxSpeed = 3.4f;
    float moveDirection = 1f;
    bool dead = false;
    Rigidbody2D r2d;
    public Animator animator;
    // Start is called before the first frame update

    void OnCollisionEnter2D(Collision2D col)
    {
        bool isGrounded = GameObject.FindGameObjectWithTag("Player").GetComponent<MarioController>().isGrounded;
        bool spin = GameObject.FindGameObjectWithTag("Player").GetComponent<MarioController>().spin;
        bool charge = GameObject.FindGameObjectWithTag("Player").GetComponent<MarioController>().charge;
        if(col.collider.tag == "Player" && (isGrounded == false || spin == true || charge == true))
        {
            dead = true;
            GetComponent<AudioSource>().Play();
        }
    } 

    void Start()
    {
        sonic = GameObject.Find("Sonic").transform;;
        r2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead == false){
            if(Vector2.Distance(sonic.position, transform.position) < 2f){
                if(sonic.position.x > transform.position.x){
                    moveDirection = 1f;
                } else {
                    moveDirection = -1f;
                }
            } else moveDirection = 0f;
        } else animator.Play("Goomba Stomped");
        
    }

    void FixedUpdate()
    {
        // Apply movement velocity
        if (dead == false){
            r2d.velocity = new Vector2(moveDirection, 0f);
        }
    }
}
