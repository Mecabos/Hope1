using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    //**** Public variables
    public string groundTagName = "Ground"; //name of the ground tag to be detected

    //**** Private variables
    private Player player;

	void Start(){
		player = gameObject.GetComponentInParent<Player> ();
	}

	void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == groundTagName)
		    player.movementInfo.isGrounded = true;
	}

	void OnTriggerStay2D(Collider2D col) {
        if (col.tag == groundTagName)
            player.movementInfo.isGrounded = true;
	}

	void OnTriggerExit2D(Collider2D col) {
        if (col.tag == groundTagName)
            player.movementInfo.isGrounded = false;
	}

}
