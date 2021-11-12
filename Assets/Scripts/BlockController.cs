using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private GameConstants gameConstants;
    private GameState gameState;
    private GameManager gameManager;
    private WallKickData wallKickData;
    private KeyBindsManager keyBindsManager;

    public bool debugLockDelay;

    // Rotation states are made in the unity editor
    [SerializeField] List<GameObject> rotationStates = null;
    [SerializeField] int currentRotationState = 0;

    [SerializeField] string tetrominoType;
    public Color tetrominoColor;

    public List<GameObject> ghostBlocksDisplay;
    BlocksCoordinates validGhostBlockCoordinates;



    // Keeps track of how long the arrow keys are being held down, resets when held up
    private float timeHeldKey = 0;

    // Number of seconds in between each movement down
    private float attraction = 0f;
    private float softDropMult = 10;

    public int lockDelayCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        lockDelayCount = 0;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameConstants = GameObject.Find("Game Manager").GetComponent<GameConstants>();
        gameState = GameObject.Find("Game Manager").GetComponent<GameState>();
        wallKickData = GameObject.Find("Game Manager").GetComponent<WallKickData>();
        keyBindsManager = GameObject.Find("KeyBindsManager").GetComponent<KeyBindsManager>();

        validGhostBlockCoordinates = new BlocksCoordinates(gameConstants.numOfBlocks);
        UpdateGhostBlockDisplay();
        StartCoroutine(AddLockDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.gameIsActive)
        {
            BlockFalls();

            if (Input.GetKey(keyBindsManager.keyBinds["Left"])
            && !Input.GetKey(keyBindsManager.keyBinds["Right"])
            && transform.position.x > gameConstants.leftBorder)
            {
                MoveBlockOnXAxis(-1);
            }

            if (Input.GetKey(keyBindsManager.keyBinds["Right"])
            && !Input.GetKey(keyBindsManager.keyBinds["Left"])
            && transform.position.x < gameConstants.rightBorder)
            {
                MoveBlockOnXAxis(1);
            }

            if (Input.GetKeyUp(keyBindsManager.keyBinds["Left"]) || Input.GetKeyUp(keyBindsManager.keyBinds["Right"]))
            {
                timeHeldKey = 0;
            }

            if (Input.GetKeyDown(keyBindsManager.keyBinds["RotateRight"])
            && tetrominoType != "O")
            {
                HandleRotationInput(1);
            }

            if (Input.GetKeyDown(keyBindsManager.keyBinds["RotateLeft"])
            && tetrominoType != "O")
            {
                HandleRotationInput(-1);
            }


            if (Input.GetKeyDown(keyBindsManager.keyBinds["HardDrop"]))
            {
                AddScoreFromHardDrop();
                DropBlockAt(validGhostBlockCoordinates);
            }

            if (Input.GetKeyDown(keyBindsManager.keyBinds["Hold"]))
            {
                if (gameState.tetrominoWasHeld == false)
                {
                    gameManager.HoldTetromino(gameObject);
                    DestroyTetromino();
                }
            }
        }


        // Debug

        // StartCoroutine(AddLockDelay());
    }

    //
    // COORDINATE LOGIC
    //

    class BlocksCoordinates
    {
        public int[] x;
        public int[] y;

        public BlocksCoordinates(int numOfBlocks)
        {
            x = new int[numOfBlocks];
            y = new int[numOfBlocks];
        }
    }

    // Gives the coordinates of a rotation state
    BlocksCoordinates CoordinatesOfState(int rotationStateIndex)
    {
        BlocksCoordinates coordinatesToReturn = new BlocksCoordinates(gameConstants.numOfBlocks);

        // X coordinates of each block
        for (int i = 0; i < gameConstants.numOfBlocks; i++)
        {
            coordinatesToReturn.x[i] = Mathf.RoundToInt(
                rotationStates[rotationStateIndex].transform.Find("Block (" + i + ")").position.x
            );
        }

        // Y coordinates
        for (int i = 0; i < gameConstants.numOfBlocks; i++)
        {
            coordinatesToReturn.y[i] = Mathf.RoundToInt(
                rotationStates[rotationStateIndex].transform.Find("Block (" + i + ")").position.y
            );
        }

        return coordinatesToReturn;
    }

    BlocksCoordinates CurrentStateCoordinates()
    {
        return CoordinatesOfState(currentRotationState);
    }

    // This looks at a potential tetrimino state's coordinates to see if it's inside the borders
    bool isOutsideBoundaries(BlocksCoordinates stateToCheck)
    {
        // Look at each block inside a rotation state
        foreach (int yPosition in stateToCheck.y)
        {
            // Is the block below the floor?
            if (yPosition < gameConstants.lowerBorder)
            {
                return true;
            }
        }

        // Is it outside the right or let boundaries?
        foreach (int xPosition in stateToCheck.x)
        {
            if (xPosition < gameConstants.leftBorder || xPosition > gameConstants.rightBorder)
            {
                return true;
            }
        }

        return false;
    }

    // Looks at a potential tetromino state to see if any of its blocks overlap with blocks that are already on
    // the grid.
    bool overlapsWithBlock(BlocksCoordinates stateToCheck)
    {
        for (int i = 0; i < gameConstants.numOfBlocks; i++)
        {
            if (stateToCheck.x[i] < gameState.tetrisGrid.GetLength(0) &&
                stateToCheck.x[i] >= 0 &&
                stateToCheck.y[i] < gameState.tetrisGrid.GetLength(1) &&
                stateToCheck.y[i] >= 0)
            {
                if (gameState.tetrisGrid[stateToCheck.x[i], stateToCheck.y[i]] == true)
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool CoordinatesAreValid(BlocksCoordinates stateToCheck)
    {
        if (overlapsWithBlock(stateToCheck) || isOutsideBoundaries(stateToCheck))
        {
            return false;
        }
        return true;
    }

    // Calculate new blocks coordinates with a reduced Y value relative
    // If it's valid, lower Y value
    // If it's invalid, then show the ghost block at the previous Y value


    public void DestroyGhostBlocks()
    {
        foreach (GameObject block in ghostBlocksDisplay)
        {
            Destroy(block);
            ghostBlocksDisplay = new List<GameObject>();
        }
    }

    public void UpdateGhostBlockDisplay()
    {
        // Let's start with the coordinates of where the tetromino is now.
        BlocksCoordinates ghostBlockCoordinates = CurrentStateCoordinates();

        // Lowers ghost coordinates by 1
        void LowerGhostBlockCoordinates()
        {
            for (int yIndex = 0; yIndex < ghostBlockCoordinates.y.Length; yIndex++)
            {
                ghostBlockCoordinates.y[yIndex] -= 1;
            }
        }

        // Shows the ghost
        void SpawnGhostBlocks(BlocksCoordinates blockCoordinates)
        {
            if (gameConstants.ghostPieceIsEnabled == true)
            {
                for (int currentBlock = 0; currentBlock < gameConstants.numOfBlocks; currentBlock++)
                {
                    int xPos = blockCoordinates.x[currentBlock];
                    int yPos = blockCoordinates.y[currentBlock];
                    ghostBlocksDisplay.Add(Instantiate(
                        gameState.blockPrefab,
                        new Vector3Int(xPos, yPos, 0),
                        gameState.blockPrefab.transform.rotation
                    ));

                    Color ghostBlockColor = tetrominoColor;
                    ghostBlockColor.a = 0.5f;
                    ghostBlocksDisplay[currentBlock].GetComponent<SpriteRenderer>().color = ghostBlockColor;
                }
            }
        }

        // First destroy the old ghost
        DestroyGhostBlocks();


        if (!CoordinatesAreValid(ghostBlockCoordinates))
        {
            return;
        }
        else
        {
            while (CoordinatesAreValid(ghostBlockCoordinates))
            {
                validGhostBlockCoordinates.x = (int[])ghostBlockCoordinates.x.Clone();
                validGhostBlockCoordinates.y = (int[])ghostBlockCoordinates.y.Clone();

                LowerGhostBlockCoordinates();
            }

            SpawnGhostBlocks(validGhostBlockCoordinates);
        }
    }

    //
    // MOVEMENT LOGIC
    //

    void TranslateTetrominoXAxis(int direction)
    {
        Vector3Int originalPosition = Vector3Int.RoundToInt(transform.position);
        transform.Translate(Vector3Int.right * direction);

        if (CoordinatesAreValid(CurrentStateCoordinates()) == false)
        {
            transform.position = originalPosition;
        }
        else
        {
            UpdateGhostBlockDisplay();
            StartCoroutine(AddLockDelay());
        }
    }

    void MoveBlockOnXAxis(int input)
    {
        if (timeHeldKey == 0)
        {
            TranslateTetrominoXAxis(input);
        }
        timeHeldKey += Time.deltaTime;
        if (timeHeldKey > gameConstants.autoRepeatDelay / 1000)
        {
            TranslateTetrominoXAxis(input);
            // This repeats the if statement after autoRepeatSpeed time has passed
            timeHeldKey -= gameConstants.autoRepeatSpeed / 1000;
        }
    }

    void AttemptMoveDown(int numOfLinesToFall)
    {
        Vector3Int originalPosition = Vector3Int.RoundToInt(transform.position);

        // Fall one more line than needed to check if there are blocks or void right below the supposed landing location
        int linesLeftToFall = numOfLinesToFall + 1;

        // Either go all the way one below where you're supposed to land, or go where coordinates are invalid
        while (CoordinatesAreValid(CurrentStateCoordinates()) && linesLeftToFall > 0)
        {
            transform.Translate(Vector3Int.down);
            linesLeftToFall--;
        }

        // which means always raise the tetromino one up
        transform.Translate(Vector3Int.up);

        // if linesLeftToFall is more than 0 then the block should try to deposit
        if (linesLeftToFall > 0)
        {
            if (!LockDelayIsOn())
            {
                DropBlockAt(CurrentStateCoordinates());
            }
        }
        else
        {
            StartCoroutine(AddLockDelay());
            if (Input.GetKey(KeyCode.DownArrow))
            {
                gameState.AddScore(numOfLinesToFall);
            }
        }
    }

    void BlockFalls()
    {
        float mult = 1;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            mult = softDropMult;
        }
        attraction += gameState.gravity * mult;
        if (attraction > 1)
        {
            // fall by however many lines above 1 "attraction" is and reset it to 0
            int numOfLinesToFall = Mathf.FloorToInt(attraction);
            AttemptMoveDown(numOfLinesToFall);
            attraction = 0;
        }
    }


    //
    // ROTATION LOGIC
    //
    void ChangeRotationStateTo(int newRotationState)
    {
        rotationStates[currentRotationState].SetActive(false);
        rotationStates[newRotationState].SetActive(true);
        currentRotationState = newRotationState;

        UpdateGhostBlockDisplay();
        StartCoroutine(AddLockDelay());
    }

    void RotateTetromino(int newRotationState)
    {
        BlocksCoordinates newStateCoordinates;
        Vector3Int originalPosition = Vector3Int.RoundToInt(transform.position);

        // Try to rotate it normally.
        newStateCoordinates = CoordinatesOfState(newRotationState);
        if (CoordinatesAreValid(newStateCoordinates))
        {
            ChangeRotationStateTo(newRotationState);
        }
        // If that fails, try every translation possible.
        else
        {
            // wallKickData[currentState, newState, wallKickTranslations]{x, y}
            int numOfWallKickTests = wallKickData.wallKickData.GetLength(2);

            // i = 1 because attempting a normal rotation is i = 0 and we already tried that
            for (int wallKickTest = 1; wallKickTest < numOfWallKickTests; wallKickTest++)
            {
                // First translate the tetromino according to wallKickData
                int xTranslation = wallKickData.wallKickData[currentRotationState, newRotationState, wallKickTest][0];
                int yTranslation = wallKickData.wallKickData[currentRotationState, newRotationState, wallKickTest][1];
                transform.Translate(new Vector3Int(xTranslation, yTranslation, 0));

                // Then check if that would make rotation valid
                // If yes then rotate the block and end it here
                newStateCoordinates = CoordinatesOfState(newRotationState);
                if (CoordinatesAreValid(newStateCoordinates))
                {
                    ChangeRotationStateTo(newRotationState);
                    return;
                }
                // If not, then return the block to its original position and don't rotate
                else
                {
                    transform.position = originalPosition;
                }
            }
        }
    }

    void HandleRotationInput(int input)
    {
        int numOfStates = rotationStates.Count;
        // Set newRotationState according to input.
        // If it's more than the numOfStates, then use % numOfStates to set it to 0. (Last state -> First state)
        // If it's -1, set it to numOfStates instead. (First state -> Last state)
        int newRotationState = (currentRotationState + input) % numOfStates;
        if (newRotationState < 0)
        {
            newRotationState = numOfStates - 1;
        }

        RotateTetromino(newRotationState);
    }

    //
    // BLOCK STATE LOGIC
    //

    void PlaceBlocks(BlocksCoordinates coordinatesOfPlacement)
    {
        for (int i = 0; i < gameConstants.numOfBlocks; i++)
        {
            gameState.PlaceBlock(coordinatesOfPlacement.x[i], coordinatesOfPlacement.y[i], tetrominoColor);
        }
    }

    int LowestLine(BlocksCoordinates currentState)
    {
        int lowestLine = gameConstants.tetrisGridHeight;

        foreach (int yCoordinate in currentState.y)
        {
            lowestLine = Mathf.Min(lowestLine, yCoordinate);
        }

        return lowestLine;
    }

    int HighestLine(BlocksCoordinates currentState)
    {
        int highestLine = 0;

        foreach (int yCoordinate in currentState.y)
        {
            highestLine = Mathf.Max(highestLine, yCoordinate);
        }

        return highestLine;
    }

    IEnumerator AddLockDelay()
    {
        lockDelayCount++;
        yield return new WaitForSeconds(gameConstants.lockDelay);
        lockDelayCount--;
    }

    bool LockDelayIsOn()
    {
        if (lockDelayCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsTSpinPosition(BlocksCoordinates stateCoordinates)
    {
        BlocksCoordinates tSpinTestCoords = new BlocksCoordinates(gameConstants.numOfBlocks);

        tSpinTestCoords.x = (int[])stateCoordinates.x.Clone();
        tSpinTestCoords.y = (int[])stateCoordinates.y.Clone();

        // Is the position above valid?
        for (int block = 0; block < gameConstants.numOfBlocks; block++)
        {
            tSpinTestCoords.y[block] += 1;
        }
        if (CoordinatesAreValid(tSpinTestCoords))
        {
            return false;
        }

        // Is the position to the right valid?
        for (int block = 0; block < gameConstants.numOfBlocks; block++)
        {
            // Cancel previous movement
            tSpinTestCoords.y[block] -= 1;
            // Move left
            tSpinTestCoords.x[block] += 1;
        }
        if (CoordinatesAreValid(tSpinTestCoords))
        {
            return false;
        }

        // Is the position to the left valid?
        for (int block = 0; block < gameConstants.numOfBlocks; block++)
        {
            tSpinTestCoords.x[block] -= 2;
        }
        if (CoordinatesAreValid(tSpinTestCoords))
        {
            return false;
        }

        // By this point, if coordinates to the left, right, or up were valid, the function would have returned false.
        // This is therefore a t-spin.
        return true;
    }

    void DropBlockAt(BlocksCoordinates stateCoordinates)
    {
        // Check for T-spin
        bool isTSpin = false;

        if (tetrominoType == "T")
        {
            if (IsTSpinPosition(stateCoordinates))
            {
                isTSpin = true;
                gameState.AddScore(400 * gameState.level);
            }
        }

        PlaceBlocks(stateCoordinates);

        // Check for gameover
        if (HighestLine(stateCoordinates) > 20)
        {
            gameManager.GameOver();
        }
        else
        {
            gameState.RemoveLinesFromNewTetromino(LowestLine(stateCoordinates), HighestLine(stateCoordinates), isTSpin);
        }

        DestroyTetromino();

        if (gameState.gameOver == false)
        {
            gameManager.SpawnTetrominoFromBag();
        }
    }

    void AddScoreFromHardDrop()
    {
        int numOfCells = LowestLine(CurrentStateCoordinates()) - LowestLine(validGhostBlockCoordinates);

        gameState.AddScore(numOfCells * 2);
    }

    void DestroyTetromino()
    {
        DestroyGhostBlocks();
        Destroy(gameObject);
    }
}
