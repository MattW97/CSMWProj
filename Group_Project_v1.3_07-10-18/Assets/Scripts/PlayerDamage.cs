using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour {

    public int damageToPlayer;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealthManager>().DamagePlayer(damageToPlayer);

            Transform bloodParticleObject = other.gameObject.transform.Find("BloodSplatterParticle");
            bloodParticleObject.rotation = Quaternion.LookRotation(this.gameObject.transform.forward);
            bloodParticleObject.GetComponent<ParticleSystem>().Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealthManager>().DamagePlayer(damageToPlayer);
            Destroy(this.gameObject);
        }
    }
}
