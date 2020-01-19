using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SocketIO;
using Leguar.TotalJSON;
using UnityEngine.UI;

public class GameCorridor : MonoBehaviour{
    
    [HideInInspector]
    public SocketIOComponent socket;

    [SerializeField]
    private GameObject listContent;
    [SerializeField]
    private GameObject elemPrefab;

    private char[] trim = { '"' };

    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        Debug.Log("GameCorridor script started");

        socket.On("connected", (SocketIOEvent e) => {
            socket.Emit("reqGamerooms");
        });

        socket.On("sendGamerooms", (SocketIOEvent e) => {
            Debug.Log("Gamerooms recieved");
            var data = JSON.ParseString(e.data.ToString());
            JArray arr = (JArray)data["res"];

            for (int i=0; i<arr.Length; i++)
            {
                string game_name = ((JSON)arr[i])["game_name"].CreateString().Trim(trim);
                int player_num = int.Parse(((JSON)arr[i])["player_num"].CreateString().Trim(trim));
                int game_id = int.Parse(((JSON)arr[i])["game_id"].CreateString().Trim(trim));
                Debug.Log("game_name: " + game_name + ", player_num: " + player_num + ", game_id: " + game_id);

                GameObject newElem = Instantiate(elemPrefab);
                newElem.transform.SetParent(listContent.transform, false);
                Button newButton = newElem.GetComponentInChildren<Button>();
                newButton.GetComponentInChildren<Text>().text = game_name;
                newElem.GetComponentsInChildren<Text>()[1].text = player_num + "/2";

                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener(() =>
                {
                    if(player_num < 2)
                    {
                        Dictionary<string, string> send_data = new Dictionary<string, string>();
                        send_data["game_id"] = "" + game_id;
                        socket.Emit("join_game", new JSONObject(send_data));

                        Debug.Log("Move to waiting room of "+game_name);
                        // TODO: move to game waiting room
                    }
                });
            }

        });
    }

}