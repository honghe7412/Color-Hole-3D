using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public static readonly Vector3 STAGE_ONE_CAM_POS_EDITOR = new Vector3(2.75f, 10.3f, -0.7f);
    public static readonly Vector3 STAGE_TWO_CAM_POS_EDITOR = new Vector3(2.75f, 10.3f, 14.8f);

    private Vector3 stageOnePosition;
    private Vector3 stageTwoPosition;

    [Header("Settings")]
    public float movingSpeed = 6f;

    [Header("References")]
    public CameraShake cameraShake;
    public CameraAspectSetuper aspectSetuper;

    private Transform transformRef;

    private void Awake()
    {
        instance = this;

        transformRef = transform;

        CameraAspectSetuper.AspectRatioSettings settings = aspectSetuper.GetSettings();

        stageOnePosition = settings.cameraOffset;
        stageTwoPosition = stageOnePosition + new Vector3(0f, 0f, GroundGenerator.PLAYGROUND_HEIGHT + GroundGenerator.GROUNDS_OFFSET);
    }

    public void InitCameraOnFirstStage()
    {
        transformRef.position = stageOnePosition;
    }

    public void MoveToSecondStage()
    {
        StartCoroutine(MoveCameraToNextStage());
    }

    private IEnumerator MoveCameraToNextStage()
    {
        WaitForFixedUpdate delay = new WaitForFixedUpdate();

        while (Mathf.Abs(transformRef.position.z - stageTwoPosition.z) > 0.01f)
        {
            Vector3 newPos = Vector3.MoveTowards(transformRef.position, transformRef.position.SetZ(stageTwoPosition.z), movingSpeed * Time.fixedDeltaTime);
            transformRef.position = Vector3.Lerp(transformRef.position, newPos, 0.7f);

            yield return delay;
        }

        transformRef.position = stageTwoPosition;
    }

    public void ShakeCamera()
    {
        cameraShake.ShakeOnce();
    }
}