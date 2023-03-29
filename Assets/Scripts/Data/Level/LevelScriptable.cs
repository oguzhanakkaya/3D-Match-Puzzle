using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class LevelScriptable : ScriptableObject
{
    public string Name;
    public int LevelID;
    [SerializeField] private Vector2 levelBorder;
    public Vector2 LevelBorder { get => levelBorder; set => levelBorder = value; }

    public List<CapturedData> CapturedDatas;

}
