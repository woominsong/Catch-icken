using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCatchBall : MonoBehaviour
{
    public GameObject CatchEffect;
    public int playerId;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject prefab = Resources.Load("CatchEffect") as GameObject;
        prefab.GetComponent<CatchResult>().playerId = playerId;
        Instantiate(prefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
