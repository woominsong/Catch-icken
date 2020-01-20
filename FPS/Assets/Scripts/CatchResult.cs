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

        if (other.tag == "Chicken")
        {

            if (me.playerId == playerId)
            {
                other.gameObject.GetComponent<ChickenController>().CatchChicken();
                me.playerRecord += 1;
                score.ScoreUpdate();

                //자기가 catch한 치킨에 대해서만 정보처리(
                //1. 치킨 없애는 함수 실행
                //2. 자신의 점수 올림
                //3. score update
            }
        }
    }
}
