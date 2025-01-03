using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Is a singleton and saves variables between scene loads

public class GameMaster : MonoBehaviour
{

    public static GameMaster instance;

    //From PlayerController
    [Tooltip("Does the player start with a potion effect")]
    public bool hasPotionEffect;
    [Tooltip("How far along is the potion effect")]
    public int potionTime;
    [HideInInspector] public int strengthMultiplier = 2;
    [HideInInspector] public bool lowerCooldownImage;
    [Tooltip("What is the player's current level")]
    public int currentLevel = 1;

    //From Entity
    [Tooltip("How much health does the player start with")]
    public int startingHealth = 100;
    [HideInInspector] public bool firstScene = true;
    [Tooltip("What is the player's minimum attack damage to start")]
    public int minAttackDamage;
    [Tooltip("What is the player's maximum attack damage to start")]
    public int maxAttackDamage;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public Text healthText;

    //From Hud
    public float tempFill;
    public bool hasBeenPaused;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
