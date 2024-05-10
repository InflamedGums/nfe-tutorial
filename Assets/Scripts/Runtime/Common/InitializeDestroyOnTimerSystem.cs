using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    public partial struct InitializeDestroyOnTimerSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) 
        {
    
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            float simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
            NetworkTick currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (destroyOnTimer, entity) in 
                     SystemAPI.Query<RefRO<DestroyOnTimer>>().WithNone<DestroyAtTick>().WithEntityAccess())
            {
                uint lifetimeInTicks = (uint)(destroyOnTimer.ValueRO.Value * simulationTickRate);
                NetworkTick targetTick = currentTick;
                targetTick.Add(lifetimeInTicks);
                ecb.AddComponent(entity, new DestroyAtTick {Value = targetTick });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}