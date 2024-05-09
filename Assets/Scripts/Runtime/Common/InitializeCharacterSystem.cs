using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct InitializeCharacterSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) 
        {
    
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) 
        {
    
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            foreach (var (physicsMass, mobaTeam, entity) in 
                     SystemAPI.Query<RefRW<PhysicsMass>, RefRO<MobaTeam>>().WithAny<NewChampTag>().WithEntityAccess())
            {
                physicsMass.ValueRW.InverseInertia[0] = 0;
                physicsMass.ValueRW.InverseInertia[1] = 0;
                physicsMass.ValueRW.InverseInertia[2] = 0;

                float4 teamColor = mobaTeam.ValueRO.Value switch
                {
                    TeamType.Blue => new float4(0, 0, 1, 1),
                    TeamType.Red => new float4(1, 0, 0, 1),
                    _ => new float4(1)
                };
                
                ecb.SetComponent(entity, new URPMaterialPropertyBaseColor
                {
                    Value = teamColor
                });
                ecb.RemoveComponent<NewChampTag>(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}