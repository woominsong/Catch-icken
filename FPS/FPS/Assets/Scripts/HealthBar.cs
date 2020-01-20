using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    Image energybar;
    Text energy;

    private float health = 100;
    private float maxHealth = 100;

    // Start is called before the first frame update
    void Start()
    {
        energybar = GetComponentInChildren<Image>();
        energy = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateEnergybar(float input)
    {
        health = input;
        float ratio = health / maxHealth;
        energybar.rectTransform.localScale = new Vector3(ratio, 1, 1);
        energy.text = (ratio * 100).ToString();
    }
}
