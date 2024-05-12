using UnityEngine;
using Unity.Entities;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/AbilityAuthoring")]
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject AoeAbility;
        public GameObject SkillShotAbility;
        
        public float AoeAbilityCooldown;
        public float SkillShotAbilityCooldown;
        
        public NetCodeConfig NetCodeConfig;
        private int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        public class Baker : Baker<AbilityAuthoring> 
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                Entity aoeAbility = GetEntity(authoring.AoeAbility, TransformUsageFlags.Dynamic);
                Entity skillShotAbility = GetEntity(authoring.SkillShotAbility, TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = aoeAbility,
                    SkillShotAbility = skillShotAbility
                });
                AddComponent(entity, new AbilityCooldownTicks
                {
                    AoeAbility = (uint)(authoring.SimulationTickRate * authoring.AoeAbilityCooldown),
                    SkillShotAbility = (uint)(authoring.SimulationTickRate * authoring.SkillShotAbilityCooldown)
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}