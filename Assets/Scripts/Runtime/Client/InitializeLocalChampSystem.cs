using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct InitializeLocalChampSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) 
        {
            state.RequireForUpdate<NetworkId>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) 
        {
    
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            
            foreach ((RefRO<LocalTransform> transform, Entity entity) in 
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<GhostOwnerIsLocal>()
                         .WithNone<OwnerChampTag>()
                         .WithEntityAccess())
            {
                ecb.AddComponent<OwnerChampTag>(entity);
                ecb.SetComponent(entity, new ChampMoveTargetPosition {Value = transform.ValueRO.Position});
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}