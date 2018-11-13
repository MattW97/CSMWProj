using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(IKControl))]
public class PlayerController : MonoBehaviour {

    [Header("Variables")]
    public float playerNum;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] float movementSpeed = 4;
    private float dodgeSpeed = 5;
    private float shotTimer;
    private float playerTurnSpeed = 20;
    private float initAmmoAmount = 4;
    private float ammoAmount;
    private float reloadTime = 1;
    private float mashTimer;
    public float numToMash = 5;
    private float numToMashMultiplier = 2.0f;
    private float joystickAxisValue;

    public float jumpForce = 3.0f;
    private float verticalVelocity;
    private float distToGround;
    private bool isGrounded = false;
    private bool jumpingBool = false;

    public float forwardBackward;
    public float rightLeft;
    public float distanceCheck;

    private int totalCurrentMashes = 0;
    
    private bool reloading;
    private bool dodging;
    private bool canDodge;
    private bool playerInGame;
    public bool canControl;
    public bool isDead;
    public bool beenDragged;
    public bool draggingPlayer;
    public bool ragdolling;
    
    public bool pickUpMode;
    private bool inRange;

    private bool firstDistanceCheck;
    [Space]

    #region Inputs
    [HideInInspector] public string leftStickHorizontal;
    [HideInInspector] public string leftStickVertical;
    [HideInInspector] public string rightStickHorizontal;
    [HideInInspector] public string rightStickVertical;
    [HideInInspector] public string fireButton;
    [HideInInspector] public string aimButton;
    [HideInInspector] public string pickUp;
    [HideInInspector] public string reload;
    [HideInInspector] public string mashButton;
    [HideInInspector] public string dodgeButton;
    [HideInInspector] public string jumpButton;
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
    [HideInInspector] public Transform thisTransform;
    private Rigidbody thisRigidbody;
    [Space]


    [HideInInspector] public Vector3 playerDirection;

    [Header("Particle Systems")]
    public ParticleSystem muzzleFlash;
    [Space]

    [Header("Scripts")]
    public UtilityManager utilManagerScript;


    [Header("Change These For Each Player")]
    public Transform thisPlayersOrigin;
    public List<Transform> otherPlayersOrigin;
    [Space]
    public Rigidbody rightHand;
    public PlayerUI playerUILink;
    public PickUp pickUpScript;
    private GameObject closestPlayer;

    void Start()
    {
        thisTransform = GetComponent<Transform>();
        thisRigidbody = GetComponent<Rigidbody>();
        ammoAmount = InitAmmoAmount;
        reloading = false;
        dodging = false;
        canDodge = true;
        canControl = true;
        isDead = false;
        mashTimer = 0.5f;

        distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;

        //StartDistance();
        RagdollSetup();      
    }

    void Update()
    {
        GetCharDirections();

        joystickAxisValue = Mathf.Clamp01(new Vector2(Input.GetAxis(leftStickHorizontal), Input.GetAxis(leftStickVertical)).magnitude);

        if (!isDead)
        {
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

                Shooting();
                GrabAndDrag();
                Jumping();
            }

            #region Dodge bool
            if (dodging)
            {
                dodgePos = dodgePoint.transform.position;
                dodgePoint.transform.parent = null;
                canControl = false;
                transform.position = Vector3.Lerp(transform.position, dodgePos, dodgeSpeed * Time.deltaTime);
            }
            else if (!dodging && !ragdolling)
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
        if(!draggingPlayer)
            DistanceToPlayer();
        
        //if(closestPlayer != null)
            //ClosestLimb();

        if (!isDead)
        {
            if(canControl)
            {
                CharMovement();
            }
        }
    }

    void CharMovement()
    {
        #region Character Movement

        print("move");

        // Configure input for left analog stick
        movementInput = new Vector3(Input.GetAxisRaw(leftStickHorizontal) * movementSpeed, 0, Input.GetAxisRaw(leftStickVertical) * movementSpeed);
        movementVelocity = movementInput;

        // When Left Trigger is pressed...
        if (Input.GetAxisRaw(aimButton) > 0)
        {

            // Configure input for right analog stick
            playerDirection = Vector3.right * Input.GetAxisRaw(rightStickHorizontal) - Vector3.forward * Input.GetAxisRaw(rightStickVertical);

            if (playerDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, targetRotation, playerTurnSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (movementInput != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(movementInput);
            }
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
            // Stops delay between pressing the fire button and the shot firing
            // Makes it so shotTimer is only active whilst the fire button is down
            shotTimer = 0;
        }

        if (Input.GetButtonDown(reload) && !pickUpMode && InitAmmoAmount < ammoAmount || InitAmmoAmount == 0)
        {
            reloading = true;
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

   
    void ButtonMashing()
    {
        #region Button Mashing 

        // Button mashing if the player is just ragdolling
        if (ragdolling && !beenDragged && Input.GetButtonDown(mashButton))
        {
            if (TotalCurrentMashes >= (numToMash - 1))
            {
                //BreakDragging();
                GetComponent<PlayerHealthManager>().CurrentHealth = GetComponent<PlayerHealthManager>().startingHealth;
                Ragdoll(false);
            }
            else
            {
                TotalCurrentMashes++;
            }
        }

        // Button mashing if the player is been dragged by an opposing player
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

    void Jumping()
    {
        if (isGrounded)
        {
            verticalVelocity = -Physics.gravity.y * Time.deltaTime;

            if (Input.GetButtonDown(jumpButton) && !jumpingBool)
            {
                jumpingBool = true;
                StartCoroutine(JumpGravity());
            }

            if (jumpingBool)
            {
                verticalVelocity = jumpForce;
            }

        }
        else if (!isGrounded)
        {
            verticalVelocity -= 15 * Time.deltaTime;
        }

        thisRigidbody.velocity = new Vector3(movementVelocity.x, verticalVelocity, movementVelocity.z);

    }

    IEnumerator JumpGravity()
    {
        yield return new WaitForSeconds(.1f);
        isGrounded = false;
        jumpingBool = false;
    }

    //Used to calculate player directions for animation blend trees
    void GetCharDirections()
    {        
        #region Player Directions
        forwardBackward = Vector3.Dot(movementVelocity.normalized, thisTransform.forward.normalized) * joystickAxisValue;

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

        rightLeft = Vector3.Dot(-movementVelocity.normalized, Vector3.Cross(thisTransform.forward, thisTransform.up).normalized) * joystickAxisValue;

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
        rightHand.gameObject.GetComponent<PickUp>().join = false;
        Destroy(rightHand.gameObject.GetComponent<SpringJoint>());
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
            // weapon.GetComponent<Rigidbody>().isKinematic = false;
            // weapon.GetComponent<BoxCollider>().enabled = true;
            weapon.gameObject.SetActive(false);
            thisRigidbody.isKinematic = true;
            GetComponent<Animator>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }

        else if (!ragdollToggle) //Reset Ragdoll
        {
            ragdolling = false;
            thisTransform.position = new Vector3(thisPlayersOrigin.position.x, 0 , thisPlayersOrigin.position.z);
            TotalCurrentMashes = 0;
            numToMash = numToMash * numToMashMultiplier;
            canControl = true;
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }
            weapon.GetComponent<Rigidbody>().isKinematic = true;
            // weapon.GetComponent<BoxCollider>().enabled = false;
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
        foreach (Transform otherPlayertrans in otherPlayersOrigin)
        {
            float dist = Vector3.Distance(otherPlayertrans.position, transform.position);
            
            if (dist < distanceCheck  && otherPlayertrans.root.GetComponent<PlayerController>().PlayerInGame)
            {
                inRange = true;
                ClosestPlayer = otherPlayertrans.gameObject;
                break;

                //GetComponent<IKControl>().lookObj = placeToLook;
                //GetComponent<IKControl>().ikActive = true;
                //pickUpScript = closestPlayer.GetComponentInChildren<PickUp>();
            }
            else
            {
                inRange = false;
                GetComponent<IKControl>().ikActive = false;

            }
        }   
    }

    void GrabAndDrag()
    {
        #region Grabbing and Dragging
        // Left Bumper picks up player when held
        if (Input.GetAxisRaw(pickUp) > 0 && !pickUpMode && inRange && !ragdolling)
        {
            pickUpMode = true;
            weapon.SetActive(false);
            draggingPlayer = true;
        }
        else if (Input.GetAxisRaw(pickUp) == 0 && pickUpMode)
        {
            draggingPlayer = false;
            pickUpScript.join = false;
            Destroy(pickUpScript.GetComponent<SpringJoint>());
            pickUpMode = false;
            weapon.SetActive(true);
        }

        // Current Setup for picking up player
        if (pickUpMode && inRange && !pickUpScript.join && ClosestPlayer.GetComponentInParent<PlayerController>().ragdolling)
        {
            pickUpScript.join = false;
            pickUpScript.CreateJoint();
        }
        #endregion
    }

    internal void SetUpInputs(int controller)
    {
        playerNum = controller;

        leftStickHorizontal = "L_Horizontal_" + playerNum.ToString();
        leftStickVertical = "L_Vertical_" + playerNum.ToString();
        rightStickHorizontal = "R_Horizontal_" + playerNum.ToString();
        rightStickVertical = "R_Vertical_" + playerNum.ToString();
        fireButton = "Fire_" + playerNum.ToString();
        aimButton = "Aim_" + playerNum.ToString();
        pickUp = "PickUp_" + playerNum.ToString();
        reload = "Reload_" + playerNum.ToString();
        mashButton = "Mash_" + playerNum.ToString();
        dodgeButton = "B_" + playerNum.ToString();
        jumpButton = "A_" + playerNum.ToString();
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

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            canDodge = true;
        }

        if (collision.gameObject.layer == 11)
        {
            isGrounded = false;
        }
    }

    #region Getters/ Setters

    public int TotalCurrentMashes { get { return totalCurrentMashes; } set { totalCurrentMashes = value; } }

    public float InitAmmoAmount { get { return initAmmoAmount; } set { initAmmoAmount = value; } }

    public float ReloadTime { get { return reloadTime; } set { reloadTime = value; } }

    public bool PlayerInGame { get { return playerInGame; } set { playerInGame = value; } }

    public GameObject ClosestPlayer { get { return closestPlayer; } set { closestPlayer = value; } }
    
    #endregion
}
