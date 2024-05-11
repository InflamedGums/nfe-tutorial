using Unity.Entities;
using Unity.Burst;
using Unity.NetCode;
// ReSharper disable Unity.Entities.MustBeSurroundedWithRefRwRo

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct CalculateFrameDamageSystem : ISystem 
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

            foreach (var (damageBuffer, damageThisTickBuffer) in 
                     SystemAPI.Query<DynamicBuffer<DamageBufferElement>, DynamicBuffer<DamageThisTick>>()
                         .WithAll<Simulate>())
            {
                if (damageBuffer.IsEmpty)
                {
                    damageThisTickBuffer.AddCommandData(new DamageThisTick {Tick = currentTick, Value = 0});
                }
                else
                {
                    int totalDamage = 0;

                    if (damageThisTickBuffer.GetDataAtTick(currentTick, out DamageThisTick damageThisTick))
                    {
                        totalDamage = damageThisTick.Value;
                    }

                    foreach (DamageBufferElement element in damageBuffer)
                    {
                        totalDamage += element.Value;
                    }
                    
                    damageThisTickBuffer.AddCommandData(new DamageThisTick {Tick = currentTick, Value = totalDamage});
                    damageBuffer.Clear();
                }
            }
        }
    }
}