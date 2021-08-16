using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBlock : MonoBehaviour
{
    public bool isActive = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.tag == "Player" && !isActive)
        {
            isActive = true;
            GetComponent<AudioSource>().Play();
            GetComponent<SpriteRenderer>().enabled = true;

            //gameObject.transform.GetChild(0).gameObject.SetActive(true);
            //Wait();
            //gameObject.transform.GetChild(0).gameObject.SetActive(false);
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
