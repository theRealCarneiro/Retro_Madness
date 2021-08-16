using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowserArena : MonoBehaviour
{
    public bool active = false;
    public AudioSource bossTheme;
    public AudioSource mainTheme;
    public AudioSource levelClear;
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.tag == "Player" && !active)
        {
            active = true;
            mainTheme.Stop();
            bossTheme.Play();
        }
    } 

    void OnCollisionExit2D(Collision2D col)
    {
        if(col.collider.tag == "Enemy")
        {
            bossTheme.Stop();
            levelClear.Play();

            GameObject.Find("Placa").GetComponent<Rigidbody2D>().simulated = true;
            //GameObject.Find("Placa").active = true;
        }
    } 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
