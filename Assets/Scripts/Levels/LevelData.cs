using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HungwX
{
    // [CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
    public class LevelData : MonoBehaviour
    {
        public AssetReferenceGameObject prefabOfLevel;
        public string titleContent;
        public Color levelCameraColor;
        void OnDrawGizmosSelected()
        {
            Camera.main.backgroundColor = levelCameraColor;
        }
    }

    public enum LevelType
    {
        Challenges,
        Coupledancing,
        Loveposes,
        Lovestory,
        Poledancing,
        Regular,
        Seasonal,
        Vip
    }

}