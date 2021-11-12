using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject menuBlackBox;
    public TextMeshProUGUI countDown;
    public GameObject titleMenu;
    public GameObject gameUI;
    public GameObject optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionMenu();
        }
    }

    IEnumerator CountDownUnpause()
    {
        countDown.gameObject.SetActive(true);
        countDown.text = "3";
        yield return new WaitForSeconds(1);
        countDown.text = "2";
        yield return new WaitForSeconds(1);
        countDown.text = "1";
        yield return new WaitForSeconds(1);
        countDown.gameObject.SetActive(false);
        gameManager.TogglePause();
        ToggleBlackBox();
    }

    public void ToggleBlackBox()
    {
        menuBlackBox.gameObject.SetActive(!menuBlackBox.gameObject.activeSelf);
        gameUI.gameObject.SetActive(!gameUI.gameObject.activeSelf);
    }

    public void ToggleTitleMenu()
    {
        titleMenu.gameObject.SetActive(!titleMenu.gameObject.activeSelf);
        ToggleBlackBox();
    }

    public void ToggleOptionMenu()
    {
        optionsMenu.gameObject.SetActive(!optionsMenu.gameObject.activeSelf);

        if (optionsMenu.gameObject.activeSelf == false)
        {
            StartCoroutine(CountDownUnpause());
        }
        else
        {
            gameManager.TogglePause();
            ToggleBlackBox();
        }
    }
}
