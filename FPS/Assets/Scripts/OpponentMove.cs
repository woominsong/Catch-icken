using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpponentMove : MonoBehaviour
{
    [SerializeField]
    public int playerId;

    // Player move variables.
    private CharacterController cc; // Reference to attached CharacterController.

    Animator anim;

    //for Attack class
    Attack m_attack;


    private void Start()
    {
        // initialize values
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        m_attack = GetComponent<Attack>();
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