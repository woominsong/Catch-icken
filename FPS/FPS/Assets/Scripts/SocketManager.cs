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

    private char[] trim = { '"' };

    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        Debug.Log("socketmanager"+socket);

        socket.On("connected", (SocketIOEvent e) => {
            Debug.Log("connected");
        });

        socket.On("move", (SocketIOEvent e) => {
            Debug.Log("move");
            var data = JSON.ParseString(e.data.ToString());
            //Debug.Log("move1");
            var om = GetComponentsInChildren<OpponentMove>();
            //Debug.Log("move2");
            //Debug.Log(data["playerId"].CreateString().Trim(trim));
            int pId = int.Parse(data["playerId"].CreateString().Trim(trim));
            //Debug.Log("move3");
            //Debug.Log("Player "+pId+"moved by ("+data["x"].CreateString()+"," + data["y"].CreateString() + "," + data["z"].CreateString() + ")");
            //Debug.Log("move4");
            //Debug.Log(om.Length);
            for (int i=0; i<om.Length; i++)
            {
                //Debug.Log("move5");
                if (om[i].playerId == pId)
                {
                    om[i].oppMove(new Vector3(float.Parse(data["x"].CreateString().Trim(trim)), float.Parse(data["y"].CreateString().Trim(trim)), float.Parse(data["z"].CreateString().Trim(trim))));
                    break;
                }
            }            
        });
    }

}