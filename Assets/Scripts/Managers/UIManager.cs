using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject winPanel, lostPanel;
    public Button restartLevelButton, nextLevelButton;
    void Start()
    {
        GameManager.LevelCompleted += OpenWinPanel;
        GameManager.LevelFailed += OpenLosePanel;


        restartLevelButton.onClick.AddListener(RestartButtonClicked);
        nextLevelButton.onClick.AddListener(NextLevelButtonClicked); 
    }
    private void OpenWinPanel()
    {
        winPanel.SetActive(true);
    }
    private void OpenLosePanel()
    {
        lostPanel.SetActive(true);
    }
    private void RestartButtonClicked()
    {
        lostPanel.SetActive(false);
        LevelManager.instance.RecycleAll();
        GameManager.instance.InvokeRestartLevelEvent();
    }
    private void NextLevelButtonClicked()
    {
        winPanel.SetActive(false);
        LevelManager.instance.RecycleAll();
        GameManager.instance.InvokeLoadNextLevelEvent();
    }
}
