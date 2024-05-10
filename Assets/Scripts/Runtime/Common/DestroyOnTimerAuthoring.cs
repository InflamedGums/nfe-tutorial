using UnityEngine;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/DestroyOnTimerAuthoring")]
    public class DestroyOnTimerAuthoring : MonoBehaviour
    {
        public float DestroyOnTimer;
        
        public class Baker : Baker<DestroyOnTimerAuthoring> 
        {
            public override void Bake(DestroyOnTimerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyOnTimer {Value = authoring.DestroyOnTimer});
            }
        }
    }
}