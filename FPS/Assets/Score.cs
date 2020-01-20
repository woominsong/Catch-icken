using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public PlayerMove playerMove;
    public OpponentMove opponentMove;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScoreUpdate()
    {
        Text[] texts = FindObjectsOfType<Text>();
        foreach (Text t in texts)
        {
            if(t.name == "MyScore")
            {
                t.text = playerMove.playerRecord.ToString();
            }
            if(t.name == "OpponentScore")
            {
                t.text = opponentMove.playerRecord.ToString();
            }
        }
    }
}
