using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Rigidbody thisRigidbody;

    private Vector3 movementInput;
    public Vector3 movementVelocity;

    private float shotTimer;
    private float playerTurnSpeed = 8;
    private float ammoAmount = 30;
    private float reloadTime = 1;

    private bool reloading;
    public bool canControl;
    public bool isDead;

    [SerializeField] float fireRate;
    [SerializeField] float movementSpeed = 4;

    [Header("Controller Inputs")]
    [Tooltip("Enter the same name that is found within the input manager of Unity")]

    [SerializeField] string leftStickHorizontal;
    [SerializeField] string leftStickVertical;
    [SerializeField] string rightStickHorizontal;
    [SerializeField] string rightStickVertical;
    [SerializeField] string fireButton;
    [SerializeField] string fineAim;
    [SerializeField] string reload;

    public GameObject bullet;
    public GameObject bulletSpawn;
    public GameObject laserSight;

    public Transform thisTransform;

    public Text ammoCountText;
    public Image reloadBarImage;
    public Text reloadingText;

    public Vector3 playerDirection;

    public ParticleSystem muzzleFlash;

    private void Start() {

        thisTransform = GetComponent<Transform>();
        thisRigidbody = GetComponent<Rigidbody>();
        shotTimer = fireRate;
        reloading = false;
        canControl = true;
        isDead = false;

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

            Debug.Log(forwardBackward);
            // forward
        }
        else if (forwardBackward < 0) {

            Debug.Log(forwardBackward);
            // backward
        }
        else {
            // neither
        }

        float rightLeft = Vector3.Dot(-movementVelocity.normalized, Vector3.Cross(thisTransform.forward, thisTransform.up).normalized);

        if (rightLeft > 0)
        {
            Debug.Log(rightLeft);
            // right
        }
        else if (rightLeft < 0)
        {
            Debug.Log(rightLeft);
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
                if (Input.GetAxis(fireButton) > 0 && !reloading)
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

                if (Input.GetButton(reload) && ammoAmount < 30 || ammoAmount == 0)
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
                #endregion
            }
        }     
    }

    private void FixedUpdate() {

        if(!isDead) {
            if(canControl) {

                // Applies velocity from this script directly to the Rigidbody
                thisRigidbody.velocity = movementVelocity;
            }
        }       
    }
}
