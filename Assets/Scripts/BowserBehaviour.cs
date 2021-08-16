using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowserBehaviour : MonoBehaviour
{
    public Transform sonic;
    public float maxSpeed = 3.4f;
    float moveDirection = 1f;
    bool dead = false;
    public int hits = 5;
    Rigidbody2D r2d;
    public Animator animator;
    // Start is called before the first frame update

    void OnCollisionEnter2D(Collision2D col)
    {
        bool isGrounded = GameObject.FindGameObjectWithTag("Player").GetComponent<MarioController>().isGrounded;
        bool spin = GameObject.FindGameObjectWithTag("Player").GetComponent<MarioController>().spin;
        if(col.collider.tag == "Player" && (isGrounded == false || spin == true))
        {
            hits -= 1;
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

        if(Vector2.Distance(sonic.position, transform.position) < 2f){
            if(sonic.position.x > transform.position.x){
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                moveDirection = 1f;
            } else {
                moveDirection = -1f;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        } else moveDirection = 0f;
        if(hits == 0){
            dead = true;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        
    }

    void FixedUpdate()
    {
        if(dead == false)
            r2d.velocity = new Vector2(moveDirection, 0f);
    }
}
