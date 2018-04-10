using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Save.Json
{
    [Serializable]
    internal class SaveConfigJson
    {
        public string StageName;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
