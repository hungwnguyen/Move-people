using UnityEngine;

namespace HungwX
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class BlendShapeHandle : MonoBehaviour
    {
        private SkinnedMeshRenderer skinnedMeshRenderer;
        private const float maxWeight = 100f;

        void Awake()
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        public void SetBlendShapeWeight(int index, float perCentWeight)
        {
            if (index < 0 || index >= skinnedMeshRenderer.sharedMesh.blendShapeCount)
                return;
            skinnedMeshRenderer.SetBlendShapeWeight(index, maxWeight * perCentWeight);
        }

        public float GetBlendShapeWeight(int index)
        {
            return skinnedMeshRenderer.GetBlendShapeWeight(index) / maxWeight;
        }
    }
}
