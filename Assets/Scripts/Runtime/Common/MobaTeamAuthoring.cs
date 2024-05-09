using UnityEngine;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/MobaTeamAuthoring")]
    public class MobaTeamAuthoring : MonoBehaviour
    {
        public TeamType TeamType;
        
        public class Baker : Baker<MobaTeamAuthoring> 
        {
            public override void Bake(MobaTeamAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MobaTeam {Value = authoring.TeamType});
            }
        }
    }
}