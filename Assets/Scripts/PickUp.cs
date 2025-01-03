using UnityEngine;

public class PickUp : MonoBehaviour
{
    public int healthBoost = 10;
    public AudioClip pickUpSound;

    public void Apply(Entity entity)
    {
        entity.ApplyDamage(-healthBoost);

        Services.Sfx.Play(pickUpSound);
    }
}
