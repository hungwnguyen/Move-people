using UnityEngine;

namespace HungwX
{
    public interface IObstacle
    {
        string Type { get; }
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
        void Spawn(GameObject obstacle);
        void SetTranform(Transform transform);
    }
}
