using UnityEngine;

public class Potion : MonoBehaviour
{
    public AudioClip potionSound;
    public GameObject player;
    [Tooltip("How long does the potion last")]
    public int potionDuration;
    [Tooltip("How much of a strength multiplier does the potion give the player")]
    public int strengthMultiplier;

    void Start()
    {
        player.GetComponent<PlayerController>().potionDuration = potionDuration;
        player.GetComponent<PlayerController>().strengthMultiplier = strengthMultiplier;
    }

    public void Apply(Entity entity)
    {
        Services.Sfx.Play(potionSound);
    }
}