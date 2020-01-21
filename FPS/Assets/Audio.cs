using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    //Audio
    [SerializeField]
    GameObject a_chicken_hit;
    [SerializeField]
    GameObject a_container_hit;
    [SerializeField]
    GameObject a_game;
    [SerializeField]
    GameObject a_player_die;
    [SerializeField]
    GameObject a_player_hit;
    [SerializeField]
    GameObject a_shoot_1;
    [SerializeField]
    GameObject a_shoot_2;
    [SerializeField]
    GameObject a_shoot;

    //Audio
    [HideInInspector]
    public AudioSource chicken_hit;
    [HideInInspector]
    public AudioSource container_hit;
    [HideInInspector]
    public AudioSource game;
    [HideInInspector]
    public AudioSource player_die;
    [HideInInspector]
    public AudioSource player_hit;
    [HideInInspector]
    public AudioSource shoot_1;
    [HideInInspector]
    public AudioSource shoot_2;
    [HideInInspector]
    public AudioSource shoot;

    private void Start()
    {
        chicken_hit = Instantiate(chicken_hit, transform);
        container_hit = Instantiate(container_hit, transform);
        game = Instantiate(game, transform);
        player_die = Instantiate(player_die, transform);
        player_hit = Instantiate(player_hit, transform);
        shoot_1 = Instantiate(shoot_1, transform);
        shoot_2 = Instantiate(shoot_2, transform);
        shoot = Instantiate(shoot, transform);
    }
}
