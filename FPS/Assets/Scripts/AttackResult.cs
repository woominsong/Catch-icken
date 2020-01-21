using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SocketIO;
using Leguar.TotalJSON;
using UnityEngine.UI;


public class AttackResult : MonoBehaviour
{
    [HideInInspector]
    public SocketIOComponent socket;

    public int playerId;
    private int game_id;

    // Start is called before the first frame update
    void Start()
    {
        playerId = PlayerPrefs.GetInt("playerId");
        game_id = PlayerPrefs.GetInt("game_id");

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

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
            Debug.Log("Player is Hit");
            PlayerMove tmp = other.GetComponent<PlayerMove>();
            tmp.health -= 20;
            tmp.healthBar.UpdateEnergybar(tmp.health);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["playerId"] = "" + playerId;
            data["game_id"] = "" + game_id;
            socket.Emit("player_hit", new JSONObject(data));
        }
        if(other.tag == "Container")
        {
            Debug.Log("My Container is Hit");
            if(other.GetComponentInChildren<ContanerId>().containerId == FindObjectOfType<PlayerMove>().playerId)
            {
                other.GetComponentInChildren<Slider>().value -= 0.2f;

                Dictionary<string, string> data = new Dictionary<string, string>();
                data["playerId"] = "" + playerId;
                data["game_id"] = "" + game_id;
                socket.Emit("container_hit", new JSONObject(data));
            }
        }
    }
}
