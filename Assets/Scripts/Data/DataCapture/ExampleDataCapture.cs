using System;
using UnityEngine;

    public class ExampleDataCapture : DataCaptureBase<ExampleData>
    {
        [SerializeField] private int val;

        [HideInInspector] public ExampleData GateData;

        public override ExampleData GetData()
        {
            return new ExampleData(val);
        }

        public override void SetData(ExampleData data)
        {
            val = data.value;

            //GetComponent<ExampleClass>().Initialize(val);
        }
    }

    [Serializable]
    public class ExampleData
    {
        public int value;

        public ExampleData(int eValue)
        {
            value = eValue;
        }
    }
