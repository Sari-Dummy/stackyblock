using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameState : MonoBehaviour
{
    private GameConstants gameConstants;
    public GameObject blockPrefab;

    public GameObject[,] tetrisGrid;

    public GameObject currentTetromino;
    public GameObject heldTetromino;
    public bool tetrominoWasHeld;

    public bool gameIsActive = false;
    public bool gameOver = false;

    public int level = 1;
    public float gravity = 0.01667f;

    public int score = 0;
    public int numOfLinesCleared = 0;
    private int numOfLinesUntilNextLevel = 10;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI lineText;

    public float debugFPS;

    // Start is called before the first frame update
    void Start()
    {
        gameConstants = GameObject.Find("Game Manager").GetComponent<GameConstants>();

        tetrisGrid = new GameObject[gameConstants.tetrisGridWidth, gameConstants.tetrisGridHeight];

        Debug.Log(level);
    }

    // Update is called once per frame
    void Update()
    {
        debugFPS = 1.0f / Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.B))
        {
            RemoveLine(0);
        }
    }

    // Create a block at coordinates, and create a reference to it in tetrisGrid
    public void PlaceBlock(int xCoordinate, int yCoordinate, Color tetrominoColor)
    {
        tetrisGrid[xCoordinate, yCoordinate] = Instantiate(
            blockPrefab,
            new Vector2(xCoordinate, yCoordinate),
            blockPrefab.transform.rotation
        );

        tetrisGrid[xCoordinate, yCoordinate].GetComponent<SpriteRenderer>().color = tetrominoColor;
    }

    // Destroy a block at coordinates, and remove reference to it in tetrisGrid
    public void RemoveBlock(int xCoordinate, int yCoordinate)
    {

        Destroy(tetrisGrid[xCoordinate, yCoordinate]);
        tetrisGrid[xCoordinate, yCoordinate] = null;
    }

    // Removing a line:
    // 1: remove everything on line at y coordinate
    // 2: look if the line above has any block
    // 2A: if yes, move everything there down one block, then repeat step 2 one line above
    // 2B: if no, stop operation
    //

    public bool LineIsFilled(int yCoordinate)
    {
        for (int xCoordinate = 0; xCoordinate < gameConstants.tetrisGridWidth; xCoordinate++)
        {
            if (tetrisGrid[xCoordinate, yCoordinate] == false)
            {
                return false;
            }
        }
        return true;
    }

    public bool LineIsEmpty(int yCoordinate)
    {
        for (int xCoordinate = 0; xCoordinate < gameConstants.tetrisGridWidth; xCoordinate++)
        {
            if (tetrisGrid[xCoordinate, yCoordinate] == true)
            {
                return false;
            }
        }
        return true;
    }

    public bool LineHasBlocks(int yCoordinate)
    {
        for (int xCoordinate = 0; xCoordinate < gameConstants.tetrisGridWidth; xCoordinate++)
        {
            if (tetrisGrid[xCoordinate, yCoordinate] == true)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveLine(int yCoordinate)
    {
        // Remove all blocks at line y
        for (int xCoordinate = 0; xCoordinate < gameConstants.tetrisGridWidth; xCoordinate++)
        {
            RemoveBlock(xCoordinate, yCoordinate);
        }
    }

    public void LowerLine(int yCoordinate, int lowestLine)
    {
        for (int xCoordinate = 0; xCoordinate < gameConstants.tetrisGridWidth; xCoordinate++)
        {
            // Copy line-to-lower to the lowest line
            if (tetrisGrid[xCoordinate, yCoordinate] == true)
            {
                Color blockColor = tetrisGrid[xCoordinate, yCoordinate].GetComponent<SpriteRenderer>().color;
                PlaceBlock(xCoordinate, lowestLine, blockColor);
            }
        }
        // then remove the original line-to-lower
        RemoveLine(yCoordinate);
    }

    public void RemoveLinesFromNewTetromino(int lowestLine, int highestLine, bool isTSpin)
    {
        bool lineWasRemoved = false;
        int numOfRemovedLines = 0;

        // Whenever lowerLine is used, it lowers the line to the lowest empty line.
        int lowestEmptyLine = 0;

        for (int line = lowestLine; line <= highestLine; line++)
        {
            if (LineIsFilled(line))
            {
                RemoveLine(line);
                numOfRemovedLines++;
                numOfLinesCleared++;
                numOfLinesUntilNextLevel--;

                if (lineWasRemoved == false)
                {
                    lowestEmptyLine = line;
                    lineWasRemoved = true;
                }
            }
        }

        if (lineWasRemoved)
        {
            bool linesAboveGap = false;

            // If the line to check has blocks, then lower its blocks down to the lowest line and increment it.
            int lineToCheck = lowestEmptyLine + 1;

            // First, check above the empty line up to the highest line that the tetromino affected directly.
            for (int line = lineToCheck; line <= highestLine; line++)
            {
                if (LineHasBlocks(line))
                {
                    LowerLine(line, lowestEmptyLine);
                    lowestEmptyLine++;
                }
            }

            // Then check right above the highest line.
            lineToCheck = highestLine + 1;
            if (LineHasBlocks(lineToCheck))
            {
                LowerLine(lineToCheck, lowestEmptyLine);
                lowestEmptyLine++;
                lineToCheck++;
                linesAboveGap = true;
            }

            // If there was a non-empty line right above the highest line, then continuously check each line above
            // until you find an empty line.
            while (linesAboveGap)
            {
                if (LineHasBlocks(lineToCheck))
                {
                    LowerLine(lineToCheck, lowestEmptyLine);
                    lowestEmptyLine++;
                    lineToCheck++;
                }
                else
                {
                    linesAboveGap = false;
                }
            }

            // Update score
            if (isTSpin == false)
            {
                AddScore(gameConstants.scoreFromRemovedLines[numOfRemovedLines] * level);
            }
            else
            {
                AddScore(numOfRemovedLines * 400 * level);
            }

            // Update level
            if (numOfLinesUntilNextLevel <= 0)
            {
                numOfLinesUntilNextLevel += 10;
                UpdateLevel(level + 1);
            }

            // Update line number
            lineText.text = numOfLinesCleared.ToString();
        }
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
    }

    public void UpdateLevel(int newLevel)
    {
        Debug.Log("level " + level);
        Debug.Log("new level " + newLevel);
        level = newLevel;
        levelText.text = level.ToString();
        if (level < 21)
        {
            gravity = gameConstants.gravityAtLevel[level];
        }
        else
        {
            gravity = gameConstants.gravityAtLevel[20];
        }
    }
}
