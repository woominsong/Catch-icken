using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SocketIO;
using Leguar.TotalJSON;

public class SocketManager : MonoBehaviour
{
    
    [HideInInspector]
    public SocketIOComponent socket;

    //for Attack class
    AttackOrCatch attackOrCatch;

    private char[] trim = { '"' };

    void Start()
    {
        attackOrCatch = GetComponentInChildren<AttackOrCatch>();

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        Debug.Log("socketmanager"+socket);

        socket.On("connected", (SocketIOEvent e) => {
            Debug.Log("connected");
            
            Dictionary<string, string> send_data = new Dictionary<string, string>();
            send_data["client_id"] = PlayerPrefs.GetInt("client_id").ToString();
            send_data["nickname"] = PlayerPrefs.GetString("userName").ToString();
            socket.Emit("update_sid", new JSONObject(send_data));

            send_data = new Dictionary<string, string>();
            send_data["game_id"] = PlayerPrefs.GetInt("game_id").ToString();
            socket.Emit("game_ready", new JSONObject(send_data));
        });

        socket.On("move", (SocketIOEvent e) => {
            Debug.Log("move");
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                //Debug.Log("move5");
                if (om[i].playerId == pId)
                {
                    om[i].oppMove(new Vector3(float.Parse(data["x"].CreateString().Trim(trim)), float.Parse(data["y"].CreateString().Trim(trim)), float.Parse(data["z"].CreateString().Trim(trim))), float.Parse(data["ry"].CreateString().Trim(trim)));
                    break;
                }
            }

        });

        socket.On("fix_position", (SocketIOEvent e) => {
            Debug.Log("fix_position");
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                //Debug.Log("move5");
                if (om[i].playerId == pId)
                {
                    om[i].transform.position = new Vector3(float.Parse(data["x"].CreateString().Trim(trim)), float.Parse(data["y"].CreateString().Trim(trim)), float.Parse(data["z"].CreateString().Trim(trim)));
                    om[i].transform.eulerAngles = new Vector3(0, float.Parse(data["ry"].CreateString().Trim(trim)),0);
                    break;
                }
            }

        });

        socket.On("attack", (SocketIOEvent e) => {
            Debug.Log("attack");
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                //Debug.Log("move5");
                if (om[i].playerId == pId)
                {
                    om[i].oppAttack();
                    break;
                }
            }
            //Debug.Log("attack1");
            Vector3 pos = new Vector3(float.Parse(data["x"].CreateString().Trim(trim)), float.Parse(data["y"].CreateString().Trim(trim)), float.Parse(data["z"].CreateString().Trim(trim)));
            //Debug.Log(data["vx"]);
            //Debug.Log(data["vx"].CreateString());
            //Debug.Log(data["vx"].CreateString().Trim(trim));
            //Debug.Log(float.Parse(data["vx"].CreateString().Trim(trim)));
            Vector3 v = new Vector3(float.Parse(data["vx"].CreateString().Trim(trim)), float.Parse(data["vy"].CreateString().Trim(trim)), float.Parse(data["vz"].CreateString().Trim(trim)));
            //Debug.Log("attack3");
            //Debug.Log("attack at ("+pos.x+","+pos.y+","+pos.z+ ") with velocity (" + v.x + "," + v.y + "," + v.z + ")");
            attackOrCatch.ShootAttack(pos, v);
        });

        socket.On("catch", (SocketIOEvent e) => {
            Debug.Log("catch");
            var data = JSON.ParseString(e.data.ToString());
            Vector3 pos = new Vector3(float.Parse(data["x"].CreateString().Trim(trim)), float.Parse(data["y"].CreateString().Trim(trim)), float.Parse(data["z"].CreateString().Trim(trim)));
            Vector3 v = new Vector3(float.Parse(data["vx"].CreateString().Trim(trim)), float.Parse(data["vy"].CreateString().Trim(trim)), float.Parse(data["vz"].CreateString().Trim(trim)));
            int pid = int.Parse(data["playerId"].CreateString().Trim(trim));
            Debug.Log("Player "+pid+" attack at (" + pos.x + "," + pos.y + "," + pos.z + ") with velocity (" + v.x + "," + v.y + "," + v.z + ")");
            attackOrCatch.ShootCatch(pos, v, pid);
        });

        socket.On("player_walk", (SocketIOEvent e) => {
            Debug.Log("player_walk");
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                //Debug.Log("move5");
                if (om[i].playerId == pId)
                {
                    om[i].oppWalk();
                    break;
                }
            }

        });

        socket.On("player_idle", (SocketIOEvent e) => {
            Debug.Log("player_idle");
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                //Debug.Log("move5");
                if (om[i].playerId == pId)
                {
                    om[i].oppIdle();
                    break;
                }
            }

        });
    }

}