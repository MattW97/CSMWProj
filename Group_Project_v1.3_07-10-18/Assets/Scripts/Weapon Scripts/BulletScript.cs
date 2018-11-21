using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public int bulletDamage;
    public float bulletSpeed = 25;
    private float lifeTime = 0.1f;

    private Transform thisTransform;

    void Start()
    {
        thisTransform = this.GetComponent<Transform>();
    }

    void Update()
    {
        thisTransform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealthManager>().DamagePlayer(bulletDamage);

            Transform bloodParticleObject = other.gameObject.transform.Find("BloodSplatterParticle");
            bloodParticleObject.rotation = Quaternion.LookRotation(this.gameObject.transform.forward);
            bloodParticleObject.GetComponent<ParticleSystem>().Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealthManager>().DamagePlayer(bulletDamage);
            Destroy(this.gameObject);
        }
    }
}
