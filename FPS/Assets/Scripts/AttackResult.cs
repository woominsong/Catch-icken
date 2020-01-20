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
            if(other.GetComponentInChildren<ContanerId>().containerId == FindObjectOfType<PlayerMove>().playerId) //내 컨테이너가 공격당한 정보를 처리
            {
                other.GetComponentInChildren<ContanerId>().AttackContainerHP();
            }
        }
    }
}
