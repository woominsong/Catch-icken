using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchResult : MonoBehaviour
{
    public int playerId;
    private Score score;

    // Start is called before the first frame update
    void Start()
    {
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

        if (me.playerId == 2)
        {
            if (other.tag == "Chicken")
            {
                other.gameObject.SetActive(false);

                if (me.playerId == playerId)
                {
                    me.playerRecord += 1;
                }
                if (opponent.playerId == playerId)
                {
                    opponent.playerRecord += 1;
                }

                score.ScoreUpdate();
            }
        }
    }
}
