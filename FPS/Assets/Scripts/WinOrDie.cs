using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using UnityEngine.SceneManagement;

public class WinOrDie : MonoBehaviour
{
    public int playerId;
    private int game_id;
    private SocketIOComponent socket;

    [SerializeField]
    Text result;

    // Start is called before the first frame update
    void Start()
    {
        Corridor_Music.Instance.gameObject.GetComponent<AudioSource>().Play();

        playerId = PlayerPrefs.GetInt("playerId");
        game_id = PlayerPrefs.GetInt("game_id");

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        string res = PlayerPrefs.GetString("result");
        if (res.Equals("win"))
        {
            result.text = "You WIN!!";
        }
        else if (res.Equals("lose"))
        {
            result.text = "You Lose..";
        }
        else
        {
            result.text = "Tie";
        }
    }

    public void OnExit()
    {
        SceneManager.LoadScene("GameCorridor");
    }
}
