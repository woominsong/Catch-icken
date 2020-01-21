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

        socket.On("spawn_chickens", (SocketIOEvent e) => {
            Debug.Log("spawn_chickens");
            var data = JSON.ParseString(e.data.ToString());
            var cs = GetComponent<ChickenSpawner>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));

            Debug.Log("spawn_chickens 1");
            ArrayList x = (ArrayList)MyUtil.StringToObject(data["x"].CreateString().Trim(trim));
            Debug.Log("spawn_chickens 2");
            ArrayList z = (ArrayList)MyUtil.StringToObject(data["z"].CreateString().Trim(trim));
            Debug.Log("spawn_chickens 3");
            ArrayList ry = (ArrayList)MyUtil.StringToObject(data["ry"].CreateString().Trim(trim));
            Debug.Log("spawn_chickens 4");
            ArrayList cid = (ArrayList)MyUtil.StringToObject(data["cid"].CreateString().Trim(trim));
            Debug.Log("spawn_chickens 5");

            Debug.Log(x.Count);

            for (int i = 0; i < x.Count; i++)
            {
                Debug.Log("create:");
                Debug.Log((float)x[i]);
                Debug.Log((float)z[i]);
                Debug.Log((int)ry[i]);
                Debug.Log((int)cid[i]);
                cs.CreateChicken((float)x[i], (float)z[i], (int)ry[i], (int)cid[i]);
            }
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

        socket.On("player_hit", (SocketIOEvent e) => {
            Debug.Log("player_hit");
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                if (om[i].playerId == pId)
                {
                    om[i].oppHit();
                    break;
                }
            }
        });

        socket.On("container_hit", (SocketIOEvent e) => {
            Debug.Log("container_hit");
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                if (om[i].playerId == pId)
                {
                    om[i].oppHit();
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
            Vector3 pos = new Vector3(float.Parse(data["x"].CreateString().Trim(trim)), float.Parse(data["y"].CreateString().Trim(trim)), float.Parse(data["z"].CreateString().Trim(trim)));
            Vector3 v = new Vector3(float.Parse(data["vx"].CreateString().Trim(trim)), float.Parse(data["vy"].CreateString().Trim(trim)), float.Parse(data["vz"].CreateString().Trim(trim)));
            
            attackOrCatch.ShootAttack(pos, v);
        });

        socket.On("catch", (SocketIOEvent e) => {
            Debug.Log("catch");
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pid = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                //Debug.Log("move5");
                if (om[i].playerId == pid)
                {
                    om[i].oppAttack();
                    break;
                }
            }
            Vector3 pos = new Vector3(float.Parse(data["x"].CreateString().Trim(trim)), float.Parse(data["y"].CreateString().Trim(trim)), float.Parse(data["z"].CreateString().Trim(trim)));
            Vector3 v = new Vector3(float.Parse(data["vx"].CreateString().Trim(trim)), float.Parse(data["vy"].CreateString().Trim(trim)), float.Parse(data["vz"].CreateString().Trim(trim)));

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