using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot01 : MonoBehaviour {

    //**** Public variables
    [Header("Stats")]
    public float baseHealth;
    [System.NonSerialized]
    public Stats stats = new Stats();

	void Start () {
        
        stats.baseHealth = baseHealth;
        stats.currentHealth = baseHealth;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public class Stats
    {
        public float currentHealth = 100f;
        public float baseHealth = 100f;
        public float baseDammage = 40f;
        
    }
}
