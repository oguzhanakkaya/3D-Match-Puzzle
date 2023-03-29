using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class TransformDataCapture : DataCaptureBase<TransformData>
    {
        public bool CachePosition = true;
        public bool CacheRotation = true;
        public bool CacheScale = true;
        private Vector3 awakePosition;
        private Coroutine getAwakePositionRoutine;
        public float YOffset;

        public override TransformData GetData()
        {
            return new TransformData(CachePosition, CacheRotation, CacheScale,
                CachePosition ? transform.position : Vector3.zero,
                CacheRotation ? transform.rotation : Quaternion.identity,
                CacheScale ? transform.localScale : Vector3.zero,
                transform.position.y - awakePosition.y);
        }

        public override void SetData(TransformData data)
        {
            SetTransformData(data);
        }

        protected void SetTransformData(TransformData data)
        {
            if (data.CachePosition)
            {
                transform.position = data.Position;
            }

            if (data.CacheRotation)
            {
                transform.rotation = data.Rotation;
            }

            if (data.CacheScale)
            {
                //ICustomScale customScale = GetComponent<ICustomScale>();
                //if (customScale != null)
                //{
                //    customScale.SetScale(data.Scale);
                //}
                //else
                //{
                //    transform.localScale = data.Scale;
                //}
                transform.localScale = data.Scale;
            }

            if (getAwakePositionRoutine != null)
            {
                StopCoroutine(getAwakePositionRoutine);
            }

            SetAwakePosition();
            YOffset = data.YOffset;
        }

        private void OnEnable()
        {
            getAwakePositionRoutine = StartCoroutine(GetAwakePositionRoutine());
        }

        private IEnumerator GetAwakePositionRoutine()
        {
            yield return new WaitForSeconds(0.02f);
            SetAwakePosition();
        }

        private void SetAwakePosition()
        {
            awakePosition = transform.position;
        }
    }

    [System.Serializable]
    public class TransformData
    {
        public bool CachePosition;
        public bool CacheRotation;
        public bool CacheScale;

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public float YOffset;

        public TransformData(bool cachePosition, bool cacheRotation, bool cacheScale,
            Vector3 position, Quaternion rotation, Vector3 scale, float yOffset)
        {
            CachePosition = cachePosition;
            CacheRotation = cacheRotation;
            CacheScale = cacheScale;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            YOffset = yOffset;
        }
    }
