// PREPROC

#define DEBUG
//#define DMGTEST
//#define DEATHTEST

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------
// Cameron Hadfield
// TOJam 2022
// Health.cs
// This class tracks the health of anything, providing a listenable death trigger when that health reaches zero
// Also exposes damage functions
// ---------------------
public class Health : MonoBehaviour, IDamageable
{
    // -----EVENTS-----------------------------
    public delegate void OnHealthChange(float oldHealth, float newHealth);
    public event OnHealthChange HealthChanged;

    public delegate void OnDeath();
    public event OnDeath Death;

    // ------HEALTH RELATED--------------------
    [SerializeField]
    private float _health;

    public float health
    {
        get { return _health; }
        set
        {
            float oldHealth = _health;
            _health = value;

            HealthChanged?.Invoke(oldHealth, _health);
            if (_health <= 0)
            {
                Death?.Invoke();
            }
        }
    }

    // -------- DEATH :( ------------------------
    [SerializeField]
    public float maxHealth = 10; // Provide default for rapid dev

    public virtual void BeDamaged(float dmg)
    {
        health -= dmg;
    }

    void Awake()
    {
        health = maxHealth;
    }

    void Start()
    {
        health = maxHealth;
#if DMGTEST
        HealthChanged += (float oldHealth, float newHealth) =>
        {
            Debug.Log(oldHealth + " -> " + newHealth);
        };
#endif
#if DEATHTEST
        Death += () =>
        {
            Debug.Log("Character Death");
        };
#endif
    }

    public void Heal(float healAmount)
    {
        health = Mathf.Min(health + healAmount, maxHealth);
    }

    public void FullyHeal()
    {
        health = maxHealth;
    }
}
