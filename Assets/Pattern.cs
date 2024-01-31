using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Of Life/Patterns")]
public class Pattern : ScriptableObject
{
    public List<Vector2Int> cells;
}

