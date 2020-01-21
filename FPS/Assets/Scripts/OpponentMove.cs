using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpponentMove : MonoBehaviour
{
    [SerializeField]
    public int playerId;
    public int playerRecord;

    // Player move variables.
    private CharacterController cc; // Reference to attached CharacterController.

    Animator anim;

    //for Attack class
    AttackOrCatch m_attack;

    //for Energy
    public float health;
    private Slider healthSlider;
    

    private void Start()
    {
        // Initialize player position
        playerId = PlayerPrefs.GetInt("playerId");
        game_id = PlayerPrefs.GetInt("game_id");
        
        health = 100;

        if (playerId == 1)
        {
            playerId = 2;
        }
        else
        {
            playerId = 1;
        }

        if (playerId == 1)
        {
            transform.position = GameSettings.p1StartPos;
            transform.rotation = GameSettings.p1StartRot;
        }
        else
        {
            transform.position = GameSettings.p2StartPos;
            transform.rotation = GameSettings.p2StartRot;
        }

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
    }
    public void oppIdle()
    {
        anim.SetBool("isWalking", false);
    }

    public void oppAttack()
    {
        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");
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

    public void oppHit()
    {
        health -= 20;
        UpdateHealth(health);
    }

}