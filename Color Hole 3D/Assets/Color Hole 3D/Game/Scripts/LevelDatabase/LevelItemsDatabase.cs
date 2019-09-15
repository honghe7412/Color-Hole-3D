#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;
using Watermelon;

[CreateAssetMenu(fileName = "Level Items Database", menuName = "Content/Level Items Database", order = 1)]
public class LevelItemsDatabase : ScriptableObject, IInitialized
{
    [SerializeField]
    private LevelItem[] items;
    public LevelItem[] Items
    {
        get { return items; }
    }

    private Dictionary<Level.Item, int> itemsLink = new Dictionary<Level.Item, int>();

    public void Init()
    {
        if (items.IsNullOrEmpty())
            return;

        itemsLink = new Dictionary<Level.Item, int>();
        for(int i = 0; i < items.Length; i++)
        {
            itemsLink.Add(items[i].PartType, i);
        }
    }

    public LevelItem GetItem(Level.Item itemType)
    {
        if(!items.IsNullOrEmpty() && (itemsLink == null || itemsLink.Count == 0))
        {
            Init();
        }

        if (itemsLink.ContainsKey(itemType))
        {
            return items[itemsLink[itemType]];
        }

#if UNITY_EDITOR

        else
        {
            Init();

            if (itemsLink.ContainsKey(itemType))
            {
                return items[itemsLink[itemType]];
            }
        }

#endif

        return null;
    }
}