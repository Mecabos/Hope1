using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageByContact : MonoBehaviour {

    //**** Private variables
    private string playerBoltTag = "PlayerBolt";
    private Robot01 robot01; 
    void Start()
    {
        robot01 = gameObject.GetComponentInParent<Robot01>();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        
        if (other.tag == playerBoltTag)
        {
            BoltProperties boltProperties =  other.gameObject.GetComponent<BoltProperties>();
            Damage(boltProperties.baseDamage);
        }
    }
    
	
	void Update () {
        
	}

    private void Damage(float damageTaken)
    {
        robot01.stats.currentHealth -= damageTaken;
        if (robot01.stats.currentHealth <= 0)
        {
            Destroy(robot01.gameObject);
        }
    }
}
