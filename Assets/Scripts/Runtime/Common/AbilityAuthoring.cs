using UnityEngine;
using Unity.Entities;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/AbilityAuthoring")]
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject AoeAbility;
        public float AoeAbilityCooldown;

        public NetCodeConfig NetCodeConfig;
        private int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        public class Baker : Baker<AbilityAuthoring> 
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                Entity aoeAbility = GetEntity(authoring.AoeAbility, TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = aoeAbility
                });
                AddComponent(entity, new AbilityCooldownTicks
                {
                    AoeAbility = (uint)(authoring.SimulationTickRate * authoring.AoeAbilityCooldown)
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}