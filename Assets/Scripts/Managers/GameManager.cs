using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static event Action LevelCompleted;
    public static event Action LevelFailed;
    public static event Action LoadNextLevel;
    public static event Action RestartLevel;

    public TextMeshProUGUI levelIndexText;


    private void Awake()
    {
        instance = this; 
    }
    void Start()
    {
        LevelManager.instance.Initialize();
    }
    public void LevelComplete()
    {
        LevelCompleted.Invoke();
    } 
    public void LevelFail()
    {
        LevelFailed.Invoke();
    }
    public void InvokeRestartLevelEvent()
    {
        RestartLevel.Invoke();
    }
    public void InvokeLoadNextLevelEvent()
    {
        LoadNextLevel.Invoke();
    }
    public void ChangeLevelText(int i)
    {
        levelIndexText.text = "Level: " + i.ToString();
    }
}
