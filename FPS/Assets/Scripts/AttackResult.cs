using UnityEngine;

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
            PlayerMove tmp = other.GetComponent<PlayerMove>();
            tmp.health -= 20;
            tmp.healthBar.UpdateEnergybar(tmp.health);
        }
    }
}
