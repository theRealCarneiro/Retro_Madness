using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSignPost : MonoBehaviour
{
    public bool active = false;
    public Animator animator;
    //public AudioSource bossTheme;
    //public AudioSource mainTheme;
    //public AudioSource levelClear;
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.tag == "Player" && !active)
        {
            active = true;
            animator.Play("Spin");
        }
    } 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
