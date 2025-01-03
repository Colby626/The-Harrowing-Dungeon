using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxImage : MonoBehaviour
{
    public float speedX = 0;
    public int spawnCount = 2;
    private const int roundFactor = 10000;

    private Transform[] controlledTransforms;
    private float imageWidth;
    private float minLeftX;
    private float maxRightX;
    private FloatReference speedMultiplier;
    private HorizontalDirection hDir;

    private bool followTransform;

    public void MoveX(float moveBy)
    {
        moveBy *= speedX * speedMultiplier.value;
        if (hDir == HorizontalDirection.Right) moveBy *= -1;

        moveBy = Mathf.Round(moveBy * roundFactor) / roundFactor;
        for (int i = 0; i < controlledTransforms.Length; i++)
        {
            Vector3 newPos = controlledTransforms[i].position;
            newPos.x -= moveBy;
            newPos.x = Mathf.Round(newPos.x * roundFactor) / roundFactor;
            controlledTransforms[i].position = newPos;
        }
        CheckAndReposition();
    }

    private void CheckAndReposition()
    {
        if (hDir == HorizontalDirection.Left || followTransform)
        {
            for (int i = 0; i < controlledTransforms.Length; i++)
            {
                if (controlledTransforms[i].position.x < minLeftX)
                {
                    Vector3 newPos = controlledTransforms[i].position;
                    newPos.x = GetRightmostTransform().position.x + imageWidth;
                    controlledTransforms[i].position = newPos;
                }
            }
        }
        else if (hDir == HorizontalDirection.Right || followTransform)
        {
            for (int i = 0; i < controlledTransforms.Length; i++)
            {
                if (controlledTransforms[i].position.x > maxRightX)
                {
                    Vector3 newPos = controlledTransforms[i].position;
                    newPos.x = GetLeftmostTransform().position.x - imageWidth;
                    controlledTransforms[i].position = newPos;
                }
            }
        }
    }

    public void CleanUpImage()
    {
        if (controlledTransforms != null)
        {
            for (int i = 1; i < controlledTransforms.Length; i++)
            {
                Destroy(controlledTransforms[i].gameObject);
            }
        }
    }

    public void InitImage(FloatReference speedMultiplier, HorizontalDirection hDir, bool followTransform)
    {
        this.speedMultiplier = speedMultiplier;
        this.hDir = hDir;
        this.followTransform = followTransform;

        int arraySize = spawnCount;
        if (followTransform) 
            arraySize *= 2;
        arraySize += 1;
        
        controlledTransforms = new Transform[spawnCount + 1];
        controlledTransforms[0] = transform;
        

        imageWidth = GetComponent<SpriteRenderer>().bounds.size.x;

        if (followTransform)
        {
            minLeftX = transform.position.x - imageWidth - 0.5f;
            maxRightX = transform.position.x + imageWidth + 0.5f;
        }
        else
        {
            if (hDir == HorizontalDirection.Left)
            {
                minLeftX = transform.position.x - imageWidth - 0.5f;
                maxRightX = float.PositiveInfinity;
            }
            else if (hDir == HorizontalDirection.Right)
            {
                maxRightX = transform.position.x + imageWidth + 0.5f;
                minLeftX = float.NegativeInfinity;
            }
            else if (hDir == HorizontalDirection.Fix)
            {
                minLeftX = float.NegativeInfinity;
                maxRightX = float.PositiveInfinity;
            }
        }


        float posX;
        for (int i = 1; i < controlledTransforms.Length; i++) //posible problem
        {
            posX = transform.position.x + imageWidth * i;
            if (hDir == HorizontalDirection.Right)
            {
                posX = transform.position.x - imageWidth * i;
            }
            else
            {
                posX = transform.position.x + imageWidth * i;
            }
            controlledTransforms[i] = PrepareCopyAt(posX);
            /*if (followTransform)
                controlledTransforms[i] = PrepareCopyAt(posX);*/
        }
    }

    private Transform PrepareCopyAt(float posX)
    {
        GameObject go = Instantiate(gameObject, new Vector3(posX, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
        Destroy(go.GetComponent<ParallaxImage>());

        return go.transform;
    }

    private Transform GetRightmostTransform()
    {
        float currentMaxX = float.NegativeInfinity;
        Transform currentTransform = null;

        for (int i = 0; i < controlledTransforms.Length; i++)
        {
            if (currentMaxX < controlledTransforms[i].position.x)
            {
                currentMaxX = controlledTransforms[i].position.x;
                currentTransform = controlledTransforms[i];
            }
        }

        return currentTransform;
    }

    private Transform GetLeftmostTransform()
    {
        float currentMinX = float.PositiveInfinity;
        Transform currentTransform = null;

        for (int i = 0; i < controlledTransforms.Length; i++)
        {
            if (currentMinX > controlledTransforms[i].position.x)
            {
                currentMinX = controlledTransforms[i].position.x;
                currentTransform = controlledTransforms[i];
            }
        }

        return currentTransform;
    }
}
