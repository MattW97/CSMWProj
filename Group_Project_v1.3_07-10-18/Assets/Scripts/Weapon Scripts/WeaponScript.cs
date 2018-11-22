using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour {

    public Vector3 position;
    public Vector3 rotation;

    public GameObject[] bulletSpawn;
    public GameObject bullet;
    public GameObject player;

    private Transform thisTransform;
    private Transform pickUpVolume;

    private float despawnTimer;
    private float initDespawnTime = 15;
    private float fireRate = 0.3f;
    private float shotTimer;
    private float initAmmoAmount = 4;
    private float reloadTime = 1;
    public float ammoAmount;

    private int meleeDamage;

    private bool canShoot;
    private bool reloading;

    public bool canDealDamage;

    public Rigidbody thisRigidbody;

    public ParticleSystem muzzleFlash;

    private AudioSource audioSource;
    public AudioClip baseballBatImpact;
    //public AudioClip malletImpact;
    public AudioClip macheteImpact;

    public enum WeaponType
    {
        Shotgun,
        BaseballBat,
        Mallet,
        Machete
    }

    public WeaponType weaponSelection;

    void Start()
    {
        thisTransform = this.GetComponent<Transform>();
        thisRigidbody = this.GetComponent<Rigidbody>();
        audioSource = this.GetComponent<AudioSource>();
        pickUpVolume = thisTransform.Find("PickUpVolume");

        ammoAmount = initAmmoAmount;
        canShoot = false;
        reloading = false;
        canDealDamage = false;

        despawnTimer = initDespawnTime;

        if(weaponSelection == WeaponType.BaseballBat)
        {
            meleeDamage = 12;
        }
        if (weaponSelection == WeaponType.Mallet)
        {
            meleeDamage = 18;
        }
        if (weaponSelection == WeaponType.Machete)
        {
            meleeDamage = 10;
        }
    }

    void Update()
    {
        if(transform.parent == null)
        {
            despawnTimer -= Time.deltaTime;

            if(despawnTimer <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void GetPickedUp (Rigidbody rightPalm)
    {
        player = rightPalm.transform.root.gameObject;

        thisRigidbody.isKinematic = true;

        gameObject.transform.parent = rightPalm.transform;
        gameObject.transform.localPosition = position;
        gameObject.transform.localEulerAngles = rotation;

        pickUpVolume.gameObject.SetActive(false);
        gameObject.GetComponent<MeshCollider>().isTrigger = true;

        canShoot = true;

        despawnTimer = initDespawnTime;
    }

    public void GetDropped ()
    {
        gameObject.transform.parent = null;

        pickUpVolume.gameObject.SetActive(true);
        gameObject.GetComponent<MeshCollider>().isTrigger = false;

        thisRigidbody.isKinematic = false;

        canShoot = false;
    }

    public void Shoot(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        // If right trigger is pressed...
        if(weaponSelection == WeaponType.Shotgun)
        {
            if (Input.GetAxis(playerController.fireButton) > 0 && !reloading && !playerController.pickUpMode)
            {
                if (initAmmoAmount > 0)
                {

                    shotTimer -= Time.deltaTime;

                    if (shotTimer <= 0)
                    {

                        for (int i = 0; i < bulletSpawn.Length; i++)
                        {

                            Instantiate(bullet, bulletSpawn[i].transform.position, bulletSpawn[i].transform.rotation);
                        }

                        muzzleFlash.Play();
                        shotTimer = fireRate;
                        initAmmoAmount = initAmmoAmount - 1;
                    }
                }
            }
            else
            {
                // Stops delay between pressing the fire button and the shot firing
                // Makes it so shotTimer is only active whilst the fire button is down
                shotTimer = 0;
            }
        }
    }

    public void Reload(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        // If X is pressed...
        if (Input.GetButtonDown(playerController.reload) && !playerController.pickUpMode && initAmmoAmount < ammoAmount || initAmmoAmount == 0)
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime <= 0)
            {

                initAmmoAmount = ammoAmount;
                shotTimer = 0;
                reloadTime = 1;
                reloading = false;
            }
        }
    }

    void OnTriggerEnter (Collider collider)
    {
        if(player != null)
        {
            
            if (collider.tag == "Player" && canDealDamage)
            {
                collider.gameObject.GetComponent<PlayerHealthManager>().DamagePlayer(meleeDamage / 2);

                Transform bloodParticleObject = collider.gameObject.transform.Find("BloodSplatterParticle");
                bloodParticleObject.rotation = Quaternion.LookRotation(this.gameObject.transform.forward);
                bloodParticleObject.GetComponent<ParticleSystem>().Play();

                // Impact sounds
                if (weaponSelection == WeaponType.BaseballBat)
                {
                    audioSource.PlayOneShot(baseballBatImpact, 0.2f);
                }
                if (weaponSelection == WeaponType.Mallet)
                {
                    audioSource.PlayOneShot(baseballBatImpact, 0.2f);
                }
                if (weaponSelection == WeaponType.Machete)
                {
                    audioSource.PlayOneShot(macheteImpact, 0.2f);
                }
            }
        }    
    }
}
