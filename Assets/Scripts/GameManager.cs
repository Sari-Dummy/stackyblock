using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameState gameState;
    private GameConstants gameConstants;
    private UIManager UIManager;

    public GameObject[] tetrominos;

    // tetrominoBag is a list of prefab from which to spawn tetrominos in the actual well.
    public List<GameObject> tetrominoBag;

    // Meanwhile, this list should be the list of actual game objects that are displayed on the screen, representing
    // the next tetrominos to be spawned.
    public List<GameObject> nextTetrominosDisplay;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        gameState = GameObject.Find("Game Manager").GetComponent<GameState>();
        gameConstants = GameObject.Find("Game Manager").GetComponent<GameConstants>();
        UIManager = GameObject.Find("UI Manager").GetComponent<UIManager>();

        // Called twice to fill it up the "current bag" and the "backup bag".
        RefillTetrominoBag();
        RefillTetrominoBag();

        InitiateNextTetrominosDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG cheat
        if (Input.GetKeyDown(KeyCode.X))
        {
            SpawnTetrominoFromBag();
        }
    }

    void RefillTetrominoBag()
    {
        // Generate a temporary list of all the tetrominos
        List<GameObject> tetrominosToDraw = new List<GameObject>();
        tetrominosToDraw.AddRange(tetrominos);
        // Add this to the actual bag that spawns tetrominos
        List<GameObject> newTetrominoBag = new List<GameObject>();

        // Move the tetrominos from tetrominosToDraw into newTetrominoBag in a random order
        while (tetrominosToDraw.Count > 0)
        {
            int index = Random.Range(0, tetrominosToDraw.Count);
            GameObject drawnTetromino = tetrominosToDraw[index];
            newTetrominoBag.Add(drawnTetromino);
            tetrominosToDraw.RemoveAt(index);
        }

        tetrominoBag.AddRange(newTetrominoBag);
    }

    public void SpawnTetrominoFromBag()
    {
        // Spawn the first tetromino from the bag and remove it
        gameState.currentTetromino = Instantiate(tetrominoBag[0], gameConstants.spawnPosition, tetrominoBag[0].transform.rotation);
        tetrominoBag.RemoveAt(0);

        // If the bag only has 7 elements left, refill it.
        if (tetrominoBag.Count <= 7)
        {
            RefillTetrominoBag();
        }

        UpdateNextTetrominosDisplay();

        gameState.tetrominoWasHeld = false;
    }

    void InitiateNextTetrominosDisplay()
    {
        for (int tetromino = 0; tetromino < gameConstants.nextTetrominosToDisplay; tetromino++)
        {
            Vector3Int displayPos = new Vector3Int(
                gameConstants.nextTetrominoDisplayXPos,
                gameConstants.nextTetrominoDisplayYPos[tetromino],
                0
            );

            nextTetrominosDisplay.Add(Instantiate(
                tetrominoBag[tetromino],
                displayPos,
                tetrominoBag[tetromino].transform.rotation
            ));

            nextTetrominosDisplay[tetromino].GetComponent<BlockController>().enabled = false;
        }
    }

    // Called when a new tetromino is spawned.
    void UpdateNextTetrominosDisplay()
    {
        int lastTetrominoIndex = gameConstants.nextTetrominosToDisplay - 1;

        Vector3Int displayPos = new Vector3Int(
        gameConstants.nextTetrominoDisplayXPos,
        gameConstants.nextTetrominoDisplayYPos[lastTetrominoIndex],
        0
        );

        // Remove tetromino that was just spawned
        Destroy(nextTetrominosDisplay[0]);
        nextTetrominosDisplay.RemoveAt(0);

        // Translate each tetromino down to make space for new one
        foreach (GameObject tetrominoDisplay in nextTetrominosDisplay)
        {
            tetrominoDisplay.transform.Translate(new Vector3Int(0, 3, 0));
        }

        // Add tetromino to display
        nextTetrominosDisplay.Add(Instantiate(
            tetrominoBag[lastTetrominoIndex],
            displayPos,
            tetrominoBag[lastTetrominoIndex].transform.rotation
        ));

        nextTetrominosDisplay[lastTetrominoIndex].GetComponent<BlockController>().enabled = false;
    }

    void SpawnHeldTetromino()
    {
        gameState.heldTetromino.GetComponent<BlockController>().enabled = true;
        Instantiate(gameState.heldTetromino, gameConstants.spawnPosition, gameState.heldTetromino.transform.rotation);
    }


    public void HoldTetromino(GameObject tetromino)
    {
        // If there is a held tetromino, then swap out the current one for the held one
        if (gameState.heldTetromino)
        {
            SpawnHeldTetromino();
            Destroy(gameState.heldTetromino);
        }
        // Otherwise, for the first holding in the game, spawn a tetromino from the bag
        else
        {
            SpawnTetrominoFromBag();
        }

        gameState.tetrominoWasHeld = true;
        gameState.heldTetromino = Instantiate(
            tetromino,
            gameConstants.heldTetrominoPos,
            tetromino.transform.rotation
        );
        gameState.heldTetromino.GetComponent<BlockController>().enabled = false;
    }


    //
    // UI
    //

    public void TogglePause()
    {
        gameState.gameIsActive = !gameState.gameIsActive;
    }

    void ToggleTitleMenu()
    {

    }

    void ToggleOptionMenu()
    {
        TogglePause();
        UIManager.ToggleOptionMenu();
    }

    void ToggleGameOverMenu()
    {

    }

    public void GameOver()
    {
        gameState.gameOver = true;
        gameState.gameIsActive = false;
        // Game over menu is active
    }

    public void StartGame()
    {
        gameState.gameIsActive = true;
        gameState.gameOver = false;
        UIManager.ToggleTitleMenu();

        SpawnTetrominoFromBag();
    }

    public void RestartGame()
    {

    }
}
