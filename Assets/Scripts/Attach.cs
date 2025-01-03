using UnityEngine;

/// <summary>
/// Forces the game object with this component to stick to the 
/// exact position of the target game object. For example,
/// this can be used to lock the camera to the player character.
/// </summary>
public class Attach : MonoBehaviour
{
    public GameObject target;

    void LateUpdate()
    {
        Vector3 position = target.transform.position;
        position.z = transform.position.z;

        transform.position = position;
    }
}
