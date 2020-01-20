using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SocketIO;

public class Test : MonoBehaviour{

    [SerializeField]
    GameObject go1;
    [SerializeField]
    GameObject go2;
    [SerializeField]
    GameObject go3;

    Animator anim1;
    Animator anim2;
    Animator anim3;

    private void Start()
    {
        anim1 = go1.GetComponent<Animator>();
        anim2 = go2.GetComponent<Animator>();
        anim3 = go3.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim1.ResetTrigger("left");
            anim2.ResetTrigger("left");
            anim3.ResetTrigger("left");
            anim1.SetTrigger("left");
            anim2.SetTrigger("left");
            anim3.SetTrigger("left");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            anim1.ResetTrigger("right");
            anim2.ResetTrigger("right");
            anim3.ResetTrigger("right");
            anim1.SetTrigger("right");
            anim2.SetTrigger("right");
            anim3.SetTrigger("right");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            anim1.ResetTrigger("jump");
            anim2.ResetTrigger("jump");
            anim3.ResetTrigger("jump");
            anim1.SetTrigger("jump");
            anim2.SetTrigger("jump");
            anim3.SetTrigger("jump");
        }
    }

}