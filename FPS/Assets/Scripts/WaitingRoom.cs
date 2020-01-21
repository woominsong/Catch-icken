using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using Leguar.TotalJSON;
using UnityEngine.SceneManagement;

public class WaitingRoom : MonoBehaviour
{
    [SerializeField]
    GameObject p1;
    [SerializeField]
    Text p1_name;
    [SerializeField]
    GameObject p2;
    [SerializeField]
    Text p2_name;

    [HideInInspector]
    public SocketIOComponent socket;

    private char[] trim = { '"' };

    private bool canStart = false;

    private void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        Debug.Log("CreateGame script started");

        p1.SetActive(false);
        p2.SetActive(false);

        socket.On("connected", (SocketIOEvent e) =>
        {
            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["client_id"] = PlayerPrefs.GetInt("client_id").ToString();
            send_data["nickname"] = PlayerPrefs.GetString("userName").ToString();
            socket.Emit("update_sid", new JSONObject(send_data));

            send_data = new Dictionary<string, string>();
            send_data["game_id"] = PlayerPrefs.GetInt("game_id").ToString();
            socket.Emit("require_game_info", new JSONObject(send_data));
        });

        socket.On("res_game_info", (SocketIOEvent e) =>
        {
            var data = JSON.ParseString(e.data.ToString());
            JArray player_names = (JArray)data["player_names"];
            int pNum = player_names.Length;

            Debug.Log(player_names.CreatePrettyString());
            
            p1.SetActive(true);
            p1_name.text = player_names[0].CreateString().Trim(trim);
            p1_name.color = new Color(p1_name.color.r, p1_name.color.g, p1_name.color.b,1);

            if (pNum == 2)
            {
                p2.SetActive(true);
                p2_name.text = player_names[1].CreateString().Trim(trim);
                p2_name.color = new Color(p2_name.color.r, p2_name.color.g, p2_name.color.b,1);
                canStart = true;
            }
        });

        socket.On("opponent_join", (SocketIOEvent e) =>
        {
            p2.SetActive(true);
            p2_name.text = JSON.ParseString(e.data.ToString())["name"].CreateString().Trim(trim);
            p2_name.color = new Color(p2_name.color.r, p2_name.color.g, p2_name.color.b, 1);
            canStart = true;
        });

        socket.On("opponent_exit", (SocketIOEvent e) =>
        {
            p2.SetActive(false);
            p2_name.color = new Color(p2_name.color.r, p2_name.color.g, p2_name.color.b, 0);
            canStart = false;
        });

        socket.On("exit", (SocketIOEvent e) =>
        {
            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["tag"] = "exit";
            socket.Emit("rm_list", new JSONObject(send_data));
        });

        socket.On("start_game", (SocketIOEvent e) =>
        {
            onGameStart();
        });

        socket.On("removed", (SocketIOEvent e) =>
        {
            string tag = JSON.ParseString(e.data.ToString())["tag"].CreateString().Trim(trim);
            if (tag.Equals("exit"))
            {
                SceneManager.LoadScene("GameCorridor");
            }
            else if (tag.Equals("start"))
            {
                Dictionary<string, string> send_data = new Dictionary<string, string>();
                send_data["game_id"] = PlayerPrefs.GetInt("game_id").ToString();
                socket.Emit("start_game", new JSONObject(send_data));
                SceneManager.LoadScene("CharacterScene");
            }
            else
            {
                Debug.Log("Weird tag: " + tag);
            }
        });
    }

    public void OnExit()
    {
        if (PlayerPrefs.GetInt("playerId") == 1)
        {
            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["game_id"] = PlayerPrefs.GetInt("game_id").ToString();
            socket.Emit("remove_game", new JSONObject(send_data));
        }
        else
        {
            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["game_id"] = PlayerPrefs.GetInt("game_id").ToString();
            send_data["client_id"] = PlayerPrefs.GetInt("client_id").ToString();
            socket.Emit("exit_game", new JSONObject(send_data));
        }
    }

    public void onGameStart()
    {
        if (!canStart) return;
        Dictionary<string, string> send_data = new Dictionary<string, string>();
        send_data["tag"] = "start";
        socket.Emit("rm_list", new JSONObject(send_data));
    }
}
