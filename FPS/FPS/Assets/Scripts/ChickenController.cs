using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenController : MonoBehaviour
{
    // Start is called before the first frame update

    public int chickenId;
    public bool chickenLive = false;
    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        cc.Move(-transform.up * 0.8f);
    }
}
