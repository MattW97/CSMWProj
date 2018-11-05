using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(IKControl))]
public class PlayerController : MonoBehaviour {

    [Header("Variables")]    
    public float playerNum;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] float movementSpeed = 4;
    private float dodgeSpeed = 10;
    private float shotTimer;
    private float playerTurnSpeed = 8;
    private float initAmmoAmount = 4;
    private float ammoAmount;
    private float reloadTime = 1;
    private float mashTimer;
    public float numToMash = 5;
    private float numToMashMultiplier = 2.0f;

    public float forwardBackward;
    public float rightLeft;

    private int totalCurrentMashes = 0;
    
    private bool reloading;
    private bool dodging;
    private bool canDodge;
    private bool playerInGame;
    public bool canControl;
    public bool isDead;
    public bool beenDragged;
    public bool ragdolling;
    public bool inRange;
    public bool pickUpMode;
    [Space]

    #region Inputs
    [HideInInspector] public string leftStickHorizontal;
    [HideInInspector] public string leftStickVertical;
    [HideInInspector] public string rightStickHorizontal;
    [HideInInspector] public string rightStickVertical;
    [HideInInspector] public string fireButton;
    [HideInInspector] public string pickUp;
    [HideInInspector] public string reload;
    [HideInInspector] public string mashButton;
    [HideInInspector] public string dodgeButton;
    #endregion

    private Vector3 dodgePos;
    private Vector3 movementInput;
    [HideInInspector] public Vector3 movementVelocity;

    [Header("Game Objects")]
    public GameObject[] bulletSpawn;
    public GameObject bullet;   
    public GameObject weapon;
    public GameObject dodgePoint;
    [Space]

    [Header("Transforms & Rigidbodies")]   
    public Rigidbody pointToGrab;
    public Transform placeToLook;
    public Transform thisPlayersHips;
    public List<Transform> otherPlayers;
    [HideInInspector] public Transform thisTransform;
    private Rigidbody thisRigidbody;
    [Space]


    [HideInInspector] public Vector3 playerDirection;

    [Header("Particle Systems")]
    public ParticleSystem muzzleFlash;
    [Space]

    [Header("Scripts")]
    public PickUp pickUpScript;
    public PlayerUI playerUILink;
    public UtilityManager utilManagerScript;
    
    void Start()
    {
        //SetUpInputs();

        thisTransform = GetComponent<Transform>();
        thisRigidbody = GetComponent<Rigidbody>();
        ammoAmount = InitAmmoAmount;
        reloading = false;
        dodging = false;
        canDodge = true;
        canControl = true;
        isDead = false;
        mashTimer = 0.5f;

        RagdollSetup();      
    }

    void Update()
    {
        if (!isDead)
        {
            // Placed the ButtonMashing() function in here as FixedUpdate() missed some Y inputs
            // Regular Update() fixes this issue
            ButtonMashing();

            #region Button Mash degredation timer (Optional)
            //if (TotalCurrentMashes > 0)
            //{
            //    mashTimer -= Time.deltaTime;
            //if (mashTimer <= 0)
            //{
            //    TotalCurrentMashes = TotalCurrentMashes - 1;
            //    mashTimer = 1f;
            //}

            //Debug.Log(TotalCurrentMashes);
            //}
            #endregion

            if (canControl)
            {
                if (Input.GetButtonDown(dodgeButton) && canDodge)
                {
                    dodging = true;
                }
            }

            #region Dodge bool
            if (dodging)
            {
                dodgePos = dodgePoint.transform.position;
                dodgePoint.transform.parent = null;
                canControl = false;
                transform.position = Vector3.Lerp(transform.position, dodgePos, dodgeSpeed * Time.deltaTime);
            }
            else
            {
                dodgePoint.transform.parent = this.gameObject.transform;
                dodgePoint.transform.position = (movementInput.normalized * 2) + transform.position;
                canControl = true;
            }

            // See OnTriggerEnter() function for dodging = false;
            #endregion
        }
    }

    private void FixedUpdate()
    {
        GetCharDirections();
        DistanceToPlayer();

        if (!isDead)
        {
            if(canControl)
            {
                CharMovement();
                Shooting();
                GrabAndDrag();

                //This causes the player not to fall with gravity. Remove this line and player falls with gravity working normally.
                // Applies velocity from this script directly to the Rigidbody
                thisRigidbody.velocity = movementVelocity;

                
            }
        }
    }

    void CharMovement()
    {
        #region Twin Stick Character Movement

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
        #endregion
    }

    void Shooting()
    {
        #region Shooting and Reloading
        // If right trigger is pressed...
        if (Input.GetAxis(fireButton) > 0 && !reloading && !pickUpMode)
        {
            if (InitAmmoAmount > 0)
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
                    InitAmmoAmount = InitAmmoAmount - 1;
                }
            }
        }
        else
        {
            //Stops delay between pressing the fire button and the shot firing
            //Makes it so shotTimer is only active whilst the fire button is down
            shotTimer = 0;
        }

        if (Input.GetButtonDown(reload) && !pickUpMode && InitAmmoAmount < ammoAmount || InitAmmoAmount == 0)
        {
            reloading = true;
            //playerUILink.reloadBarImage.enabled = true;
            //playerUILink.reloadingText.enabled = true;
        }

        if (reloading)
        {
            ReloadTime -= Time.deltaTime;

            if (ReloadTime <= 0)
            {

                InitAmmoAmount = ammoAmount;
                shotTimer = 0;
                ReloadTime = 1;
                reloading = false;
            }
        }
        #endregion
    }

    void GrabAndDrag()
    {
        #region Grabbing and Dragging
        //Left Trigger picks up player when held
        if (Input.GetAxisRaw(pickUp) > 0 && !pickUpMode && inRange && !ragdolling)
        {
            pickUpMode = true;
            weapon.SetActive(false);
        }
        else if (Input.GetAxisRaw(pickUp) == 0 && pickUpMode)
        {
            pickUpScript.join = false;
            Destroy(pickUpScript.GetComponent<SpringJoint>());
            pickUpMode = false;
            pointToGrab.GetComponentInParent<PlayerController>().beenDragged = true;
            weapon.SetActive(true);
        }

        //Current Setup for picking up player
        if (pickUpMode && inRange && !pickUpScript.join)
        {
            pickUpScript.join = false;
            pickUpScript.CreateJoint();
        }
        #endregion
    }

    void ButtonMashing()
    {
        #region Button Mashing 

        //Button mashing if the player is just ragdolling
        if (ragdolling && !beenDragged && Input.GetButtonDown(mashButton))
        {
            if (TotalCurrentMashes >= (numToMash - 1))
            {
                BreakDragging();
                Ragdoll(false);
            }
            else
            {
                TotalCurrentMashes++;
            }
        }

        //Button mashing if the player is been dragged by an opposing player
        if (beenDragged && Input.GetButtonDown(mashButton))
        {
            if (TotalCurrentMashes >= (numToMash - 1))
            {
                BreakDragging();
                Ragdoll(false);
            }
            else
            {
                TotalCurrentMashes++;
            }
        }
        #endregion
    }

    //Used to calculate player directions for animation blend trees
    void GetCharDirections()
    {        
        #region Player Directions
        forwardBackward = Vector3.Dot(movementVelocity.normalized, thisTransform.forward.normalized);

        if (forwardBackward > 0)
        {
            //Debug.Log(forwardBackward);
            // forward
        }
        else if (forwardBackward < 0)
        {
            //Debug.Log(forwardBackward);
            // backward
        }
        else
        {
            // neither
        }

        rightLeft = Vector3.Dot(-movementVelocity.normalized, Vector3.Cross(thisTransform.forward, thisTransform.up).normalized);

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
    }

    /// <summary>
    /// If the player is been dragged call this when the player can break from it.
    /// Works by breaking the joint that is connected to the player.
    /// And currently set ths health to full
    /// </summary>
    private void BreakDragging()
    {
        beenDragged = false;
        GetComponent<PlayerHealthManager>().CurrentHealth = GetComponent<PlayerHealthManager>().startingHealth;
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
            GetComponent<CapsuleCollider>().enabled = false;
        }

        else if (!ragdollToggle) //Reset Ragdoll
        {
            ragdolling = false;
            thisTransform.position = new Vector3(thisPlayersHips.position.x, 0 , thisPlayersHips.position.z);
            TotalCurrentMashes = 0;
            numToMash = numToMash * numToMashMultiplier;
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
            GetComponent<CapsuleCollider>().enabled = true;
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

    internal void SetUpInputs(int controller)
    {

        playerNum = controller;

        leftStickHorizontal = "L_Horizontal_" + playerNum.ToString();
        leftStickVertical = "L_Vertical_" + playerNum.ToString();
        rightStickHorizontal = "R_Horizontal_" + playerNum.ToString();
        rightStickVertical = "R_Vertical_" + playerNum.ToString();
        fireButton = "Fire_" + playerNum.ToString();
        pickUp = "PickUp_" + playerNum.ToString();
        reload = "Reload_" + playerNum.ToString();
        mashButton = "Mash_" + playerNum.ToString();
        dodgeButton = "B_" + playerNum.ToString();
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Dodge Point")
        {
            dodging = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            dodging = false;
            canDodge = false;
            Debug.Log("Obstacle Hit");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            canDodge = true;
        }
    }

    #region Getters/ Setters

    public int TotalCurrentMashes { get { return totalCurrentMashes; } set { totalCurrentMashes = value; } }

    public float InitAmmoAmount { get { return initAmmoAmount; } set { initAmmoAmount = value; } }

    public float ReloadTime { get { return reloadTime; } set { reloadTime = value; } }

    public bool PlayerInGame { get { return playerInGame; } set { playerInGame = value; } }

    #endregion
}
