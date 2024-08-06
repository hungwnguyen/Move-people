using System;
using System.Collections.Generic;
using UnityEngine;

namespace HungwX
{
    [Serializable]
    public class Obstacle
    {
        public string Type;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }

    [Serializable]
    public class ObstacleData
    {
        public List<Obstacle> Obstacles;
    }
}
