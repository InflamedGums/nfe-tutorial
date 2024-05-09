using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct ChampMoveSystem : ISystem 
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

            foreach (var (transform, moveTargetPosition, moveSpeed) in 
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<ChampMoveTargetPosition>, RefRO<CharacterMoveSpeed>>()
                         .WithAll<Simulate>())
            {
                float3 moveTarget = moveTargetPosition.ValueRO.Value;
                moveTarget.y = transform.ValueRO.Position.y;

                if (math.distancesq(transform.ValueRO.Position, moveTarget) < 0.001f) continue;
                float3 moveDirection = math.normalize(moveTarget - transform.ValueRO.Position);
                float3 moveVector = deltaTime * moveSpeed.ValueRO.Value * moveDirection;
                
                transform.ValueRW.Position += moveVector;
                transform.ValueRW.Rotation = quaternion.LookRotation(moveDirection, math.up());
            }
        }
    }
}