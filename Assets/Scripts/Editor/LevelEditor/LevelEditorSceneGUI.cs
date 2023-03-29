using Code.LevelEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


[InitializeOnLoad]
public class LevelEditorSceneGUI : Editor
{
    private const int NONE = 0;
    private const int SELECT = 1;
    private const int PAINT = 2;
    private const int ERASE = 3;
    public const int toolbarHeight = 40;
    private static bool canPaint;
    private static readonly string[] buttons = { "None", "Select", "Paint", "Erase" };
    private static GameObject currentObject;
    private static GameObject lastObject;

    private static int roadLayer = 1 << 8;

    static LevelEditorSceneGUI()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
        EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
        EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
    }

    public static int SelectedTool
    {
        get => EditorPrefs.GetInt("LevelEditorSelectedTool", 0);
        set
        {
            if (value == SelectedTool) return;
            EditorPrefs.SetInt("LevelEditorSelectedTool", value);


            switch (value)
            {
                case NONE:
                    Tools.hidden = false;
                    break;
                case SELECT:
                    Tools.hidden = true;
                    break;
                case PAINT:
                    Tools.hidden = true;
                    LevelEditorPaintItemsUI.levelItemsGroup = true;
                    break;
                case ERASE:
                    Tools.hidden = true;
                    break;
            }
        }
    }


    private static void OnSceneGUI(SceneView sceneView)
    {
        if (IsValid())
        {
            Tools.hidden = SelectedTool != NONE;
        }
        else
        {
            Tools.hidden = false;
            Init();
            return;
        }

        if (Application.isPlaying)
            return;

        DrawToolsMenu(sceneView.position);
        DrawHandle(sceneView);
        HandleMouseInput();
        HandleKeyboardInput();
        // sceneView.Repaint();


    }

    private static void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (IsValid())
        {
            Tools.hidden = SelectedTool != NONE;
            Init();
        }
        else
        {
            Tools.hidden = false;
        }
    }

    public static void Init()
    {
        currentObject = null;
        lastObject = null;
        canPaint = false;
    }


    private static void DrawHandle(SceneView sceneView)
    {
        if (SelectedTool == NONE)
        {
            LevelEditorPaintItemsUI.DisablePreviewItem();   
            return;
        }


        if (SelectedTool == ERASE || SelectedTool == SELECT)
        {
            LevelEditorPaintItemsUI.DisablePreviewItem();
            DrawSelectHandle(sceneView);
        }
        else if (SelectedTool == PAINT)
        {
            DrawPaintHandle(sceneView);
        }
    }


    private static void DrawPaintHandle(SceneView sceneView)
    {
        var pos = Event.current.mousePosition;
        var ray = HandleUtility.GUIPointToWorldRay(pos);


        if (Physics.Raycast(ray, out var hit, 1000f, roadLayer))
        {
            canPaint = true;
            sceneView.Repaint();
        }
        else
        {
            canPaint = false;
            LevelEditorPaintItemsUI.DisablePreviewItem();
        }
    }

    private static void DrawSelectHandle(SceneView sceneView)
    {
        var defaultMatrix = Handles.matrix;
        var pos = Event.current.mousePosition;
        var ray = HandleUtility.GUIPointToWorldRay(pos);
        var mask = ~roadLayer;

        if (Event.current.control)
            mask = roadLayer;

        if (Physics.Raycast(ray, out var hit, 1000f, mask))
        {
            if (!hit.transform.GetComponentInParent<TransformDataCapture>())
            {
                currentObject = null;
                return;
            }
            //Handles.color = Color.red;
            //var rotationMatrix = Matrix4x4.TRS(hit.transform.position, hit.transform.rotation,
            //    hit.transform.lossyScale);
            //Handles.matrix = rotationMatrix;
            //Handles.DrawWireCube(Vector3.zero, Vector3.one);
            currentObject = hit.transform.GetComponentInParent<TransformDataCapture>().gameObject;
            sceneView.Repaint();
        }
        else
        {
            currentObject = null;
        }
    }

    private static void HandleKeyboardInput()
    {
        var controlID = GUIUtility.GetControlID(FocusType.Keyboard);
        if (Event.current.type == EventType.KeyDown)
        {
            if (SelectedTool == SELECT)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    currentObject = null;
                    Selection.activeGameObject = null;
                }
            }


            if (Event.current.keyCode == KeyCode.Alpha1)
            {
                SelectedTool = NONE;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Alpha2)
            {
                SelectedTool = SELECT;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Alpha3)
            {
                SelectedTool = PAINT;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Alpha4)
            {
                SelectedTool = ERASE;
                Event.current.Use();
            }
        }
        else if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.Alpha1 ||
                Event.current.keyCode == KeyCode.Alpha2 ||
                Event.current.keyCode == KeyCode.Alpha3 ||
                Event.current.keyCode == KeyCode.Alpha4)
                Event.current.Use();
        }

        HandleUtility.AddDefaultControl(controlID);
    }

    private static void HandleMouseInput()
    {
        if (SelectedTool == NONE) return;

        var cId = GUIUtility.GetControlID(FocusType.Passive);
        if (Event.current.type == EventType.MouseDown &&
            Event.current.button == 0)
        {
            if (SelectedTool == SELECT)
            {
                lastObject = currentObject;
                if (currentObject)
                {
                    Selection.activeGameObject = currentObject;
                }
            }
            else if (SelectedTool == ERASE)
            {
                if (currentObject)
                {
                    Undo.RegisterFullObjectHierarchyUndo(currentObject, "Delete level item");
                    DestroyImmediate(currentObject);
                }
            }
        }
        else if (Event.current.type == EventType.MouseDrag &&
            Event.current.button == 0)
        {
        }
        else if (Event.current.type == EventType.MouseUp &&
            Event.current.button == 0)
        {
            if (SelectedTool == PAINT)
            {
                var activeO = LevelEditorWindow.activePaintLevelEditorItem;


                if (activeO != null && canPaint)
                {
                    var pos = Event.current.mousePosition;
                    var ray = HandleUtility.GUIPointToWorldRay(pos);
                    if (Physics.Raycast(ray, out var hit, 1000f, roadLayer))
                    {
                        //Object paint
                        var transformPoint = hit.point;
                        transformPoint.y = 3f;

                        GameObject paintObject = GameObject.Instantiate(LevelEditorWindow.activePaintLevelEditorItem, transformPoint, Quaternion.identity, LevelEditorWindow.levelArea.transform);
                    }
                }
            }
        }

        HandleUtility.AddDefaultControl(cId);
    }

    private static void DrawToolsMenu(Rect position)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(0, position.height - toolbarHeight, position.width, 20), EditorStyles.toolbar);
        SelectedTool =
            GUILayout.SelectionGrid(SelectedTool, buttons, 4, EditorStyles.toolbarButton, GUILayout.Width(300));
        GUILayout.EndArea();

        Handles.EndGUI();
    }


    public static bool IsValid()
    {
        return PrefabStageUtility.GetCurrentPrefabStage() == null && FTFEditorWindow.Instance != null/* &&
               SceneManager.GetActiveScene().name == "LevelEditor"*/;
    }

}