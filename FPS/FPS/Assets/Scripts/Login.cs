using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SocketIO;
using Leguar.TotalJSON;

public class Login : MonoBehaviour
{
    [HideInInspector]
    public SocketIOComponent socket;

    [SerializeField]
    private Text noName;

    private bool connected = false;
    private char[] trim = { '"' };

    private void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        Debug.Log("GameCorridor script started");

        socket.On("connected", (SocketIOEvent e) => {
            connected = true;
        });

        socket.On("res_login", (SocketIOEvent e) => {
            int id = int.Parse((JSON.ParseString(e.data.ToString())["client_id"]).CreateString().Trim(trim));
            PlayerPrefs.SetInt("client_id", id);
            SceneManager.LoadScene("GameCorridor");
        });
    }

    public void onSubmit()
    {
        if (!connected) return;

        InputField iF = FindObjectOfType<InputField>();
        var userName = iF.text;

        if (userName == "")
        {
            noName.color = new Color(noName.color.r, noName.color.g, noName.color.b, 1);
        }
        else
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("userName", userName);

            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["userName"] = PlayerPrefs.GetString("userName");
            socket.Emit("login", new JSONObject(send_data));
        }
    }
}
