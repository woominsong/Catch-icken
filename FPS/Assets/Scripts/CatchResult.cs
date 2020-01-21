using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class CatchResult : MonoBehaviour
{
    public int playerId;
    private int game_id;
    private Score score;

    [HideInInspector]
    public SocketIOComponent socket;


    // Start is called before the first frame update
    void Start()
    {
        playerId = PlayerPrefs.GetInt("playerId");
        game_id = PlayerPrefs.GetInt("game_id");

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        score = FindObjectOfType<Score>();
        Destroy(gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var me = FindObjectOfType<PlayerMove>();
        var opponent = FindObjectOfType<OpponentMove>();

        if (other.tag == "Chicken")
        {
            if (me.playerId == playerId)
            {
                var chickenId = other.gameObject.GetComponent<ChickenController>().chickenId;
                //other.gameObject.GetComponent<ChickenController>().CatchChicken();
                me.playerRecord += 1;
                score.ScoreUpdate();

                //자기가 catch한 치킨에 대해서만 정보처리()
                //1. 치킨 없애는 함수 실행
                //2. 자신의 점수 올림
                //3. score update

                Dictionary<string, string> data = new Dictionary<string, string>();
                data["playerId"] = "" + playerId;
                data["game_id"] = "" + game_id;
                data["chickenId"] = "" + chickenId;
                socket.Emit("chicken_hit", new JSONObject(data));
            }
        }
    }
}
