using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackResult : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            PlayerMove tmp = other.GetComponent<PlayerMove>();
            tmp.health -= 20;
            tmp.healthBar.UpdateEnergybar(tmp.health);
        }
    }
}
