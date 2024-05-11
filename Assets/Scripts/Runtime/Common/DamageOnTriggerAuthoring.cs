using UnityEngine;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/DamageOnTriggerAuthoring")]
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        public int DamageOnTrigger;
        
        public class Baker : Baker<DamageOnTriggerAuthoring> 
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageOnTrigger {Value = authoring.DamageOnTrigger});
                AddBuffer<AlreadyDamagedEntityBuffer>(entity);
            }
        }
    }
}