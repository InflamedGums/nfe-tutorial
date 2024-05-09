using UnityEngine;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/AbilityAuthoring")]
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject AoeAbility;
        
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
            }
        }
    }
}