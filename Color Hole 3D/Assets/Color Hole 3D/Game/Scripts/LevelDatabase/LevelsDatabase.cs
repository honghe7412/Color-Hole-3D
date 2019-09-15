#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

[CreateAssetMenu(fileName = "Levels Database", menuName = "Content/Levels Database", order = 1)]
public class LevelsDatabase : ScriptableObject
{
    [SerializeField]
    private Level[] levels;
    public Level[] Levels
    {
        get { return levels; }
    }

    public int LevelsAmount
    {
        get { return levels.Length; }
    }

    public Level GetLevel(int index)
    {
        if(levels.IsInRange(index))
        {
            return levels[index];
        }

        return null;
    }
}