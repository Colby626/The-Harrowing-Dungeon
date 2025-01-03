using UnityEngine;

/// <summary>
/// This behavior moves the player based on input and checks for adjacent enemies, pickups, and exits.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Tooltip("The player loses this amount of health per step taken.")]
    public int healthLossPerStep = 1;

    private Entity entity;
    [HideInInspector] public bool paused = false;
    [HideInInspector] public bool hasPotionEffect = false;
    [HideInInspector] public int potionTime = 0;
    [HideInInspector] public float potionDuration = 3;
    [HideInInspector] public int strengthMultiplier = 2;
    [HideInInspector] public bool lowerCooldownImage = false;
    [HideInInspector] public int currentLevel = 1;
    public GameObject hud;

    void Awake()
    {
        paused = false; // I'm not sure why this has to be here, but if it isn't the game starts paused
        Services.PlayerController = this;
    }

    void Start()
    {
        // store a reference to the player's entity component
        // because we will use it alot.
        entity = GetComponent<Entity>();

        //Pull neccessary data from GameMaster
        hasPotionEffect = GameMaster.instance.hasPotionEffect;
        potionTime = GameMaster.instance.potionTime;
        strengthMultiplier = GameMaster.instance.strengthMultiplier;
        lowerCooldownImage = GameMaster.instance.lowerCooldownImage;
        currentLevel = GameMaster.instance.currentLevel;

        //Only for the first scene can set higher player levels
        if (GameMaster.instance.firstScene == true)
        {
            for (; currentLevel > 1; currentLevel--)
            {
                entity.minAttackDamage++;
                entity.maxAttackDamage++;
            }
        }

        currentLevel = GameMaster.instance.currentLevel;
    }

	void Update ()
    {
        // if game is paused, ignore input
        if (paused)
        {
            return;
        }

        // just ignore input if the simulation is currently processing a job,
        // for example an enemy is playing an attack animation.
        if (Services.Simulation == null || Services.Simulation.IsWorking())
        {
            return;
        }

        // ignore input if the player has been defeated.
        if (!entity.IsAlive())
        {
            return;
        }

        // handle user input...
        if (Input.GetKeyDown(KeyCode.Escape) && paused == false)
        {
            Services.PauseScreen.SetActive(true);
            paused = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            TryWalk(new Vector2(-1, 0));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            TryWalk(new Vector2(1, 0));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            TryWalk(new Vector2(0, 1));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            TryWalk(new Vector2(0, -1));
        }
	}

    public void UpArrowButton()
    {
        // if game is paused, ignore input
        if (paused)
        {
            return;
        }

        // just ignore input if the simulation is currently processing a job,
        // for example an enemy is playing an attack animation.
        if (Services.Simulation == null || Services.Simulation.IsWorking())
        {
            return;
        }

        // ignore input if the player has been defeated.
        if (!entity.IsAlive())
        {
            return;
        }
        TryWalk(new Vector2(0, 1));
    }

    public void RightArrowButton()
    {
        // if game is paused, ignore input
        if (paused)
        {
            return;
        }

        // just ignore input if the simulation is currently processing a job,
        // for example an enemy is playing an attack animation.
        if (Services.Simulation == null || Services.Simulation.IsWorking())
        {
            return;
        }

        // ignore input if the player has been defeated.
        if (!entity.IsAlive())
        {
            return;
        }
        TryWalk(new Vector2(1, 0));
    }

    public void LeftArrowButton()
    {
        // if game is paused, ignore input
        if (paused)
        {
            return;
        }

        // just ignore input if the simulation is currently processing a job,
        // for example an enemy is playing an attack animation.
        if (Services.Simulation == null || Services.Simulation.IsWorking())
        {
            return;
        }

        // ignore input if the player has been defeated.
        if (!entity.IsAlive())
        {
            return;
        }
        TryWalk(new Vector2(-1, 0));
    }

    public void DownArrowButton()
    {
        // if game is paused, ignore input
        if (paused)
        {
            return;
        }

        // just ignore input if the simulation is currently processing a job,
        // for example an enemy is playing an attack animation.
        if (Services.Simulation == null || Services.Simulation.IsWorking())
        {
            return;
        }

        // ignore input if the player has been defeated.
        if (!entity.IsAlive())
        {
            return;
        }
        TryWalk(new Vector2(0, -1));
    }

    public void PauseButton()
    {
        paused = true;
        Services.PauseScreen.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "PickUp")
        {
            PickUp pickUp = collider.gameObject.GetComponent<PickUp>();
            pickUp.Apply(entity);
            Destroy(collider.gameObject);
        }
        else if (collider.gameObject.tag == "Potion")
        {
            potionDuration = collider.gameObject.GetComponent<Potion>().potionDuration;
            Potion potion = collider.gameObject.GetComponent<Potion>();
            potion.Apply(entity);
            Destroy(collider.gameObject);
            if(hasPotionEffect == true)
            {
                entity.minAttackDamage /= strengthMultiplier;
                entity.maxAttackDamage /= strengthMultiplier;
            }
            hasPotionEffect = true;
            potionTime = 0;
            entity.minAttackDamage *= strengthMultiplier;
            entity.maxAttackDamage *= strengthMultiplier;
        }
        else if (collider.gameObject.tag == "Exit")
        {
            currentLevel++;
            //Checks to see if the player has a potion effect to add strength properly
            if (hasPotionEffect == true)
            {
                entity.minAttackDamage /= strengthMultiplier;
                entity.maxAttackDamage /= strengthMultiplier;
                entity.minAttackDamage++;
                entity.maxAttackDamage++;
                entity.minAttackDamage *= strengthMultiplier;
                entity.maxAttackDamage *= strengthMultiplier;
            }
            else
            {
                entity.minAttackDamage++;
                entity.maxAttackDamage++;
            }
            Save();
            MapExit exit = collider.gameObject.GetComponent<MapExit>();
            exit.OnPlayerTrigger();
        }
    }

    public void Unpause()
    {
        paused = false;
    }

    public void PostMove()
    {
        FindAdjacentEnemy(new Vector2(-1, 0));
        FindAdjacentEnemy(new Vector2(1, 0));
        FindAdjacentEnemy(new Vector2(0, -1));
        FindAdjacentEnemy(new Vector2(0, 1));
    }

    private void TryWalk(Vector2 direction)
    {
        entity.UpdateFacing(direction);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1);

        if (hit.collider != null)
        {
            switch (hit.collider.gameObject.tag)
            {
                case "Wall":
                    // return now, do not move the player as happens below
                    return;
                case "Enemy":
                    Entity targetEntity = hit.collider.gameObject.GetComponent<Entity>();
                    AddAttackJob(GetComponent<Entity>(), targetEntity);
                    potionTime++;
                    lowerCooldownImage = true;
                    return;
                default:
                    // do nothing
                    break;
            }
        }

        potionTime++;
        lowerCooldownImage = true;
        if (hasPotionEffect == true && potionTime >= potionDuration)
        {
            hasPotionEffect = false;
            entity.minAttackDamage /= strengthMultiplier;
            entity.maxAttackDamage /= strengthMultiplier;
            potionTime = 0;
        }
        
        entity.ApplyDamage(healthLossPerStep);
        
        transform.Translate(direction);

        PostMove();
    }
       
    private void FindAdjacentEnemy(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1);

        if (hit.collider != null && hit.collider.gameObject.tag == "Enemy")
        {
            Entity attackerEntity = hit.collider.gameObject.GetComponent<Entity>();
            AddAttackJob(attackerEntity, GetComponent<Entity>());
        }
    }

    // Saves all data to the GameMaster to persist into the next scene
    public void Save()
    {
        GameMaster.instance.hasPotionEffect = hasPotionEffect;
        GameMaster.instance.potionTime = potionTime;
        GameMaster.instance.strengthMultiplier = strengthMultiplier;
        GameMaster.instance.lowerCooldownImage = lowerCooldownImage;
        GameMaster.instance.currentLevel = currentLevel;

        GameMaster.instance.startingHealth = entity.startingHealth;
        GameMaster.instance.firstScene = entity.firstScene;
        GameMaster.instance.minAttackDamage = entity.minAttackDamage;
        GameMaster.instance.maxAttackDamage = entity.maxAttackDamage;
        GameMaster.instance.currentHealth = entity.currentHealth;

        hud.GetComponent<Hud>().tempFill = hud.GetComponent<Hud>().cooldown.fillAmount;
        GameMaster.instance.tempFill = hud.GetComponent<Hud>().tempFill;
        GameMaster.instance.hasBeenPaused = hud.GetComponent<Hud>().hasBeenPaused;
    }

    //Resets all data to starting values in the GameMaster
    public void Restart()
    {
        GameMaster.instance.hasPotionEffect = false;
        GameMaster.instance.potionTime = 0;
        GameMaster.instance.strengthMultiplier = 2;
        GameMaster.instance.lowerCooldownImage = false;

        for (; currentLevel > 1; currentLevel--)
        {
            GameMaster.instance.minAttackDamage--;
            GameMaster.instance.maxAttackDamage--;
        }

        GameMaster.instance.currentLevel = currentLevel;

        GameMaster.instance.firstScene = true;
        if (hasPotionEffect == true)
        {
            hasPotionEffect = false;
            entity.minAttackDamage /= strengthMultiplier;
            entity.maxAttackDamage /= strengthMultiplier;
        }
        GameMaster.instance.currentHealth = GameMaster.instance.startingHealth;

        hud.GetComponent<Hud>().tempFill = 0;
        GameMaster.instance.tempFill = 0;
        GameMaster.instance.hasBeenPaused = false;
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
