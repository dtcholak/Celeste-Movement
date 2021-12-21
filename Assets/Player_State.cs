using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State : MonoBehaviour
{

    public int healthInitial = 3;
    public int healthCurrent;

    // Start is called before the first frame update
    void Start()
    {
        ResetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetHealth()
    {
        healthCurrent = healthInitial;
    }

    public void TakeDamage(int damageAmount) 
    {
        healthCurrent -= damageAmount;

        if(healthCurrent <= 0)
        {
            Destroy(gameObject);
        }

    }

    public void Heal(int healAmount)
    {
        healthCurrent += healAmount;
        if(healthCurrent > healthInitial)
        {
            ResetHealth();
        }
    }
}

