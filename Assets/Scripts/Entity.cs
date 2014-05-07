using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public float health;

	public void TakeDamage(float dmg){
		health -=  dmg;
		if (health <= 0) {
			Die();
		}
	}

	public void Die(){

		Debug.Log ("DIE");
	}
}
