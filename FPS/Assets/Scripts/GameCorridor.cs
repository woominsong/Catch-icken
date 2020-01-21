using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SocketIO;
using Leguar.TotalJSON;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["client_id"] = PlayerPrefs.GetInt("client_id").ToString();
            send_data["nickname"] = PlayerPrefs.GetString("userName").ToString();
            socket.Emit("update_sid", new JSONObject(send_data));
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
                        send_data["client_id"] = "" + PlayerPrefs.GetInt("client_id");
                        socket.Emit("join_game", new JSONObject(send_data));
                    }
                });
            }
        });

        socket.On("confirm_join", (SocketIOEvent e) => {
            Debug.Log("confirm_join recieved");
            var data = JSON.ParseString(e.data.ToString());

            Debug.Log("Move to waiting room of game " + data["game_id"].CreateString().Trim(trim));
            PlayerPrefs.SetInt("game_id",int.Parse(data["game_id"].CreateString().Trim(trim)));
            PlayerPrefs.SetInt("playerId", 2);
            socket.Emit("rm_list");
            SceneManager.LoadScene("WaitingRoom");
        });

        socket.On("game_full", (SocketIOEvent e) => {
            Debug.Log("game_full recieved");
            SceneManager.LoadScene("GameCorridor");
        });

        socket.On("removed", (SocketIOEvent e) => {
            Debug.Log("removed recieved");
            SceneManager.LoadScene("CreateGame");
        });
    }

    public void createGame()
    {
        socket.Emit("rm_list", new JSONObject(new Dictionary<string, string>()));
    }

}