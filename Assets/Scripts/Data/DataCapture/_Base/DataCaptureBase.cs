using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [RequireComponent(typeof(DataCapturePrefabInfo))]
    public abstract class DataCaptureBase<T> : MonoBehaviour
    {
        public abstract T GetData();
        public abstract void SetData(T data);
    }
