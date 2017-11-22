using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {


    //**** Public variables
    public float boltSpeed;

    private float startTime; 
    


    //**** Private variables
    private Rigidbody2D boltRB;
    private Transform boltTransofrm;
    private Player player;
    private string enemyHitBoxTag = "Enemy";


    void Start()
    {
        startTime = Time.time;
        player = gameObject.GetComponentInParent<Player>();
        boltRB = gameObject.GetComponent<Rigidbody2D>();
        boltTransofrm = gameObject.GetComponent<Transform>();
        boltTransofrm.localScale = new Vector3(boltTransofrm.localScale.x * player.movementInfo.facingDirection, boltTransofrm.localScale.x, boltTransofrm.localScale.x);
        boltRB.velocity = Vector2.right * boltSpeed  * Mathf.Sign(player.movementInfo.facingDirection) ;
        transform.parent = null;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            if (other.tag == enemyHitBoxTag)
        {
            AudioSource audio = gameObject.GetComponentInChildren<AudioSource>();
            Collider2D collider = gameObject.GetComponent<Collider2D>();

            if (audio.clip.length > Time.time - startTime)
            {
                SpriteRenderer vfx = gameObject.GetComponentInChildren<SpriteRenderer>();
                Destroy(vfx);
                Destroy(collider);
                Destroy(gameObject, audio.clip.length - (Time.time - startTime));
            }
            else
            {
                Destroy(gameObject);
            }
        }
           
        

        
    }
}
