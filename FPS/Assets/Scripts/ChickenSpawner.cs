using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    public GameObject chickenPrefab;

    public int numberOfChicken = 20;

    public int currentNumberOfChicken = 0;

    // Start is called before the first frame update
    void Start()
    {
        SpawnChickens();
    }

    public void SpawnChickens()
    {
        var needChickenNum = numberOfChicken - currentNumberOfChicken;
        for (int i = 0; i < needChickenNum / 2; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(-71f, -28f),
                15,
                Random.Range(-25f, 20f));

            var spawnRotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0, 180), 0.0f));

            var chicken = (GameObject)Instantiate(chickenPrefab, spawnPosition, spawnRotation);

            chicken.GetComponent<ChickenController>().chickenId = i;
            chicken.GetComponent<ChickenController>().chickenLive = true;
        }

        for (int i = needChickenNum / 2; i < needChickenNum; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(-28f, 15f),
                15,
                Random.Range(-70f, -25f));
            var spawnRotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0, 180), 0.0f));

            var chicken = (GameObject)Instantiate(chickenPrefab, spawnPosition, spawnRotation);

            chicken.GetComponent<ChickenController>().chickenId = i;
            chicken.GetComponent<ChickenController>().chickenLive = true;
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
