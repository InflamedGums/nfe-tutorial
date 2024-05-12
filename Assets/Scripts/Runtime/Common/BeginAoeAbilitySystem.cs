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
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
            
            if (!networkTime.IsFirstTimeFullyPredictingTick) 
                return;

            NetworkTick currentTick = networkTime.ServerTick;

            foreach (AoeAspect aoeAspect in SystemAPI.Query<AoeAspect>().WithAll<Simulate>())
            {
                bool isOnCooldown = true;
                AbilityCooldownTargetTicks currentTargetTicks = new();

                for (uint i = 0; i < networkTime.SimulationStepBatchSize; ++i)
                {
                    NetworkTick testTick = currentTick;
                    testTick.Subtract(i);

                    if (!aoeAspect.AbilityCooldownTargetTicks.GetDataAtTick(testTick, out currentTargetTicks))
                    {
                        currentTargetTicks.AoeAbility = NetworkTick.Invalid;
                    }

                    if (currentTargetTicks.AoeAbility == NetworkTick.Invalid ||
                        !currentTargetTicks.AoeAbility.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }

                if (isOnCooldown)
                    continue;
                
                if (!aoeAspect.ShouldAttack)
                    continue;
                
                Entity newAoeAbility = ecb.Instantiate(aoeAspect.AbilityPrefab);
                LocalTransform abilityTransform = LocalTransform.FromPosition(aoeAspect.AttackPosition);
                ecb.SetComponent(newAoeAbility, abilityTransform);
                ecb.SetComponent(newAoeAbility, aoeAspect.Team);

                if (state.WorldUnmanaged.IsServer())
                    continue;

                NetworkTick newAoeTargetTick = currentTick;
                newAoeTargetTick.Add(aoeAspect.CooldownTicks);
                currentTargetTicks.AoeAbility = newAoeTargetTick;

                NetworkTick nextTick = currentTick;
                nextTick.Add(1);
                currentTargetTicks.Tick = nextTick;

                aoeAspect.AbilityCooldownTargetTicks.AddCommandData(currentTargetTicks);
            }
        }
    }
}