using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public HorizontalDirection horizontalDirection;

    public FloatReference speedMultiplier;

    public MoveType moveType;

    [Header("Only for follow transform")]
    public Transform transformToFollow;

    private List<ParallaxImage> images;
    private float lastX;

    private void Start ()
    {
        InitController();
    }


    private void FixedUpdate ()
    {
        if (images == null) return;

        if (moveType == MoveType.OverTime)
            MoveOverTime();
        else if (moveType == MoveType.FollowTransform)
            FollowTransform();
    }

    private void MoveOverTime()
    {
        if (horizontalDirection == HorizontalDirection.Fix)
            return;
        
        foreach (var item in images)
        {
            item.MoveX(Time.deltaTime);
        }
    }

    private void FollowTransform ()
    {
        if (horizontalDirection == HorizontalDirection.Fix)
            return;

        float distance = lastX - transformToFollow.position.x;
        if (Mathf.Abs(distance) < 0.001f) return;

        foreach (var item in images)
        {
            item.MoveX(distance);
        }
        lastX = transformToFollow.position.x;
    }

    private void InitController()
    {
        InitList();

        ScanForImages();

        foreach (var item in images)
        {
            item.InitImage(speedMultiplier, horizontalDirection, moveType == MoveType.FollowTransform);
        }
    }

    private void InitList()
    {
        if (images == null)
            images = new List<ParallaxImage>();
        else
        {
            foreach (var item in images)
            {
                item.CleanUpImage();
            }
            images.Clear();
        }

        if (moveType == MoveType.FollowTransform)
        {
            lastX = transformToFollow.position.x;
        }
    }
    private void ScanForImages()
    {
        ParallaxImage pi;

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                pi = child.GetComponent<ParallaxImage>();
                if (pi != null)
                    images.Add(pi);
            }
        }
    }
}

[System.Serializable]
public class FloatReference
{
    [Range(0.01f, 5)]
    public float value = 1;
}

public enum HorizontalDirection
{
    Fix,
    Left,
    Right
}

public enum MoveType
{
    OverTime,
    FollowTransform
}