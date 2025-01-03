using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// These are the types of jobs the simulation can execute.
/// </summary>
public enum JobType
{
    ApplyDamage,
    Attack,
    ExecutePostActions,
    PlaySfx,
    TriggerAnimation,
    UpdateFacing,
    Wait,
}

/// <summary>
/// A job is an atomic action that can be performed by the simulation.
/// </summary>
public class Job
{
    public JobType jobType;
    public Entity instigator;
    public Entity target;
    public float duration;
    public string triggerName;
    public int damage;
    public AudioClip audioClip;
}

/// <summary>
/// The simulation class serves as a command queue for jobs that need to be performed in order. For example, when
/// the player attacks an enemy, several performances must happen in serial fashion: play an attack animation, apply damage,
/// play a hit react, check if enemy was defeated and if so play defeat animation, etc.
/// </summary>
public class Simulation : MonoBehaviour
{
    public AudioClip backgroundAudio;

    private Queue<Job> jobs = new Queue<Job>();
    private Job currentJob = null;
    private float waitTimer = 0.0f;
    
    void Awake()
    {
        Services.Simulation = this;
    }

    void Update ()
    {
        if (waitTimer > 0.0f)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0.0f)
            {
                // wait job that started this timer is finished.
                currentJob = null;
            }
        }
        	    
        if (currentJob == null && jobs.Count > 0)
        {
            PlayerController playerController = null;

            currentJob = jobs.Dequeue();

            switch(currentJob.jobType)
            {
                case JobType.ApplyDamage:
                    currentJob.target.ApplyDamage(currentJob.damage);
                    currentJob = null;
                    break;

                case JobType.Attack:
                    if (currentJob.instigator.IsAlive())
                    {
                        currentJob.instigator.SetAnimationTrigger("attack");

                        int damage = currentJob.instigator.RollDamage();
                                                
                        Job waitJob = new Job() { jobType = JobType.Wait, duration = 0.2f };
                        AddJob(waitJob);
                                                
                        Job damageJob = new Job() { jobType = JobType.ApplyDamage, target = currentJob.target, damage = damage };
                        AddJob(damageJob);

                        playerController = currentJob.instigator.GetComponent<PlayerController>();

                        if (playerController != null)
                        {
                            Job postActionJob = new Job() { jobType = JobType.ExecutePostActions, instigator = currentJob.instigator };
                            AddJob(postActionJob);
                        }
                    }

                    currentJob = null;
                    break;

                case JobType.ExecutePostActions:
                    playerController = currentJob.instigator.GetComponent<PlayerController>();

                    if (playerController != null)
                    {
                        playerController.PostMove();
                    }

                    currentJob = null;
                    break;

                case JobType.PlaySfx:
                    Services.Sfx.Play(currentJob.audioClip);
                    currentJob = null;
                    break;
                                    
                case JobType.TriggerAnimation:
                    currentJob.target.SetAnimationTrigger(currentJob.triggerName);

                    currentJob = null;
                    break;

                case JobType.UpdateFacing:
                    Vector3 directionToTarget = currentJob.target.transform.position - currentJob.instigator.transform.position;
                    currentJob.instigator.UpdateFacing(directionToTarget);
                    
                    currentJob = null;
                    break;
                case JobType.Wait:
                    waitTimer = currentJob.duration;
                    break;
            }
        }
	}

    public bool IsWorking()
    {
        return jobs.Count > 0 || currentJob != null;
    }

    public void AddJob(Job job)
    {
        jobs.Enqueue(job);
    }
}
