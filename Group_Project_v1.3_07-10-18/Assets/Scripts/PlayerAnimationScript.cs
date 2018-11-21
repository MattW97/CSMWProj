using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour {

    Animator animator;
    PlayerController playerController;

    [HideInInspector] public WeaponScript weapon;
    [HideInInspector] public bool canDealDamage;

    private AudioSource audioSource;
    //public AudioClip shotgunFire;
    public AudioClip baseballBatSwing;
    //public AudioClip malletSwing;
    //public AudioClip macheteSwing;

    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        canDealDamage = false;
	}
	
	// Update is called once per frame
	void Update ()
    {

        float h = playerController.rightLeft;
        float v = playerController.forwardBackward;

        animator.SetFloat("MoveRight", h);
        animator.SetFloat("MoveForward", v);

        weapon = playerController.weapon.GetComponent<WeaponScript>();

        if (!playerController.isHoldingWeapon)
        {
            // If unarmed
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 0);
            animator.SetLayerWeight(3, 0);
        }
        else
        {
            if (weapon.weaponSelection == WeaponScript.WeaponType.Shotgun)
            {               
                animator.SetLayerWeight(1, 0);
                animator.SetLayerWeight(2, 1);
                animator.SetLayerWeight(3, 0);
            }
            if (weapon.weaponSelection == WeaponScript.WeaponType.BaseballBat)
            {
                animator.SetLayerWeight(1, 1);
                animator.SetLayerWeight(2, 0);
                animator.SetLayerWeight(3, 0);

                if (Input.GetAxisRaw(playerController.fireButton) > 0 && animator.GetCurrentAnimatorStateInfo(1).IsName("BaseballBatMovementBlendTree"))
                {
                    animator.ResetTrigger("AttackTrigger");
                    animator.SetTrigger("AttackTrigger");

                    audioSource.PlayOneShot(baseballBatSwing, 0.5f);
                }

            }
            if (weapon.weaponSelection == WeaponScript.WeaponType.Mallet)
            {
                animator.SetLayerWeight(1, 1);
                animator.SetLayerWeight(2, 0);
                animator.SetLayerWeight(3, 0);

                if (Input.GetAxisRaw(playerController.fireButton) > 0 && animator.GetCurrentAnimatorStateInfo(1).IsName("BaseballBatMovementBlendTree"))
                {
                    animator.ResetTrigger("AttackTrigger");
                    animator.SetTrigger("AttackTrigger");

                    audioSource.PlayOneShot(baseballBatSwing, 0.5f);
                }
            }
            if (weapon.weaponSelection == WeaponScript.WeaponType.Machete)
            {               
                animator.SetLayerWeight(1, 0);
                animator.SetLayerWeight(2, 0);
                animator.SetLayerWeight(3, 1);

                if (Input.GetAxisRaw(playerController.fireButton) > 0 && animator.GetCurrentAnimatorStateInfo(3).IsName("MacheteMovementBlendTree"))
                {
                    animator.ResetTrigger("AttackTrigger");
                    animator.SetTrigger("AttackTrigger");

                    audioSource.PlayOneShot(baseballBatSwing, 0.5f);
                }
            }
        }      
    }

    public void DamageOn ()
    {
        canDealDamage = true;
    }

    public void DamageOff()
    {
        canDealDamage = false;
    }
}
