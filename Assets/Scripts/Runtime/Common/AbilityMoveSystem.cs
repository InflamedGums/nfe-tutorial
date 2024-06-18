using Unity.Entities;
using Unity.Burst;
using Unity.NetCode;
using Unity.Transforms;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct AbilityMoveSystem : ISystem 
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
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, moveSpeed) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<AbilityMoveSpeed>>().WithAll<Simulate>())
            {
                transform.ValueRW.Position += deltaTime * moveSpeed.ValueRO.Value * transform.ValueRW.Forward();
            }
        }
    }
}