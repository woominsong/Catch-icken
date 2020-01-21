﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContanerId : MonoBehaviour
{
    // Start is called before the first frame update

    public int containerId;
    public int containerHP = 100;

    ChickenSpawner chickenSpawner;
    PlayerMove playerMove;
    Score score;
    
    void Start()
    {
        chickenSpawner = FindObjectOfType<ChickenSpawner>();
        playerMove = FindObjectOfType<PlayerMove>();
        score = FindObjectOfType<Score>();
    }

    // Update is called once per frame
    void Update() 
    {
        if(containerHP == 0 && playerMove.playerId == containerId) // 자기 컨테이너만 파괴될때 처리
        {
            chickenSpawner.RespawnChickens();
            playerMove.playerRecord = 0;
            score.ScoreUpdate();
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
        containerHP = 100;
        GetComponentInChildren<Slider>().value = 1;
    }
}
