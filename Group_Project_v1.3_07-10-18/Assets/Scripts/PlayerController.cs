using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(IKControl))]
public class PlayerController : MonoBehaviour {

    #region floats
    [Header("Variables")]
    [HideInInspector] public float movementSpeed = 4;
    [HideInInspector] public float jumpForce = 3.0f;
    [HideInInspector] public float numToMash = 5;

    [HideInInspector] public float playerNum;
    [HideInInspector] public float distanceCheck;
    [HideInInspector] public float forwardBackward;
    [HideInInspector] public float rightLeft;

    private float playerTurnSpeed = 20;
    //private float mashTimer;
    private float numToMashMultiplier = 2.0f;
    private float joystickAxisValue;  
    private float verticalVelocity;
    private float respawnTimer;
    private float respawnTimerReset = 3;
    #endregion

    #region ints
    [HideInInspector] public int totalCurrentMashes = 0;
    private int lives = 3;
    #endregion

    #region bools 
    [HideInInspector] public bool canControl;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool ragdolling;
    [HideInInspector] public bool canPickUpWeapon;
    [HideInInspector] public bool isHoldingWeapon;
    [HideInInspector] public bool beenDragged;
    [HideInInspector] public bool draggingPlayer;
    [HideInInspector] public bool pickUpMode;
    [HideInInspector] public bool inCountdown;

    private bool playerInGame;
    private bool isGrounded = false;
    private bool jumpingBool = false;  
    private bool inRange;
    [Space]
    #endregion

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
    [HideInInspector] public string bButton;
    [HideInInspector] public string jumpButton;
    [HideInInspector] public string interactButton;
    #endregion

    #region GameObjects
    [HideInInspector] public GameObject weapon;
    private GameObject closestPlayer;
    #endregion

    #region Vector3s
    private Vector3 movementInput;
    [HideInInspector] public Vector3 movementVelocity;
    [HideInInspector] public Vector3 playerDirection;
    [HideInInspector] public Vector3 startingPosition;
    #endregion

    private Transform thisTransform;
    private Rigidbody thisRigidbody;
    [HideInInspector] public Rigidbody rightHand;

    [HideInInspector] public PickUp pickUpScript;

    [HideInInspector] public PlayerHealthManager healthManager;

    [Header("Change These For Each Player")]
    public GameObject playerSkeletalMesh;
    public List<Transform> otherPlayersOrigin;
    public Transform thisPlayersOrigin;
    public PlayerUI playerUILink;

    void Awake()
    {
        thisTransform = GetComponent<Transform>();
        thisRigidbody = GetComponent<Rigidbody>();
        Instantiate(playerSkeletalMesh, thisTransform.position, thisTransform.rotation, thisTransform);
    }

