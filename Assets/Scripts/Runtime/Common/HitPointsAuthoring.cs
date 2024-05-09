using UnityEngine;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/HitPointsAuthoring")]
    public class HitPointsAuthoring : MonoBehaviour
    {
        public int MaxHitPoints;
        
        public class Baker : Baker<HitPointsAuthoring> 
        {
            public override void Bake(HitPointsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MaxHitPoints {Value = authoring.MaxHitPoints});
                AddComponent(entity, new CurrentHitPoints {Value = authoring.MaxHitPoints});
                AddBuffer<DamageBufferElement>(entity);
                AddBuffer<DamageThisTick>(entity);
            }
        }
    }
}