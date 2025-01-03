using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public static Entity entity;

    [Tooltip("If for the player, only manipulatable through GameMaster")]
    public int startingHealth;

    [Tooltip("If for the player, only manipulatable through GameMaster")]
    public int minAttackDamage;
    [Tooltip("If for the player, only manipulatable through GameMaster")]
    public int maxAttackDamage;

    public bool spriteIsFacingLeft = false;
    [HideInInspector] public string healthTextName = "";

    [HideInInspector] public bool firstScene = true;

    public AudioClip attackSound;
    public AudioClip defeatSound;
    
    [HideInInspector] public int currentHealth;
    [HideInInspector] public Text healthText;

    void Start()
    {
        //Pulls data from GameMaster if the entitiy is the player
        if(IsPlayer())
        {
        minAttackDamage = GameMaster.instance.minAttackDamage;
        maxAttackDamage = GameMaster.instance.maxAttackDamage;
        currentHealth = GameMaster.instance.currentHealth;
        healthText = GameMaster.instance.healthText;
        firstScene = GameMaster.instance.firstScene; 
            //Sets the starting health of the player if this is the first scene of the game
            if (firstScene == true)
            {
                startingHealth = GameMaster.instance.startingHealth;
                currentHealth = startingHealth;
                firstScene = false;
            }
        }
        else
        {
            currentHealth = startingHealth;
        }

        if (!string.IsNullOrEmpty(healthTextName))
        {
            healthText = GameObject.Find(healthTextName).GetComponent<Text>();
        }

        UpdateHealthText();
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
        
    public void SetAnimationTrigger(string trigger)
    {
        GetComponent<Animator>().SetTrigger(trigger);
    }

    public void PlayAttackSound()
    {
        Services.Sfx.Play(attackSound);
    }

    public void ApplyDamage(int damage)
    {
        currentHealth = currentHealth - damage;

        UpdateHealthText();


        if (currentHealth <= 0)
        {
            Die();
        }
    }
        
    public int RollDamage()
    {
        return Random.Range(minAttackDamage, maxAttackDamage + 1);
    }

    /// <summary>
    /// Make the entity face left or right depending on the direction of movement requested.
    /// </summary>
    /// <param name="direction">the requested movement direction.</param>
    public void UpdateFacing(Vector2 direction)
    {
        if (direction.x < 0)
        {
            if (spriteIsFacingLeft)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            
        }
        else if (direction.x > 0)
        {
            if (spriteIsFacingLeft)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    private void Die()
    {
        Job sfxJob = new Job() { jobType = JobType.PlaySfx, audioClip = defeatSound };
        Services.Simulation.AddJob(sfxJob);

        Job waitJob = new Job() { jobType = JobType.Wait, duration = 0.4f };
        Services.Simulation.AddJob(waitJob);

        if (IsPlayer())
        {
            gameObject.SetActive(false);
            Services.Hud.ShowDefeatUi();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// If our gameObject has a PlayerController, it is the player.
    /// </summary>
    /// <returns></returns>
    private bool IsPlayer()
    {
        return GetComponent<PlayerController>() != null;
    }
    /// <summary>
    /// This method schedules a bunch of events like playing animations and sounds related to an attack.
    /// </summary>
    /// <param name="instigator"></param>
    /// <param name="target"></param>
    private void AddAttackJob(Entity instigator, Entity target)
    {
        Job updateFacingJob = new Job() { jobType = JobType.UpdateFacing, instigator = instigator, target = target };
        Services.Simulation.AddJob(updateFacingJob);

        Job sfxJob = new Job() { jobType = JobType.PlaySfx, audioClip = instigator.attackSound };
        Services.Simulation.AddJob(sfxJob);

        Job attackJob = new Job() { jobType = JobType.Attack, instigator = instigator, target = target };
        Services.Simulation.AddJob(attackJob);

        if (target == entity)
        {
            Job waitJob = new Job() { jobType = JobType.Wait, duration = 0.2f };
            Services.Simulation.AddJob(waitJob);

            Job hurtJob = new Job() { jobType = JobType.TriggerAnimation, target = target, triggerName = "hurt" };
            Services.Simulation.AddJob(hurtJob);

            waitJob = new Job() { jobType = JobType.Wait, duration = 0.2f };
            Services.Simulation.AddJob(waitJob);
        }
        else
        {
            Job waitJob = new Job() { jobType = JobType.Wait, duration = 0.4f };
            Services.Simulation.AddJob(waitJob);
        }
    }
}


