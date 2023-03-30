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
        private int recoveryLevelId = 2;

        public const string LEVELS_PATH = "Data/Levels/";

        private LevelArea levelArea;

        public void Awake()
        {
            instance = this;
        }
        public void Initialize()
        {
            _levels = Resources.LoadAll<LevelScriptable>(LEVELS_PATH).ToList();
            _levelPrefabList.Initialize(levelPoolParent);

            levelArea = LevelArea.Instance;

            GameManager.LoadNextLevel += OnLoadNextLevel;
            GameManager.RestartLevel += OnLoadLevel;

            OnLoadLevel();
        }

        public void CreateLevelIDs(string levelIndexes)
        {
           // levelIDs = levelIndexes.Split(',').Select(x => int.Parse(x)).ToArray();
        }

        private int GetLevelIndex()
        {
            int level = DataUtility.Level;
            
            return level;
        }

        public int GetCurrentLevelID()
        {
            int currentLevel = GetLevelIndex();


            if (currentLevel >= _levels.Count+1)
            {
                var lvl = currentLevel % _levels.Count;

            if (lvl==0)
            {
                lvl = _levels.Count;
            }
            return lvl;
            }


            return currentLevel;
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
            Basket.Instance.ClearBasketList();
            LevelArea.Instance.ClearList();
            
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

                if (instance.TryGetComponent<Item>(out Item item))
                {
                    if (levelArea!=null)
                    {
                        levelArea.AddItemToList(item);
                    }
                }
                
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

        public void RecycleItem(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(levelPoolParent.transform);
        }
        public void RecycleAll()
        {
            foreach(Transform child in levelArea.transform)
            {
                child.gameObject.SetActive(false);
                child.transform.SetParent(levelPoolParent.transform);
            }
        }
    }
