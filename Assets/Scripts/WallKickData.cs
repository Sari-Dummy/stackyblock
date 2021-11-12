using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallKickData : MonoBehaviour
{

    public int[,,][] wallKickData = new int[4, 4, 5][];
    public int[,,][] iTetrominoWallKickData = new int[4, 4, 5][];

    void Start()
    {
        PopulateWallKickData();
    }

    // wallKickData[original state, new state, wall kick test] = new int[] {xTranslation, yTranslation};
    // taken from the Tetris Standard Rotation System
    void PopulateWallKickData()
    {
        wallKickData[0, 1, 0] = new int[] { 0, 0 };
        wallKickData[0, 1, 1] = new int[] { -1, 0 };
        wallKickData[0, 1, 2] = new int[] { -1, 1 };
        wallKickData[0, 1, 3] = new int[] { 0, -2 };
        wallKickData[0, 1, 4] = new int[] { -1, -2 };

        wallKickData[1, 2, 0] = new int[] { 0, 0 };
        wallKickData[1, 2, 1] = new int[] { 1, 0 };
        wallKickData[1, 2, 2] = new int[] { 1, -1 };
        wallKickData[1, 2, 3] = new int[] { 0, 2 };
        wallKickData[1, 2, 4] = new int[] { 1, 2 };

        wallKickData[2, 3, 0] = new int[] { 0, 0 };
        wallKickData[2, 3, 1] = new int[] { 1, 0 };
        wallKickData[2, 3, 2] = new int[] { 1, 1 };
        wallKickData[2, 3, 3] = new int[] { 0, -2 };
        wallKickData[2, 3, 4] = new int[] { 1, -2 };

        wallKickData[3, 0, 0] = new int[] { 0, 0 };
        wallKickData[3, 0, 1] = new int[] { -1, 0 };
        wallKickData[3, 0, 2] = new int[] { -1, -1 };
        wallKickData[3, 0, 3] = new int[] { 0, 2 };
        wallKickData[3, 0, 4] = new int[] { -1, 2 };


        wallKickData[3, 2, 0] = new int[] { 0, 0 };
        wallKickData[3, 2, 1] = new int[] { -1, 0 };
        wallKickData[3, 2, 2] = new int[] { -1, -1 };
        wallKickData[3, 2, 3] = new int[] { 0, 2 };
        wallKickData[3, 2, 4] = new int[] { -1, 2 };

        wallKickData[2, 1, 0] = new int[] { 0, 0 };
        wallKickData[2, 1, 1] = new int[] { -1, 0 };
        wallKickData[2, 1, 2] = new int[] { -1, 1 };
        wallKickData[2, 1, 3] = new int[] { 0, -2 };
        wallKickData[2, 1, 4] = new int[] { -1, -2 };

        wallKickData[1, 0, 0] = new int[] { 0, 0 };
        wallKickData[1, 0, 1] = new int[] { 1, 0 };
        wallKickData[1, 0, 2] = new int[] { 1, -1 };
        wallKickData[1, 0, 3] = new int[] { 0, 2 };
        wallKickData[1, 0, 4] = new int[] { 1, 2 };

        wallKickData[0, 3, 0] = new int[] { 0, 0 };
        wallKickData[0, 3, 1] = new int[] { 1, 0 };
        wallKickData[0, 3, 2] = new int[] { 1, 1 };
        wallKickData[0, 3, 3] = new int[] { 0, -2 };
        wallKickData[0, 3, 4] = new int[] { 1, -2 };
    }

    void PopulateITetrominoWallData()
    {
        iTetrominoWallKickData[0, 1, 0] = new int[] { 0, 0 };
        iTetrominoWallKickData[0, 1, 1] = new int[] { -2, 0 };
        iTetrominoWallKickData[0, 1, 2] = new int[] { 1, 0 };
        iTetrominoWallKickData[0, 1, 3] = new int[] { -2, -1 };
        iTetrominoWallKickData[0, 1, 4] = new int[] { 1, 2 };

        iTetrominoWallKickData[1, 2, 0] = new int[] { 0, 0 };
        iTetrominoWallKickData[1, 2, 1] = new int[] { -1, 0 };
        iTetrominoWallKickData[1, 2, 2] = new int[] { 2, 0 };
        iTetrominoWallKickData[1, 2, 3] = new int[] { -1, 2 };
        iTetrominoWallKickData[1, 2, 4] = new int[] { 2, -1 };

        iTetrominoWallKickData[2, 3, 0] = new int[] { 0, 0 };
        iTetrominoWallKickData[2, 3, 1] = new int[] { 2, 0 };
        iTetrominoWallKickData[2, 3, 2] = new int[] { -1, 0 };
        iTetrominoWallKickData[2, 3, 3] = new int[] { 2, 1 };
        iTetrominoWallKickData[2, 3, 4] = new int[] { -1, -2 };

        iTetrominoWallKickData[3, 0, 0] = new int[] { 0, 0 };
        iTetrominoWallKickData[3, 0, 1] = new int[] { 1, 0 };
        iTetrominoWallKickData[3, 0, 2] = new int[] { -2, 0 };
        iTetrominoWallKickData[3, 0, 3] = new int[] { 1, -2 };
        iTetrominoWallKickData[3, 0, 4] = new int[] { -2, 1 };


        iTetrominoWallKickData[3, 2, 0] = new int[] { 0, 0 };
        iTetrominoWallKickData[3, 2, 1] = new int[] { -2, 0 };
        iTetrominoWallKickData[3, 2, 2] = new int[] { 1, 0 };
        iTetrominoWallKickData[3, 2, 3] = new int[] { -2, -1 };
        iTetrominoWallKickData[3, 2, 4] = new int[] { 1, 2 };

        iTetrominoWallKickData[2, 1, 0] = new int[] { 0, 0 };
        iTetrominoWallKickData[2, 1, 1] = new int[] { 1, 0 };
        iTetrominoWallKickData[2, 1, 2] = new int[] { -2, 0 };
        iTetrominoWallKickData[2, 1, 3] = new int[] { 1, -2 };
        iTetrominoWallKickData[2, 1, 4] = new int[] { -2, 1 };

        iTetrominoWallKickData[1, 0, 0] = new int[] { 0, 0 };
        iTetrominoWallKickData[1, 0, 1] = new int[] { 2, 0 };
        iTetrominoWallKickData[1, 0, 2] = new int[] { -1, 0 };
        iTetrominoWallKickData[1, 0, 3] = new int[] { 2, 1 };
        iTetrominoWallKickData[1, 0, 4] = new int[] { -1, -2 };

        iTetrominoWallKickData[0, 3, 0] = new int[] { 0, 0 };
        iTetrominoWallKickData[0, 3, 1] = new int[] { -1, 0 };
        iTetrominoWallKickData[0, 3, 2] = new int[] { 2, 0 };
        iTetrominoWallKickData[0, 3, 3] = new int[] { -1, 2 };
        iTetrominoWallKickData[0, 3, 4] = new int[] { 2, -1 };
    }

}
