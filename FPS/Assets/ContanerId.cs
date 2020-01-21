using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class ContanerId : MonoBehaviour
{
    [HideInInspector]
    public SocketIOComponent socket;

    public int playerId;
    private int game_id;

    public int containerId;
    public int containerHP = 100;

    public bool isDestroyed = false;

    ChickenSpawner chickenSpawner;
    PlayerMove playerMove;
    Score score;
    
    void Start()
    {
        playerId = PlayerPrefs.GetInt("playerId");
        game_id = PlayerPrefs.GetInt("game_id");

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        chickenSpawner = FindObjectOfType<ChickenSpawner>();
        playerMove = FindObjectOfType<PlayerMove>();
        score = FindObjectOfType<Score>();
    }

    // Update is called once per frame
    void Update() 
    {
        if(containerHP == 0 && playerMove.playerId == containerId && !isDestroyed) // 자기 컨테이너만 파괴될때 처리
        {
            isDestroyed = true;
            chickenSpawner.RespawnChickens();
            //playerMove.playerRecord = 0;
            //score.ScoreUpdate();
            StartCoroutine(RefreshAfterTime(15f));
        }
    }

    public void AttackContainerHP() // 이미 참조 코드에서 처리되었기 때문에 내꺼인지 확인할 필요x
    {
        GetComponentInChildren<Slider>().value -= 0.2f;
        containerHP -= 20;
    }

    IEnumerator RefreshAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Dictionary<string, string> data = new Dictionary<string, string>();
        data["playerId"] = "" + playerId;
        data["game_id"] = "" + game_id;
        socket.Emit("container_restore", new JSONObject(data));
    }
}
