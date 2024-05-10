using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginAoeAbilitySystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) 
        {
    
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick) return;

            NetworkTick currentTick = networkTime.ServerTick;

            foreach (AoeAspect aoeAspect in SystemAPI.Query<AoeAspect>().WithAll<Simulate>())
            {
                if (!aoeAspect.ShouldAttack)
                    continue;
                
                Entity newAoeAbility = ecb.Instantiate(aoeAspect.AbilityPrefab);
                LocalTransform abilityTransform = LocalTransform.FromPosition(aoeAspect.AttackPosition);
                ecb.SetComponent(newAoeAbility, abilityTransform);
                ecb.SetComponent(newAoeAbility, aoeAspect.Team);
            }
        }
    }
}