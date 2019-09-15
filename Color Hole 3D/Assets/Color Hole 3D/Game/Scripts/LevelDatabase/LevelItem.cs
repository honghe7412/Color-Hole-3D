using UnityEngine;

[System.Serializable]
public class LevelItem
{
    [SerializeField]
    private Level.Item partType;
    public Level.Item PartType
    {
        get { return partType; }
    }

    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab
    {
        get { return prefab; }
    }

    public LevelItem(Level.Item partType, GameObject prefab)
    {
        this.partType = partType;
        this.prefab = prefab;
    }
}
