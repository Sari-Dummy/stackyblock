using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    private GameConstants gameConstants;
    private GameState gameState;

    public TextMeshProUGUI autoRepeatDelayText;
    public TextMeshProUGUI autoRepeatSpeedText;
    public Text ghostPieceText;

    private int autoRepeatIncrement = 10;
    private int minAutoRepeatValue = 20;
    private int maxAutoRepeatValue = 500;

    private int defaultAutoRepeatDelay = 170;
    private int defaultAutoRepeatSpeed = 50;
    private bool defaultGhostPiece = true;

    // Start is called before the first frame update
    void Start()
    {
        gameConstants = GameObject.Find("Game Manager").GetComponent<GameConstants>();
        gameState = GameObject.Find("Game Manager").GetComponent<GameState>();

        LoadOptions();
    }

    void SaveAutoRepeatDelay()
    {
        PlayerPrefs.SetFloat("autoRepeatDelay", gameConstants.autoRepeatDelay);
    }

    public void DecreaseAutoRepeatDelay()
    {
        if (gameConstants.autoRepeatDelay > minAutoRepeatValue)
        {
            gameConstants.autoRepeatDelay -= autoRepeatIncrement;
            autoRepeatDelayText.text = gameConstants.autoRepeatDelay.ToString() + " ms";
            SaveAutoRepeatDelay();
        }
    }

    public void IncreaseAutoRepeatDelay()
    {
        if (gameConstants.autoRepeatDelay < maxAutoRepeatValue)
        {
            gameConstants.autoRepeatDelay += autoRepeatIncrement;
            autoRepeatDelayText.text = gameConstants.autoRepeatDelay.ToString() + " ms";
            SaveAutoRepeatDelay();
        }
    }

    void SaveAutoRepeatSpeed()
    {
        PlayerPrefs.SetFloat("autoRepeatSpeed", gameConstants.autoRepeatSpeed);
    }

    public void DecreaseAutoRepeatSpeed()
    {
        if (gameConstants.autoRepeatSpeed > minAutoRepeatValue)
        {
            gameConstants.autoRepeatSpeed -= autoRepeatIncrement;
            autoRepeatSpeedText.text = gameConstants.autoRepeatSpeed.ToString() + " ms";
            SaveAutoRepeatSpeed();
        }
    }

    public void IncreaseAutoRepeatSpeed()
    {
        if (gameConstants.autoRepeatSpeed < maxAutoRepeatValue)
        {
            gameConstants.autoRepeatSpeed += autoRepeatIncrement;
            autoRepeatSpeedText.text = gameConstants.autoRepeatSpeed.ToString() + " ms";
            SaveAutoRepeatSpeed();

            Debug.Log(PlayerPrefs.GetInt("autoRepeatSpeed"));
        }
    }

    void UpdateGhostPieceText()
    {
        if (gameConstants.ghostPieceIsEnabled)
        {
            ghostPieceText.text = "Enabled";
            gameState.currentTetromino.GetComponent<BlockController>().UpdateGhostBlockDisplay();
        }
        else
        {
            ghostPieceText.text = "Disabled";
            gameState.currentTetromino.GetComponent<BlockController>().DestroyGhostBlocks();
        }
    }

    public void ToggleGhostPiece()
    {
        gameConstants.ghostPieceIsEnabled = !gameConstants.ghostPieceIsEnabled;
        UpdateGhostPieceText();

        if (gameConstants.ghostPieceIsEnabled == true)
        {
            PlayerPrefs.SetString("GhostPiece", "true");
        }
        else
        {
            PlayerPrefs.SetString("GhostPiece", "false");
        }
    }

    public void LoadOptions()
    {
        gameConstants.autoRepeatDelay = PlayerPrefs.GetFloat("autoRepeatDelay", defaultAutoRepeatDelay);
        autoRepeatDelayText.text = gameConstants.autoRepeatDelay.ToString() + " ms";

        gameConstants.autoRepeatSpeed = PlayerPrefs.GetFloat("autoRepeatSpeed", defaultAutoRepeatSpeed);
        autoRepeatSpeedText.text = gameConstants.autoRepeatSpeed.ToString() + " ms";

        if (PlayerPrefs.GetString("GhostPiece") == "false")
        {
            gameConstants.ghostPieceIsEnabled = false;
        }
        else
        {
            gameConstants.ghostPieceIsEnabled = true;
        }
        UpdateGhostPieceText();
    }

    public void ResetOptions()
    {
        PlayerPrefs.DeleteKey("autoRepeatDelay");
        PlayerPrefs.DeleteKey("autoRepeatSpeed");
        PlayerPrefs.DeleteKey("GhostPiece");

        LoadOptions();
    }
}
