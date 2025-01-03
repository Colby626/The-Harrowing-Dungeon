using UnityEngine;

/// <summary>
/// Randomly selects one sprite from a set of sprites to be
/// used as the actual source image by the sprite renderer.
/// This behavior can be used to add variety to repetitive
/// content like walls, etc.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
    public Sprite[] sprites;

	void Start ()
    {
        int randomIndex = Random.Range(0, sprites.Length);

        GetComponent<SpriteRenderer>().sprite = sprites[randomIndex];
	}
}
