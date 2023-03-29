using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


    public class LevelManager : MonoBehaviour
    {
        private List<LevelScriptable> _levels;
        [SerializeField] private LevelPrefabList _levelPrefabList;
        [Space(10f)] public Transform _levelParent, levelPoolParent;

        public static LevelManager instance;

        private List<GameObject> _currentObjects = new List<GameObject>();
        private int[] levelIDs;
        private int recoveryLevelId = 2;

        public const string LEVELS_PATH = "Data/Levels/";

        public void Initialize()
        {
            instance = this;
            _levels = Resources.LoadAll<LevelScriptable>(LEVELS_PATH).ToList();

            OnLoadLevel();
        }

        public void CreateLevelIDs(string levelIndexes)
        {
            levelIDs = levelIndexes.Split(',').Select(x => int.Parse(x)).ToArray();
        }

        private int GetLevelIndex()
        {
            int level = DataUtility.Level - 1;

        

            return level;
        }

        public int GetCurrentLevelID()
        {
            int currentLevel = GetLevelIndex();

            if (currentLevel >= levelIDs.Length || currentLevel < 0)
                return DataUtility.Level - 1;

            int levelID = levelIDs[currentLevel];

            if (!_levels.Exists(x => x.LevelID == levelID))
                levelID = currentLevel;

            return levelID;
        }

        public LevelScriptable GetLevel()
        {
            return GetLevel(GetCurrentLevelID());
        }

        public LevelScriptable GetLevel(int levelID)
        {
            LevelScriptable level = _levels.FirstOrDefault(lstLevel => lstLevel.LevelID == levelID);

            if (level == null)
            {
                level = _levels.FirstOrDefault(lstLevel => lstLevel.LevelID == recoveryLevelId);
                Debug.Log("Level created with recover id.");
            }

            return level;
        }

        public void OnLoadLevel()
        {

            LevelScriptable levelScriptable = GetLevel();

            if (levelScriptable == null) return;

             InitializeLevel(levelScriptable);
        }

        private void InitializeLevel(LevelScriptable levelScriptable)
        {
            ClearLevel();

            for (int i = 0; i < levelScriptable.CapturedDatas.Count; i++)
            {
                GameObject instance = GetPrefabInstanceWithID(levelScriptable.CapturedDatas[i].PrefabID).gameObject;
                instance.transform.SetParent(_levelParent);
                _currentObjects.Add(instance);

                DataCaptureMain.SetObjectData(levelScriptable.CapturedDatas[i], instance.GetComponent<DataCapturePrefabInfo>());
                instance.SetActive(true);

            }
        }

        private void ClearLevel()
        {
            if (_currentObjects == null)
                return;

            for (int i = 0; i < _currentObjects.Count; i++)
            {
                if (_currentObjects[i] == null)
                    continue;

                _currentObjects[i].transform.SetParent(levelPoolParent);
                _currentObjects[i].SetActive(false);
            }

            _currentObjects.Clear();
        }

        public void LoadLevel(int levelId)
        {
            LevelScriptable levelScriptable = _levels.FirstOrDefault(level => level.LevelID == levelId);

            if (levelScriptable == null) return;

            InitializeLevel(levelScriptable);
          
        }

        public void LoadLevel(LevelScriptable levelScriptable)
        {
            if (levelScriptable == null) return;

            InitializeLevel(levelScriptable);
     
        }

        private void OnLoadNextLevel()
        {
            DataUtility.Level += 1;
            OnLoadLevel();
        }

        public Transform GetPrefabInstanceWithID(int prefabID)
        {
            PrefabData data = _levelPrefabList.PrefabDatas.FirstOrDefault(x => x.PrefabID == prefabID);
            if (data != null)
            {
                return data.GetInstance();
            }
            else
            {
                Debug.LogError("Pool Doesnt Include the Prefab ID : " + prefabID);
                return null;
            }
        }

        public int GetLevelId()
        {
            return DataUtility.Level;
        }
    }