    void Start()
    {
        healthManager = GetComponent<PlayerHealthManager>();
        canControl = true;
        isDead = false;
        //mashTimer = 0.5f;
        respawnTimer = respawnTimerReset;
        isHoldingWeapon = false;
        startingPosition = transform.position + new Vector3(0, 0.5f, 0);
        RagdollSetup();

        // Recursivly searches all children to find the right hand
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.name == "RightPalm")
            {
                rightHand = child.transform.GetComponent<Rigidbody>();
                pickUpScript = child.transform.GetComponent<PickUp>();
            }
        }
    }

    void Update()
    {
        // Only get values if moving
        if(thisRigidbody.velocity.x > 0 || thisRigidbody.velocity.x < 0 || thisRigidbody.velocity.z > 0 || thisRigidbody.velocity.z < 0)
        {
            GetCharDirections();
        }

        joystickAxisValue = Mathf.Clamp01(new Vector2(Input.GetAxis(leftStickHorizontal), Input.GetAxis(leftStickVertical)).sqrMagnitude);

        if (!isDead)
        {
            if (canControl)
            {
                CharMovement();
                GrabAndDrag();
                Jumping();

                #region Pick Up Weapon and Drop Weapon
                if (!isHoldingWeapon && canPickUpWeapon)
                {
                    if(Input.GetButtonDown(interactButton))
                    {
                        PickUpWeapon();
                        isHoldingWeapon = !isHoldingWeapon;
                    }
                }
                else if (isHoldingWeapon)
                {
                    if (Input.GetButtonDown(interactButton))
                    {
                        DropWeapon();
                        isHoldingWeapon = !isHoldingWeapon;
                        weapon.GetComponent<WeaponScript>().canDealDamage = false;
                    }

                    canPickUpWeapon = false;
                    weapon.GetComponent<WeaponScript>().Shoot(gameObject);
                    weapon.GetComponent<WeaponScript>().Reload(gameObject);
                }
                #endregion
            }

            if (ragdolling)
            {
                ButtonMashing();
            }

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
        }

        if (isDead)
        {
            #region Respawning
            respawnTimer -= Time.deltaTime;

            if(respawnTimer <= 0 && lives > 0)
            {
                transform.position = startingPosition;
                isDead = false;
                canControl = true;
                ragdolling = false;
                Ragdoll(false);
                numToMash = 5;
                healthManager.currentHealth = healthManager.startingHealth;
                respawnTimer = respawnTimerReset;
                lives = lives - 1;
            }
            #endregion
        }

        // Drop weapon if you get knocked out or die
        if (!canControl || isDead)
        {
            isHoldingWeapon = false;
            DropWeapon();
        }

        if (!draggingPlayer)
        {
            DistanceToPlayer();
        }
    }

    void CharMovement()
    {
        #region Character Movement
        // Configure input for left analog stick
        movementInput = new Vector3(Input.GetAxisRaw(leftStickHorizontal) * movementSpeed, 0, Input.GetAxisRaw(leftStickVertical) * movementSpeed);
        movementVelocity = movementInput;

        // When Left Trigger is pressed...
        if (Input.GetAxisRaw(aimButton) > 0)
        {
            movementSpeed = 3;

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
                movementSpeed = 4;
                transform.rotation = Quaternion.LookRotation(movementInput);
            }
        }
        #endregion
    }
   
    void ButtonMashing()
    {
        #region Button Mashing 

        // Button mashing if the player is just ragdolling
        if (!beenDragged && Input.GetButtonDown(mashButton))
        {
            if (totalCurrentMashes >= (numToMash - 1))
            {
                //BreakDragging();
                healthManager.currentHealth = healthManager.startingHealth;
                Ragdoll(false);
            }
            else
            {
                totalCurrentMashes++;
            }
        }

        // Button mashing if the player is been dragged by an opposing player
        if (beenDragged && Input.GetButtonDown(mashButton))
        {
            if (totalCurrentMashes >= (numToMash - 1))
            {
                BreakDragging();
                Ragdoll(false);
            }
            else
            {
                totalCurrentMashes++;
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
        healthManager.currentHealth = healthManager.startingHealth;
        rightHand.gameObject.GetComponent<PickUp>().join = false;
        Destroy(rightHand.gameObject.GetComponent<SpringJoint>());
        totalCurrentMashes = 0;
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
            thisRigidbody.isKinematic = true;
            GetComponent<Animator>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }

        else if (!ragdollToggle) //Reset Ragdoll
        {
            ragdolling = false;
            thisTransform.position = new Vector3(thisPlayersOrigin.position.x, 0 , thisPlayersOrigin.position.z);
            totalCurrentMashes = 0;
            numToMash = numToMash * numToMashMultiplier;
            canControl = true;
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }
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
            draggingPlayer = true;
        }
        else if (Input.GetAxisRaw(pickUp) == 0 && pickUpMode)
        {
            draggingPlayer = false;
            pickUpScript.join = false;
            Destroy(pickUpScript.GetComponent<SpringJoint>());
            pickUpMode = false;
        }

        // Current Setup for picking up player
        if (pickUpMode && inRange && !pickUpScript.join && ClosestPlayer.GetComponentInParent<PlayerController>().ragdolling)
        {
            pickUpScript.join = false;
            pickUpScript.CreateJoint();
        }
        #endregion
    }

    void PickUpWeapon ()
    {
        WeaponScript weaponScript = weapon.GetComponent<WeaponScript>();
        weaponScript.GetPickedUp(rightHand);
    }

    void DropWeapon()
    {
        if(weapon != null)
        {
            WeaponScript weaponScript = weapon.GetComponent<WeaponScript>();
            weaponScript.GetDropped();
        }   
    }

    internal void SetUpInputs(int controller)
    {
        playerNum = controller;

        leftStickHorizontal = "L_Horizontal_" + playerNum.ToString(); // Left Analogue Stick
        leftStickVertical = "L_Vertical_" + playerNum.ToString();
        rightStickHorizontal = "R_Horizontal_" + playerNum.ToString(); // Right Analogue Stick
        rightStickVertical = "R_Vertical_" + playerNum.ToString();
        fireButton = "Fire_" + playerNum.ToString(); // RT
        aimButton = "Aim_" + playerNum.ToString(); // LT
        pickUp = "PickUp_" + playerNum.ToString(); // LB
        reload = "Reload_" + playerNum.ToString(); // X
        mashButton = "Mash_" + playerNum.ToString(); // Y
        bButton = "B_" + playerNum.ToString(); // B
        jumpButton = "A_" + playerNum.ToString(); // A
        interactButton = "Interact_" + playerNum.ToString(); // RB
    }

    void OnTriggerStay(Collider col)
    {
        if (!isHoldingWeapon)
        {
            if (col.gameObject.tag == "Weapon")
            {
                weapon = col.transform.parent.gameObject;
                canPickUpWeapon = true;
            }
        }
    }

    void OnTriggerExit (Collider col)
    {
        if(!isHoldingWeapon)
        {
            this.weapon = null;
            canPickUpWeapon = false;
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
        if (collision.gameObject.layer == 11)
        {
            isGrounded = false;
        }
    }

    #region Getters/ Setters
    public bool PlayerInGame { get { return playerInGame; } set { playerInGame = value; } }
    public GameObject ClosestPlayer { get { return closestPlayer; } set { closestPlayer = value; } } 
    #endregion
}
