using System;

    public static class DataCaptureMain
    {
        public static CapturedData GetObjectData(DataCapturePrefabInfo info)
        {
            TransformDataCapture transformDataCapture = info.GetComponent<TransformDataCapture>();
            ExampleDataCapture exampleDataCapture = info.GetComponent<ExampleDataCapture>();

            return new CapturedData()
            {
                PrefabID = info.PrefabId,
                TransformData = transformDataCapture ? transformDataCapture.GetData() : null,
                ExampleData = exampleDataCapture ? exampleDataCapture.GetData() : null,

            };
        }

        public static void SetObjectData(CapturedData capturedData, DataCapturePrefabInfo info)
        {
            TransformDataCapture transformDataCapture = info.GetComponent<TransformDataCapture>();
            ExampleDataCapture exampleDataCapture = info.GetComponent<ExampleDataCapture>();

            if (transformDataCapture)
            {
                transformDataCapture.SetData(capturedData.TransformData);
            }
            if (exampleDataCapture)
            {
                exampleDataCapture.SetData(capturedData.ExampleData);
            }
        }
    }

    [Serializable]
    public class CapturedData
    {
        public int PrefabID;

        public TransformData TransformData;
        public ExampleData ExampleData;
    }

