
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


    [CreateAssetMenu(fileName = "LevelPrefabList", menuName = "Create LevelPrefabList", order = -1)]
    public class LevelPrefabList : ScriptableObject
    {
        public List<PrefabData> PrefabDatas;

        public void Initialize(Transform parent)
        {
            for (int i = 0; i < PrefabDatas.Count; i++)
            {
                PrefabDatas[i].Initialize(parent);
            }
        }
    }

    [Serializable]
    public class PrefabData
    {
        public int PrefabID;
        public GameObject Prefab;
        private BasicPool<Transform> basicPool;
        public bool IsPoolable = true;
        public int PoolSize;
        public bool ShowOnLevelEditor = true;
        private Transform parent;

        public void Initialize(Transform parent)
        {
            this.parent = parent;
            basicPool = new BasicPool<Transform>(Prefab.transform, parent, IsPoolable ? PoolSize : 0);
        }

        public Transform GetInstance()
        {
            if (IsPoolable && Application.isPlaying)
            {
                return basicPool.GetInstance();
            }
            else
            {
                return GameObject.Instantiate(Prefab.transform, parent);
            }
        }
    }
