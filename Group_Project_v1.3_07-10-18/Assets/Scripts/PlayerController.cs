using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(IKControl))]
public class PlayerController : MonoBehaviour {
    
    private float shotTimer;
    private float playerTurnSpeed = 8;
    private float ammoAmount = 30;
    private float reloadTime = 1;
    [SerializeField] float fireRate;
    [SerializeField] float movementSpeed = 4;

    private int totalCurrentMashes = 0;
    public int numToMash = 4;

    private bool reloading;
    public bool canControl;
    public bool isDead;
    public bool beenDragged;
    public bool ragdolling;
    public bool inRange;
    public bool pickUpMode;


    [Header("Controller Inputs")]
    [Tooltip("Enter the same name that is found within the input manager of Unity")]
    [SerializeField] string leftStickHorizontal;
    [SerializeField] string leftStickVertical;
    [SerializeField] string rightStickHorizontal;
    [SerializeField] string rightStickVertical;
    [SerializeField] string fireButton;
    [SerializeField] string fineAim;
    [SerializeField] string reload;
    [SerializeField] string pickUpState;
    private Vector3 movementInput;
    public Vector3 movementVelocity;
    [Space]

    [Header("Game Objects")]
    public GameObject bullet;
    public GameObject bulletSpawn;
    public GameObject laserSight;
    public GameObject weapon;
    [Space]

    [Header("Transforms & Rigidbodies")]
    private Rigidbody thisRigidbody;
    public Rigidbody pointToGrab;
    public Transform thisTransform;
    public Transform placeToLook;
    public Transform thisPlayersHips;
    public List<Transform> otherPlayers;
    [Space]

    [Header("UI")]
    public Text ammoCountText;
    public Image reloadBarImage;
    public Text reloadingText;
    [Space]

    public Vector3 playerDirection;

    public ParticleSystem muzzleFlash;

    [Header("Script Links")]
    public PickUp _pickUpLink;
    
    private void Start() {

        thisTransform = GetComponent<Transform>();
        thisRigidbody = GetComponent<Rigidbody>();
        shotTimer = fireRate;
        reloading = false;
        canControl = true;
        isDead = false;

        RagdollSetup();

        // UI
        reloadBarImage.enabled = false;
        reloadingText.enabled = false;
    }

    private void Update() {

        // UI elements
        ammoCountText.text = ammoAmount.ToString() + " | ∞";
        reloadBarImage.fillAmount = reloadTime;      

        #region Player directions
        float forwardBackward = Vector3.Dot(movementVelocity.normalized, thisTransform.forward.normalized);

        if (forwardBackward > 0) {

            //Debug.Log(forwardBackward);
            // forward
        }
        else if (forwardBackward < 0) {

            //Debug.Log(forwardBackward);
            // backward
        }
        else {
            // neither
        }

        float rightLeft = Vector3.Dot(-movementVelocity.normalized, Vector3.Cross(thisTransform.forward, thisTransform.up).normalized);

        if (rightLeft > 0)
        {
            //Debug.Log(rightLeft);
            // right
        }
        else if (rightLeft < 0)
        {
            //Debug.Log(rightLeft);
            // left
        }
        else
        {
            // neither
        }
        #endregion

        if(!isDead) {
            if (canControl) {
                #region Character inputs

                // Configure input for left analog stick
                movementInput = new Vector3(Input.GetAxisRaw(leftStickHorizontal), 0f, Input.GetAxisRaw(leftStickVertical));
                movementVelocity = movementInput * movementSpeed;

                // Configure input for right analog stick
                playerDirection = Vector3.right * Input.GetAxisRaw(rightStickHorizontal) - Vector3.forward * Input.GetAxisRaw(rightStickVertical);

                // Stops log outputting that the Vector3 is equal to 0
                if (playerDirection != Vector3.zero)
                {

                    // Old method of rotation - Jittery
                    // transform.rotation = Quaternion.LookRotation(playerDirection, Vector3.up);

                    // New method of rotation - Smooth
                    Quaternion targetRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, targetRotation, playerTurnSpeed * Time.deltaTime);
                }

                // If right trigger is pressed...
                if (Input.GetAxis(fireButton) > 0 && !reloading && !pickUpMode)
                {
                    if (ammoAmount > 0)
                    {

                        shotTimer -= Time.deltaTime;

                        if (shotTimer <= 0)
                        {

                            Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
                            muzzleFlash.Play();
                            shotTimer = fireRate;
                            ammoAmount = ammoAmount - 1;
                        }
                    }
                }

                if (Input.GetButton(reload) && !pickUpMode && ammoAmount < 30 || ammoAmount == 0)
                {

                    reloading = true;
                    reloadBarImage.enabled = true;
                    reloadingText.enabled = true;
                }

                if (reloading)
                {
                    reloadTime -= Time.deltaTime;

                    if (reloadTime <= 0)
                    {

                        ammoAmount = 30;
                        reloadTime = 1;
                        reloading = false;
                        reloadBarImage.enabled = false;
                        reloadingText.enabled = false;
                    }
                }

                // If left trigger is pressed...
                if (Input.GetAxis(fineAim) > 0)
                {

                    playerTurnSpeed = 0.5f;
                    movementSpeed = 1;

                    // Slowly extends laser sight
                    if (laserSight.transform.localScale.y != 10)
                    {

                        laserSight.transform.localScale += new Vector3(0, 0.1f, 0);
                    }

                    // Stops floating point error, sets to extended scale if it goes above 10
                    if (laserSight.transform.localScale.y > 10)
                    {

                        laserSight.transform.localScale = new Vector3(3, 10, 1);
                    }

                }
                else
                {

                    playerTurnSpeed = 8;
                    movementSpeed = 4;

                    // Quickly retracts laser sight
                    if (laserSight.transform.localScale.y != 1)
                    {

                        laserSight.transform.localScale -= new Vector3(0, 0.5f, 0);
                    }

                    // Stops floating point error, sets to normal scale if it goes below 1
                    if (laserSight.transform.localScale.y < 1)
                    {

                        laserSight.transform.localScale = new Vector3(3, 1, 1);
                    }
                }

                //Y button is used to swap between Pick Up mode and weapon mode
                if (Input.GetButtonDown(pickUpState) && !pickUpMode && !ragdolling)
                {
                    pickUpMode = true;
                    weapon.SetActive(false);
                }
                else if (Input.GetButtonDown(pickUpState) && pickUpMode && !ragdolling)
                {
                    _pickUpLink.join = false;
                    Destroy(_pickUpLink.GetComponent<SpringJoint>());
                    pickUpMode = false;
                    pointToGrab.GetComponentInParent<PlayerController>().beenDragged = false;
                    weapon.SetActive(true);
                }

                //Current Setup for picking up player
                if(Input.GetButtonDown(reload) && pickUpMode && inRange && !_pickUpLink.join)
                {
                    _pickUpLink.join = false;
                    _pickUpLink.CreateJoint();
                }
                #endregion
            }
        }

