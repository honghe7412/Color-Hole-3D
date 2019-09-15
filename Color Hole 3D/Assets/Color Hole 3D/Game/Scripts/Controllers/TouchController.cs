using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public static TouchController instance;

    [Header("Settings")]
    public float touchSens = 1f;

    private bool isTouchActive;
    private float pixelToPlayground;
    private float heightToWidthCoef;

    private float leftBound;
    private float rightBound;
    private float topBound;
    private float bottomBound;

    private Vector3 prevPosition;
    private Vector3 targetPosition;
    private Transform holeTransform;

    private float stageOffsetZ;

    private void Awake()
    {
        instance = this;
        isTouchActive = false;

        pixelToPlayground = GroundGenerator.PLAYGROUND_WIDTH / Screen.width;
        heightToWidthCoef = (float)Screen.height / Screen.width;

        leftBound = GroundGenerator.HOLE_DIAMETER * 0.5f;
        rightBound = GroundGenerator.PLAYGROUND_WIDTH - leftBound;

        bottomBound = GroundGenerator.HOLE_DIAMETER * 0.5f;
        topBound = GroundGenerator.PLAYGROUND_HEIGHT - bottomBound;
    }

    private void Start()
    {
        holeTransform = HoleBehabiour.Transform;
    }

    public void Activate()
    {
        if (GameController.CurrentState == GameController.LevelState.First)
        {
            stageOffsetZ = 0f;
        }
        else
        {
            stageOffsetZ = GroundGenerator.PLAYGROUND_HEIGHT + GroundGenerator.GROUNDS_OFFSET;
        }

        isTouchActive = true;
    }

    public void Deactivate()
    {
        isTouchActive = false;
    }

    private void Update()
    {
        if (!isTouchActive)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            prevPosition = Input.mousePosition;
            targetPosition = holeTransform.position;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 touchDelta = Input.mousePosition - prevPosition; // screen touch delta
            Vector3 positionDelta = touchDelta * pixelToPlayground * touchSens; // world touch delta
            positionDelta = new Vector3(positionDelta.x, 0f, positionDelta.y * heightToWidthCoef); // normalized world delta

            targetPosition = targetPosition + positionDelta; // target position

            targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, leftBound, rightBound), 0f, Mathf.Clamp(targetPosition.z, bottomBound + stageOffsetZ, topBound + stageOffsetZ)); // clamped to playground bounds

            holeTransform.position = Vector3.Lerp(holeTransform.position, targetPosition, 0.3f); // lerped position

            GroundGenerator.UpdateGround();

            prevPosition = Input.mousePosition;
        }
    }
}