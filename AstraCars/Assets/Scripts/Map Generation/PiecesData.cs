using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PiecesData")]
public class PiecesData : ScriptableObject
{

    public enum Direction
    {
        North,
        South,
        East,
        West
    }

    public GameObject[] levelPieces;
    public Vector2 piezeSize = new Vector2(1f, 1f);
    public Direction entry;
    public Direction exit;

}
