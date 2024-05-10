using Unity.Entities;
using Unity.Burst;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct DestroyOnTimerSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) 
        {
    
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            NetworkTick currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (destroyAtTick, entity) in SystemAPI.Query<RefRO<DestroyAtTick>>()
                         .WithNone<DestroyEntityTag>().WithEntityAccess())
            {
                NetworkTick tick = destroyAtTick.ValueRO.Value;

                if (currentTick.Equals(tick) || currentTick.IsNewerThan(tick))
                {
                    ecb.AddComponent<DestroyEntityTag>(entity);
                }
            }
        }
    }
}