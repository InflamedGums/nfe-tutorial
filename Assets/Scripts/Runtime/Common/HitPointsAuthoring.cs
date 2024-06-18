using UnityEngine;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/HitPointsAuthoring")]
    public class HitPointsAuthoring : MonoBehaviour
    {
        public int MaxHitPoints;
        public Vector3 HealthBarOffset;
        
        public class Baker : Baker<HitPointsAuthoring> 
        {
            public override void Bake(HitPointsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MaxHitPoints {Value = authoring.MaxHitPoints});
                AddComponent(entity, new CurrentHitPoints {Value = authoring.MaxHitPoints});
                AddComponent(entity, new HealthBarOffset { Value = authoring.HealthBarOffset });
                AddBuffer<DamageBufferElement>(entity);
                AddBuffer<DamageThisTick>(entity);
            }
        }
    }
}