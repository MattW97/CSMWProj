using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {

    [SerializeField] float bulletSpeed;

    private float lifeTime = 0.15f;

	void Update () {

        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);

        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
	}

    void OnTriggerEnter(Collider other) {

        if(other.gameObject.tag == "Obstacle") {

            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Player") {

            // Deal damage

            Destroy(gameObject);
        }

        //Debug.Log(other.gameObject);
    }
}
