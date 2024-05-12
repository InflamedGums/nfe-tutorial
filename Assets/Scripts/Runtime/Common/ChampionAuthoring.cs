using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/ChampionAuthoring")]
    public class ChampionAuthoring : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        
        public class Baker : Baker<ChampionAuthoring> 
        {
            public override void Bake(ChampionAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<ChampTag>(entity);
                AddComponent<NewChampTag>(entity);
                AddComponent<MobaTeam>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
                AddComponent<ChampMoveTargetPosition>(entity);
                
                AddComponent(entity, new CharacterMoveSpeed
                {
                    Value = authoring._moveSpeed
                });
                
                AddComponent<AbilityInput>(entity);
                AddComponent<AimInput>(entity);
            }
        }
    }
}