using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitySystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) 
        {
    
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NetworkTime networkTick = SystemAPI.GetSingleton<NetworkTime>();

            if (!networkTick.IsFirstTimeFullyPredictingTick)
                return;

            NetworkTick currentTick = networkTick.ServerTick;

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<DestroyEntityTag, Simulate>().WithEntityAccess())
            {
                if (state.World.IsServer())
                {
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    transform.ValueRW.Position = new float3(1000f);
                }
            }
        }
    }
}