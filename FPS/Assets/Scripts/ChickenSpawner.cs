using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChickenSpawner : MonoBehaviour
{
    public GameObject chickenPrefab;

    public int maxNumberOfChicken = 20;

    public int currentNumberOfChicken = 0;

    ChickenController[] chickenControllers;

    Text textNum;

    // Start is called before the first frame update
    void Start()
    {
        SpawnChickens();
        chickenControllers = FindObjectsOfType<ChickenController>();
        Text[] texts = FindObjectOfType<Canvas>().GetComponentsInChildren<Text>();
        foreach (Text t in texts)
        {
            if(t.name == "ChickenNumber")
            {
                textNum = t;
            }
        }
    }

    private void FixedUpdate()
    {
        textNum.text = currentNumberOfChicken.ToString();
    }

    public void SpawnChickens()
    {
        var needChickenNum = maxNumberOfChicken - currentNumberOfChicken;
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
        currentNumberOfChicken = maxNumberOfChicken;
    }

    public void RespawnChickens()
    {
        foreach(ChickenController chickenController in chickenControllers)
        {
            if(chickenController.chickenLive == false)
            {
                var spawnPosition = new Vector3(
                Random.Range(-28f, 15f),
                15,
                Random.Range(-70f, -25f))
                +
                new Vector3(
                Random.Range(-71f, -28f),
                15,
                Random.Range(-25f, 20f)); ;
                var spawnRotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0, 180), 0.0f));
                chickenController.GetComponent<Transform>().position = spawnPosition;
                chickenController.GetComponent<Transform>().rotation = spawnRotation;
                chickenController.RespawnChicken();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
