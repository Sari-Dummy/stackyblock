using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindsManager : MonoBehaviour
{
    public Dictionary<string, KeyCode> keyBinds = new Dictionary<string, KeyCode>();

    public Text Left, Right, RotateLeft, RotateRight, SoftDrop, HardDrop, Hold;

    public Color normalColor;
    public Color selectedColor;

    private GameObject currentKey;

    // Start is called before the first frame update
    void Start()
    {
        LoadKeyBinds();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (currentKey != null)
        {
            Event newEvent = Event.current;
            if (newEvent.isKey)
            {
                keyBinds[currentKey.name] = newEvent.keyCode;
                currentKey.transform.GetChild(0).GetComponent<Text>().text = newEvent.keyCode.ToString();
                currentKey.GetComponent<Image>().color = normalColor;

                // Save "Up" to "UpArrow" for example
                Debug.Log(currentKey.name + " " + newEvent.keyCode.ToString());
                PlayerPrefs.SetString(currentKey.name, newEvent.keyCode.ToString());

                currentKey = null;
            }
        }
    }

    public void ChangeKey(GameObject clickedBinding)
    {
        if (currentKey != null)
        {
            currentKey.GetComponent<Image>().color = normalColor;
        }

        currentKey = clickedBinding;
        currentKey.GetComponent<Image>().color = selectedColor;
    }

    public void LoadKeyBinds()
    {
        keyBinds.Add("Left",
        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "LeftArrow")));

        keyBinds.Add("Right",
        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "RightArrow")));

        keyBinds.Add("RotateLeft",
        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RotateLeft", "Z")));

        keyBinds.Add("RotateRight",
        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RotateRight", "UpArrow")));

        keyBinds.Add("SoftDrop",
        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("SoftDrop", "DownArrow")));

        keyBinds.Add("HardDrop",
        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("HardDrop", "Space")));

        keyBinds.Add("Hold",
        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hold", "C")));

        Left.text = keyBinds["Left"].ToString();
        Right.text = keyBinds["Right"].ToString();
        RotateLeft.text = keyBinds["RotateLeft"].ToString();
        RotateRight.text = keyBinds["RotateRight"].ToString();
        SoftDrop.text = keyBinds["SoftDrop"].ToString();
        HardDrop.text = keyBinds["HardDrop"].ToString();
        Hold.text = keyBinds["Hold"].ToString();
    }

    public void ResetKeyBinds()
    {
        PlayerPrefs.DeleteKey("Left");
        PlayerPrefs.DeleteKey("Right");
        PlayerPrefs.DeleteKey("RotateLeft");
        PlayerPrefs.DeleteKey("RotateRight");
        PlayerPrefs.DeleteKey("SoftDrop");
        PlayerPrefs.DeleteKey("HardDrop");
        PlayerPrefs.DeleteKey("Hold");

        keyBinds = new Dictionary<string, KeyCode>();

        LoadKeyBinds();
    }
}
