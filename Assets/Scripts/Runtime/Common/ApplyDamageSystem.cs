using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(CalculateFrameDamageSystem))]
    public partial struct ApplyDamageSystem : ISystem 
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

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NetworkTick currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach (var (currentHitPoints, damageThisTickBuffer, entity) in 
                     SystemAPI.Query<RefRW<CurrentHitPoints>, DynamicBuffer<DamageThisTick>>().WithEntityAccess())
            {
                if (!damageThisTickBuffer.GetDataAtTick(currentTick, out DamageThisTick damageThisTick))
                    continue;

                if (damageThisTick.Tick != currentTick)
                    continue;

                currentHitPoints.ValueRW.Value -= damageThisTick.Value;
                
                if (currentHitPoints.ValueRO.Value <= 0f)
                    ecb.AddComponent<DestroyEntityTag>(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}