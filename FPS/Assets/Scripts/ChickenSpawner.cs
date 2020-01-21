using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class ChickenSpawner : MonoBehaviour
{
    public int playerId;
    private int game_id;
    public GameObject chickenPrefab;

    public int maxNumberOfChicken = 20;

    public int currentNumberOfChicken = 0;

    private SocketIOComponent socket;
    public ChickenController[] chickenControllers;

    Text textNum;

    // Start is called before the first frame update
    void Start()
    {
        playerId = PlayerPrefs.GetInt("playerId");
        game_id = PlayerPrefs.GetInt("game_id");

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        
        //if (playerId == 1)
        //{
        //    SpawnChickens();
        //}
        //SpawnChickens();
        
        
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

        ArrayList x = new ArrayList();
        ArrayList z = new ArrayList();
        ArrayList ry = new ArrayList();
        ArrayList cid = new ArrayList();

        for (int i = 0; i < needChickenNum / 2; i++)
        {
            x.Add(Random.Range(-71f, -28f));
            z.Add(Random.Range(-25f, 20f));
            ry.Add(Random.Range(0, 180));
            cid.Add(i);
        }

        for (int i = needChickenNum / 2; i < needChickenNum; i++)
        {
            x.Add(Random.Range(-28f, 15f));
            z.Add(Random.Range(-70f, -25f));
            ry.Add(Random.Range(0, 180));
            cid.Add(i);
        }

        Debug.Log("spawn chicken: player id is "+playerId);

        Dictionary<string, string> data = new Dictionary<string, string>();
        data["playerId"] = "" + playerId;
        data["game_id"] = "" + game_id;
        data["x"] = MyUtil.ObjectToString(x);
        data["z"] = MyUtil.ObjectToString(z);
        data["ry"] = MyUtil.ObjectToString(ry);
        data["cid"] = MyUtil.ObjectToString(cid);
        Debug.Log("emit spawn_chickens");
        socket.Emit("spawn_chickens", new JSONObject(data));
    }

    public void CreateChicken(float x, float z, int ry, int chickenId)
    {
        Debug.Log("chicken created at ("+x+",15,"+z+")");
        Vector3 pos = new Vector3(x,15,z);
        Vector3 rot = new Vector3(0,ry,0);
        var chicken = Instantiate(chickenPrefab, pos, Quaternion.Euler(rot));
        chicken.GetComponent<ChickenController>().chickenId = chickenId;
        chicken.GetComponent<ChickenController>().chickenLive = true;
    }

    public void RespawnChickens()
    {
        ArrayList x = new ArrayList();
        ArrayList z = new ArrayList();
        ArrayList ry = new ArrayList();
        ArrayList cid = new ArrayList();

        foreach (ChickenController chickenController in chickenControllers)
        {
            if(chickenController.chickenLive == false)
            {
                x.Add(Random.Range(-28f, 15f) + Random.Range(-71f, -28f));
                z.Add(Random.Range(-70f, -25f) + Random.Range(-25f, 20f));
                ry.Add(Random.Range(0, 180));
                cid.Add(chickenController.chickenId);

                /*var spawnPosition = new Vector3(
                Random.Range(-28f, 15f),
                15,
                Random.Range(-70f, -25f))
                +
                new Vector3(
                Random.Range(-71f, -28f),
                15,
                Random.Range(-25f, 20f));*/ 
            }
        }

        Dictionary<string, string> data = new Dictionary<string, string>();
        data["playerId"] = "" + playerId;
        data["game_id"] = "" + game_id;
        data["x"] = MyUtil.ObjectToString(x);
        data["z"] = MyUtil.ObjectToString(z);
        data["ry"] = MyUtil.ObjectToString(ry);
        data["cid"] = MyUtil.ObjectToString(cid);
        Debug.Log("emit respawn_chickens");
        socket.Emit("respawn_chickens", new JSONObject(data));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
