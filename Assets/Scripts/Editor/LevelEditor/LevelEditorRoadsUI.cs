using System;
using Code.LevelEditor;
using UnityEditor;
using UnityEngine;

public class LevelEditorRoadsUI
{
    private static bool roadButtonsGroup;
    private readonly LevelEditorWindow _levelEditorWindow;
    private FTFEditorWindow _ftfEditor;

    public LevelEditorRoadsUI(LevelEditorWindow levelEditorWindow, FTFEditorWindow ftfEditor)
    {
        _levelEditorWindow = levelEditorWindow;
        _ftfEditor = ftfEditor;
    }

    public void DrawRoadsUI()
    {
        roadButtonsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(roadButtonsGroup, "Roads");

        if (roadButtonsGroup)
        {
            EditorGUI.indentLevel++;
            DrawRoadButtons();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public void DrawRoadButtons()
    {
        var itemWidth = 100; // Mathf.FloorToInt((_levelEditorWindow.position.width - 40) / itemPerRow);
        var itemPerRow = Math.Max(1, Mathf.FloorToInt((_ftfEditor.position.width - 40) / itemWidth));
        var c = 0;
        EditorGUILayout.BeginVertical();
        foreach (var k in _levelEditorWindow.roadButtons.Keys)
        {
            if (c % itemPerRow == 0)
            {
                if (c > 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

                EditorGUILayout.BeginHorizontal();
            }

            DrawRoadButton(k, itemWidth);

            c++;
        }

        if (c > 0) EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawRoadButton(string k, int width)
    {
        var p = _levelEditorWindow.roadButtons[k];
        var g = new GUIContent(p.preview);
        g.tooltip = p.name;
        EditorGUILayout.BeginVertical();
        GUILayout.Label(p.name);
        if (GUILayout.Button(g, GUILayout.Width(width), GUILayout.Height(width))) p.callback(p);
        EditorGUILayout.EndVertical();
    }
}