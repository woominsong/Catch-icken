using UnityEngine;
using UnityEngine.UI;

public class AttackResult : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            Debug.Log("Player is Attacked");
            PlayerMove tmp = other.GetComponent<PlayerMove>();
            tmp.health -= 20;
            tmp.healthBar.UpdateEnergybar(tmp.health);
        }
        if(other.tag == "Container")
        {
            Debug.Log("My Container is Attacked");
            if(other.GetComponentInChildren<ContanerId>().contanerId == FindObjectOfType<PlayerMove>().playerId)
            {
                other.GetComponentInChildren<Slider>().value -= 0.2f;
            }
        }
    }
}
