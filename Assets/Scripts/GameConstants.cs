using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants : MonoBehaviour
{
    public int numOfBlocks = 4;

    public int nextTetrominosToDisplay = 6;
    public int nextTetrominoDisplayXPos = 12;
    public int[] nextTetrominoDisplayYPos = { 19, 16, 13, 10, 7, 4 };
    public Vector3 heldTetrominoPos = new Vector3Int(-4, 19, 0);

    public float lockDelay = 0.5f;

    // How long to press left/right key until piece continually moves in miliseconds
    public float autoRepeatDelay = 170;
    // Number of miliseconds between each movement while piece continually moves
    public float autoRepeatSpeed = 50;

    public bool ghostPieceIsEnabled = true;

    // Border coordinates
    public int leftBorder = 0;
    public int rightBorder = 9;
    public int lowerBorder = 0;

    public int tetrisGridWidth = 10;
    public int tetrisGridHeight = 24;

    public Vector3Int spawnPosition = new Vector3Int(4, 21, 0);

    // For 0, 1, 2, 3, or 4 lines removed.
    public int[] scoreFromRemovedLines = { 0, 100, 300, 500, 800 };

    // from tetris guidelines
    public float[] gravityAtLevel = {
        // Level 1 to 5
        0.01667f, 0.021017f, 0.026977f, 0.035256f, 0.04693f,
        // level 6 to 10
        0.06361f, 0.0879f, 0.1236f, 0.1775f, 0.2598f,
        // Level 11 to 15
        0.388f, 0.59f, 0.92f, 1.46f, 2.36f,
        // Level 16 to 20
        3.84f, 6.34f, 10f, 16f, 20f
        // Level 21+ is just 20G
    };
}
