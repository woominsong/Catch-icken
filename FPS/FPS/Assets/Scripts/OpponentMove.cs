using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpponentMove : MonoBehaviour
{
    // Player move variables.
    private CharacterController cc; // Reference to attached CharacterController.

    Animator anim;

    //for Attack class
    AttackOrCatch m_attack;

    private Slider healthSlider;
    
    private void Start()
    {
        // initialize values
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        m_attack = GetComponent<AttackOrCatch>();

        healthSlider = GetComponentInChildren<Slider>();
    }

    public void UpdateHealth(float updatedHealth)
    {
        //updateHealth 안에 깎인 현재 체력
        healthSlider.value = updatedHealth / 100f;

    }

    public void oppWalk()
    {
        anim.SetBool("isWalking", true);
        anim.SetBool("isIdle", false);
    }
    public void oppIdle()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isIdle", true);
    }
    public void oppAttack(Vector3 shootStartPoint, float shootVelocity)
    {
        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");
        m_attack.ShootAttack(shootStartPoint, shootVelocity);
    }

    public void oppMove(Vector3 displacement)
    {
        cc.Move(displacement);
    }

}