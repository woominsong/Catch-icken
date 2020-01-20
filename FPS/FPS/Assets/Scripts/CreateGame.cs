using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SocketIO;
using Leguar.TotalJSON;

public class CreateGame : MonoBehaviour
{
    [SerializeField]
    private Text noName;

    [HideInInspector]
    public SocketIOComponent socket;

    private char[] trim = { '"' };

    private void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        Debug.Log("CreateGame script started");

        socket.On("connected", (SocketIOEvent e) => {
            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["client_id"] = PlayerPrefs.GetInt("client_id").ToString();
            send_data["nickname"] = PlayerPrefs.GetString("userName").ToString();
            socket.Emit("update_sid", new JSONObject(send_data));
        });

        socket.On("confirm_join", (SocketIOEvent e) => {
            Debug.Log("confirm_join recieved");
            var data = JSON.ParseString(e.data.ToString());
            Debug.Log("123");

            Debug.Log("Move to waiting room of game " + data["game_id"].CreateString().Trim(trim));
            PlayerPrefs.SetInt("game_id", int.Parse(data["game_id"].CreateString().Trim(trim)));
            socket.Emit("rm_list");
        });

        socket.On("removed", (SocketIOEvent e) => {
            Debug.Log("removed recieved");
            PlayerPrefs.SetInt("playerId",1);
            SceneManager.LoadScene("WaitingRoom");
        });
    }

    public void onSubmit()
    {
        InputField iF = FindObjectOfType<InputField>();
        var gameName = iF.text;

        if (gameName == "")
        {
            noName.color = new Color(noName.color.r, noName.color.g, noName.color.b, 1);
        }

        else
        {
            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["game_name"] = gameName;
            send_data["client_id"] = PlayerPrefs.GetInt("client_id").ToString();
            socket.Emit("create_game", new JSONObject(send_data));
        }
    }
}
