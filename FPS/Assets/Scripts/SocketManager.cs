using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using Leguar.TotalJSON;
using UnityEngine.SceneManagement;

public class SocketManager : MonoBehaviour
{
    [HideInInspector]
    public SocketIOComponent socket;

    //for Attack class
    AttackOrCatch attackOrCatch;
    private Score score;

    private char[] trim = { '"' };

    public int maxNumberOfChicken = 20;

    //audio
    [SerializeField]
    private Audio audio;
    [SerializeField]
    private Audio oppAudio;
    private void Start()
    {
        audio = FindObjectOfType<Audio>();

        Corridor_Music.Instance.gameObject.GetComponent<AudioSource>().Stop();

        attackOrCatch = GetComponentInChildren<AttackOrCatch>();
        score = FindObjectOfType<Score>();

        Text[] texts = FindObjectOfType<Canvas>().GetComponentsInChildren<Text>();

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

            audio.game.Play();

            var data = JSON.ParseString(e.data.ToString());
            var cs = GetComponent<ChickenSpawner>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));

            //Debug.Log("spawn_chickens 1");
            ArrayList x = (ArrayList)MyUtil.StringToObject(data["x"].CreateString().Trim(trim));
            //Debug.Log("spawn_chickens 2");
            ArrayList z = (ArrayList)MyUtil.StringToObject(data["z"].CreateString().Trim(trim));
            //Debug.Log("spawn_chickens 3");
            ArrayList ry = (ArrayList)MyUtil.StringToObject(data["ry"].CreateString().Trim(trim));
            //Debug.Log("spawn_chickens 4");
            ArrayList cid = (ArrayList)MyUtil.StringToObject(data["cid"].CreateString().Trim(trim));
            //Debug.Log("spawn_chickens 5");

            //Debug.Log(x.Count);

            for (int i = 0; i < x.Count; i++)
            {
                //Debug.Log("create:");
                //Debug.Log((float)x[i]);
                //Debug.Log((float)z[i]);
                //Debug.Log((int)ry[i]);
                //Debug.Log((int)cid[i]);
                cs.CreateChicken((float)x[i], (float)z[i], (int)ry[i], (int)cid[i]);
            }
            FindObjectOfType<ChickenSpawner>().chickenControllers = FindObjectsOfType<ChickenController>();
            FindObjectOfType<ChickenSpawner>().currentNumberOfChicken = maxNumberOfChicken;
        });

        socket.On("respawn_chickens", (SocketIOEvent e) => {
            Debug.Log("respawn_chickens");
            var data = JSON.ParseString(e.data.ToString());
            var cs = GetComponent<ChickenSpawner>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));

            ArrayList x = (ArrayList)MyUtil.StringToObject(data["x"].CreateString().Trim(trim));
            ArrayList z = (ArrayList)MyUtil.StringToObject(data["z"].CreateString().Trim(trim));
            ArrayList ry = (ArrayList)MyUtil.StringToObject(data["ry"].CreateString().Trim(trim));
            ArrayList cid = (ArrayList)MyUtil.StringToObject(data["cid"].CreateString().Trim(trim));

            for (int i = 0; i < x.Count; i++)
            {
                foreach (ChickenController chickenController in cs.chickenControllers)
                {
                    if (chickenController.chickenId == (int)cid[i])
                    {
                        var spawnRotation = Quaternion.Euler(new Vector3(0.0f, (int)ry[i], 0.0f));
                        chickenController.GetComponent<Transform>().position = new Vector3((float)x[i],15,(float)z[i]);
                        chickenController.GetComponent<Transform>().rotation = spawnRotation;
                        chickenController.RespawnChicken();
                    }
                }
            }
            Debug.Log("respawn_chickens 1");
            if (FindObjectOfType<PlayerMove>().playerId == pId)
            {
                Debug.Log("respawn_chickens 2");
                FindObjectOfType<PlayerMove>().playerRecord = 0;
            }
            else
            {
                Debug.Log("respawn_chickens 3");
                FindObjectOfType<OpponentMove>().playerRecord = 0;
            }
            score.ScoreUpdate();
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
            bool mine = true;
            for (int i = 0; i < om.Length; i++)
            {
                if (om[i].playerId == pId)
                {
                    om[i].oppHit();
                    oppAudio.player_hit.Play();
                    mine = false;
                    break;
                }
            }
            if (mine) audio.player_hit.Play();
        });

        socket.On("container_hit", (SocketIOEvent e) => {
            Debug.Log("container_hit");
            var data = JSON.ParseString(e.data.ToString());
            var containers = FindObjectsOfType<ContanerId>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < containers.Length; i++)
            {
                if (containers[i].containerId == pId)
                {
                    containers[i].AttackContainerHP();
                    if(pId == PlayerPrefs.GetInt("playerId"))
                    {
                        audio.container_hit.Play();
                    }
                    else
                    {
                        oppAudio.container_hit.Play();
                    }
                }
            }
        });

        socket.On("dead", (SocketIOEvent e) => {
            Debug.Log("dead");
            oppAudio.player_die.Play();
            var data = JSON.ParseString(e.data.ToString());
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < om.Length; i++)
            {
                if (om[i].playerId == pId)
                {
                    om[i].oppDie();
                    
                    break;
                }
            }
        });

        socket.On("container_restore", (SocketIOEvent e) => {
            Debug.Log("container_restore");
            var data = JSON.ParseString(e.data.ToString());
            var containers = FindObjectsOfType<ContanerId>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            for (int i = 0; i < containers.Length; i++)
            {
                if (containers[i].containerId == pId)
                {
                    containers[i].containerHP = 100;
                    containers[i].GetComponentInChildren<Slider>().value = 1;
                    containers[i].isDestroyed = false;
                }
            }
        });

        socket.On("chicken_hit", (SocketIOEvent e) => {
            Debug.Log("chicken_hit");
            var data = JSON.ParseString(e.data.ToString());
            var chickens = FindObjectsOfType<ChickenController>();
            var om = GetComponentsInChildren<OpponentMove>();
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            int cId = int.Parse(data["chickenId"].CreateString().Trim(trim));

            bool mine = true;

            for (int i = 0; i < chickens.Length; i++)
            {
                if (chickens[i].chickenId == cId)
                {
                    chickens[i].CatchChicken();
                    break;
                }
            }
            for (int i = 0; i < om.Length; i++)
            {
                if (om[i].playerId == pId)
                {
                    om[i].playerRecord += 1;
                    score.ScoreUpdate();
                    mine = false;
                    break;
                }
            }
            if (mine)
            {
                audio.chicken_hit.Play();
            }
            else
            {
                oppAudio.chicken_hit.Play();
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
                    oppAudio.shoot.Play();
                    oppAudio.shoot_1.Play();
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
                    oppAudio.shoot.Play();
                    oppAudio.shoot_2.Play();
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

        socket.On("game_over", (SocketIOEvent e) => {
            Debug.Log("game_over");
            var data = JSON.ParseString(e.data.ToString());
            int wId = int.Parse(data["winnerId"].CreateString().Trim(trim));
            int playerId = FindObjectOfType<PlayerMove>().playerId;
            if(playerId == wId)
            {
                PlayerPrefs.SetString("result","win");
            }
            else if (wId == 0)
            {
                PlayerPrefs.SetString("result", "tie");
            }
            else
            {
                PlayerPrefs.SetString("result", "lose");
            }
            SceneManager.LoadScene("WinOrDie");
        });
    }
}