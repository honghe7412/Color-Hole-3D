using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;

    [Header("References")]
    public LevelItemsDatabase itemsDatabase;
    public LevelsDatabase levelsDatabase;
    public Transform cameraTransform;
    public Transform gatesTransform;
    public Transform pathSpheresPrefabTransform;

    private List<Pool> itemsPools = new List<Pool>();
    private List<LevelItemBehaviour> firstStageObstaclesList = new List<LevelItemBehaviour>();


    private Pool cubes;

    public int LevelsAmount
    {
        get { return levelsDatabase.LevelsAmount; }
    }

    private void Awake()
    {
        instance = this;

        cubes = PoolManager.GetPoolByName("CubeItem");

        List<LevelItem> items = new List<LevelItem>();
        items.AddRange(itemsDatabase.Items);
        items.Sort((LevelItem l1, LevelItem l2) => { return l1.PartType.CompareTo(l2.PartType); });

        for (int i = 0; i < itemsDatabase.Items.Length; i++)
        {
            itemsPools.Add(PoolManager.GetPoolByName(items[i].Prefab.name));
        }
    }

    public void LoadLevel(int levelNumber)
    {
        for (int i = 0; i < itemsPools.Count; i++)
        {
            itemsPools[i].ReturnToPoolEverything();
        }

        int firstStageItemsAmount = 0;
        int secondStageItemsAmount = 0;
        float firstStageHighestZ = GroundGenerator.PLAYGROUND_HEIGHT;
        float secondStageLowestZ = GroundGenerator.PLAYGROUND_HEIGHT + GroundGenerator.GROUNDS_OFFSET;
        firstStageObstaclesList.Clear();
        CloseGates();

        Level level = levelsDatabase.GetLevel(levelNumber - 1);

        for (int i = 0; i < level.Items.Length; i++)
        {
            Level.ItemSave itemSave = level.Items[i];
            LevelItemBehaviour item = itemsPools[(int)itemSave.Item].GetPooledObject(itemSave.Position).GetComponent<LevelItemBehaviour>();

            item.Transform.eulerAngles = itemSave.Rotation;
            item.Transform.localScale = itemSave.Scale;

            item.Init(itemSave.Type);

            if (item.Type == LevelItemBehaviour.ItemType.Normal)
            {
                if (itemSave.Position.z < firstStageHighestZ)
                {
                    firstStageItemsAmount++;
                }
                else if (itemSave.Position.z > secondStageLowestZ)
                {
                    secondStageItemsAmount++;
                }
            }
            else
            {
                if (itemSave.Position.z < firstStageHighestZ)
                {
                    firstStageObstaclesList.Add(item);
                }
            }
        }

        GameController.FirstStageItemsAmount = firstStageItemsAmount;
        GameController.SecondStageItemsAmount = secondStageItemsAmount;

        int sphereId = (int)Level.Item.Sphere;
        Vector3 positionOffset = pathSpheresPrefabTransform.position;
        Vector3 itemsScale = pathSpheresPrefabTransform.GetChild(0).localScale;

        for (int i = 0; i < pathSpheresPrefabTransform.childCount; i++)
        {
            LevelItemBehaviour item = itemsPools[sphereId].GetPooledObject(pathSpheresPrefabTransform.GetChild(i).localPosition + positionOffset).GetComponent<LevelItemBehaviour>();

            item.Transform.localScale = itemsScale;
            item.Transform.rotation = new Quaternion();

            item.Init(LevelItemBehaviour.ItemType.Normal);
        }
    }

    public void OnFirstStageCompleted()
    {
        StartCoroutine(MoveHoleToNextStage(HoleBehabiour.Transform));
    }

    private IEnumerator MoveHoleToNextStage(Transform holeTransform)
    {
        TouchController.instance.Deactivate();
        DeactivateFirstStageObstacles();
        OpenGates();

        float playgroundCenter = GroundGenerator.PlaygroundCenter;
        WaitForFixedUpdate delay = new WaitForFixedUpdate();

        while (Mathf.Abs(holeTransform.position.x - playgroundCenter) > 0.01f)
        {
            holeTransform.position = Vector3.MoveTowards(holeTransform.position, holeTransform.position.SetX(playgroundCenter), 2f * Time.fixedDeltaTime);
            GroundGenerator.UpdateGround();

            yield return delay;
        }

        HoleBehabiour.EnableAroundHoleMesh();
        holeTransform.position = holeTransform.position.SetX(playgroundCenter);

        float nextStageStartZ = GroundGenerator.PLAYGROUND_HEIGHT + GroundGenerator.GROUNDS_OFFSET + GroundGenerator.HOLE_INITIAL_OFFSET_Z;

        CameraController.instance.MoveToSecondStage();
        GroundGenerator.stopVerticalMeshGen = true;

        while (Mathf.Abs(holeTransform.position.z - nextStageStartZ) > 0.01f)
        {
            holeTransform.position = Vector3.MoveTowards(holeTransform.position, holeTransform.position.SetZ(nextStageStartZ), 4f * Time.fixedDeltaTime);
            GroundGenerator.UpdateGround();

            GroundGenerator.UpdateConnectingPath();

            yield return delay;
        }

        ActivateFirstStageObstacles();
        HoleBehabiour.DisableAroundHoleMesh();
        GameController.OnSecondStageReached();

        GroundGenerator.stopVerticalMeshGen = false;
        GroundGenerator.RecalculateNormals();
        GroundGenerator.UpdateGround();
    }

    private void DeactivateFirstStageObstacles()
    {
        for (int i = 0; i < firstStageObstaclesList.Count; i++)
        {
            firstStageObstaclesList[i].Deactivate();
        }
    }

    private void ActivateFirstStageObstacles()
    {
        for (int i = 0; i < firstStageObstaclesList.Count; i++)
        {
            firstStageObstaclesList[i].Activate();
        }
    }

    private void CloseGates()
    {
        gatesTransform.localScale = Vector3.one;
    }

    private void OpenGates()
    {
        gatesTransform.DOScale(new Vector3(1f, 0f, 1f), 0.4f).SetEasing(Ease.Type.CubicOut).OnComplete(() => gatesTransform.localScale = Vector3.zero);
    }
}