        #region Button Mashing 

        //Button mashing if the player is just ragdolling
        if (ragdolling && !beenDragged && Input.GetButtonDown(pickUpState))
        {
            if (TotalCurrentMashes >= (numToMash - 1))
            {
                GetComponent<PlayerHealthManager>().currentHealth = GetComponent<PlayerHealthManager>().startingHealth;
                ragdolling = false;
                Ragdoll(false);
            }
            else
            {
                TotalCurrentMashes++;
            }
        }

        //Button mashing if the player is been dragged by an opposing player
        if (beenDragged && Input.GetButtonDown(pickUpState))
        {
            if (TotalCurrentMashes >= (numToMash - 1))
            {
                BreakDragging();
            }
            else
            {
                TotalCurrentMashes++;
            }

        }

        #endregion

        DistanceToPlayer();
    }

    private void FixedUpdate() {

        if(!isDead) {
            if(canControl) {

                //This causes the player not to fall with gravity. Remove this line and player falls with gravity working normally.

                // Applies velocity from this script directly to the Rigidbody
                thisRigidbody.velocity = movementVelocity;
            }
        }
    }

    /// <summary>
    /// If the player is been dragged call this when the player can break from it.
    /// Works by breaking the joint that is connected to the player.
    /// And currently set ths health to full
    /// </summary>
    private void BreakDragging()
    {
        beenDragged = false;
        GetComponent<PlayerHealthManager>().currentHealth = GetComponent<PlayerHealthManager>().startingHealth;
        pointToGrab.GetComponent<PickUp>().join = false;
        Destroy(pointToGrab.GetComponent<SpringJoint>());
        TotalCurrentMashes = 0;
    }

    /// <summary>
    /// This sets the ragdoll up and should be called during the start method
    /// </summary>
    private void RagdollSetup()
    {
        ragdolling = false;
        GetComponent<Animator>().enabled = true;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
        foreach (CharacterJoint cj in GetComponentsInChildren<CharacterJoint>())
        {
            cj.enableCollision = true;
            cj.enableProjection = true;
        }
        thisRigidbody.isKinematic = false;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Ragdoll Toggle is used for running and reseting the ragdoll
    /// </summary>
    /// <param name="ragdollToggle"></param>
    public void Ragdoll(bool ragdollToggle)
    {
        if (ragdollToggle) //Initiate Ragdoll
        {
            ragdolling = true;
            canControl = false;
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }
            //weapon.GetComponent<Rigidbody>().isKinematic = false;
            //weapon.GetComponent<BoxCollider>().enabled = true;
            weapon.gameObject.SetActive(false);
            thisRigidbody.isKinematic = true;
            GetComponent<Animator>().enabled = false;
        }

        else if (!ragdollToggle) //Reset Ragdoll
        {
            thisTransform.position = new Vector3(thisPlayersHips.position.x, 0 , thisPlayersHips.position.z);
            TotalCurrentMashes = 0;
            canControl = true;
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }
            weapon.GetComponent<Rigidbody>().isKinematic = true;
            //weapon.GetComponent<BoxCollider>().enabled = false;
            weapon.gameObject.SetActive(true);
            thisRigidbody.isKinematic = false;
            GetComponent<Animator>().enabled = true;
            ragdolling = false;
        }
    }

    /// <summary>
    /// Used to be constantly aware of how close the player is to each player
    /// </summary>
    public void DistanceToPlayer()
    {
        foreach (Transform otherPlayertrans in otherPlayers)
        {
            float dist = Vector3.Distance(otherPlayertrans.position, transform.position);
            
            if (dist < 1)
            {
                GetComponent<IKControl>().lookObj = placeToLook;
                GetComponent<IKControl>().ikActive = true;
                inRange = true;
            }
            else
            {
                inRange = false;
                GetComponent<IKControl>().ikActive = false;
            }


        }
    }

    #region Getters/ Setters

    public int TotalCurrentMashes
    {
        get
        {
            return totalCurrentMashes;
        }

        set
        {
            totalCurrentMashes = value;
        }
    }
    #endregion
}
