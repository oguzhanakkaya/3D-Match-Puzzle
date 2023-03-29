using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class BasicPool<T>
           where T : Component
    {
        public T Prefab;
        public List<T> Objects;
        private List<GameObject> GObjects;
        private Transform Parent;

        public T GetInstance()
        {
            T instance = GetAvailableObj();
            if (!instance)
            {
                //#if UNITY_EDITOR
                //                Debug.Log("!There is none available object left in the pool " + Prefab.name + " |  Increasing PoolSize From :" + Objects.Count + " to :" + (Objects.Count + 1));
                //#endif
                AddCapacity(1);
                instance = GetAvailableObj();
            }
            return instance;
        }

        private T GetAvailableObj()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (!GObjects[i].activeSelf)
                {
                    GObjects[i].SetActive(true);
                    return Objects[i];
                }
            }
            return null;
        }

        public void SetCapacity(int capacity)
        {
            if (capacity > Objects.Count)
            {
                AddCapacity(capacity - Objects.Count);
            }
            else
            {
                for (int i = Objects.Count - 1; i >= capacity; i--)
                {
                    T t = Objects[i];
                    if (t == null)
                    {
                        Objects.RemoveAt(i);
                        GObjects.RemoveAt(i);
                    }
                    else if (!t.gameObject.activeSelf)
                    {
                        Objects.Remove(t);
                        GObjects.Remove(t.gameObject);
                        GameObject.Destroy(t.gameObject);
                    }
                }
            }
        }

        public void AddCapacity(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                T t = GameObject.Instantiate(Prefab, Parent);
                Objects.Add(t);
                GObjects.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }
        }

        public void CloseColliders()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Collider[] colliders = Objects[i].GetComponentsInChildren<Collider>();
                for (int j = 0; j < colliders.Length; j++)
                {
                    colliders[j].enabled = false;
                }
            }
        }

        public void CloseAll()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (GObjects[i])
                    GObjects[i].SetActive(false);
            }
        }

        public void SetParentAll()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].transform.parent = Parent;
            }
        }

        public void SetParentAll(Transform parent)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].transform.parent = parent;
            }
        }

        public void CleanAll()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i])
                    GameObject.Destroy(Objects[i].gameObject);
            }
            Objects = new List<T>();
            GObjects = new List<GameObject>();
        }

        public BasicPool(T prefab, Transform parent, int capacity)
        {
            Prefab = prefab;
            Parent = parent;
            Objects = new List<T>();
            GObjects = new List<GameObject>();
            AddCapacity(capacity);
        }
    }
