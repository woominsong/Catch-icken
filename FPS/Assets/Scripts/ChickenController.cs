using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenController : MonoBehaviour
{
    // Start is called before the first frame update

    public int chickenId;
    public bool chickenLive;
    private CharacterController cc;

    private ChickenSpawner chickenSpawner;

    private float timer;

    public float newTarget;

    void Start()
    {
        chickenSpawner = FindObjectOfType<ChickenSpawner>();
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        cc.Move(-transform.up * 0.8f);

        timer += Time.deltaTime;
        if(timer < newTarget)
        {
            cc.Move(transform.forward * 0.02f);
        }
        if(timer >= newTarget)
        {
            var tmp = gameObject.GetComponent<Transform>().rotation;
            tmp.Set(tmp.x, tmp.y + 180f, tmp.z, tmp.w);
            timer = 0;
        }
    }

    public void CatchChicken()
    {
        this.gameObject.SetActive(false);
        chickenLive = false;
        chickenSpawner.currentNumberOfChicken -= 1;
    }

    public void RespawnChicken()
    {
        this.gameObject.SetActive(true);
        chickenLive = true;
        chickenSpawner.currentNumberOfChicken += 1;
    }
}
