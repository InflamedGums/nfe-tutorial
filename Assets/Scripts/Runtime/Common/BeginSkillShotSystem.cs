using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginSkillShotSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) 
        {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);

            NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();

            if (!networkTime.IsFirstTimeFullyPredictingTick)
                return;

            NetworkTick currentTick = networkTime.ServerTick;
            bool isServer = state.WorldUnmanaged.IsServer();

            foreach (SkillShotAspect skillShotAspect in 
                     SystemAPI.Query<SkillShotAspect>().WithAll<Simulate>().WithNone<AimSkillShotTag>())
            {
                bool isOnCooldown = true;

                for (uint i = 0; i < networkTime.SimulationStepBatchSize; ++i)
                {
                    NetworkTick testTick = currentTick;
                    testTick.Subtract(i);

                    if (!skillShotAspect.AbilityCooldownTargetTicks
                            .GetDataAtTick(testTick, out AbilityCooldownTargetTicks currentTargetTicks))
                    {
                        currentTargetTicks.SkillShotAbility = NetworkTick.Invalid;
                    }

                    if (currentTargetTicks.SkillShotAbility == NetworkTick.Invalid ||
                        !currentTick.IsNewerThan(testTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }

                if (isOnCooldown)
                    continue;

                if (!skillShotAspect.BeginAttack)
                    continue;
                
                ecb.AddComponent<AimSkillShotTag>(skillShotAspect.ChampionEntity);
            }

            foreach (SkillShotAspect skillShotAspect in 
                     SystemAPI.Query<SkillShotAspect>().WithAll<AimSkillShotTag, Simulate>())
            {
                if (!skillShotAspect.ConfirmAttack)
                    continue;

                Entity skillShotAbility = ecb.Instantiate(skillShotAspect.AbilityPrefab);
                
                LocalTransform spawnTransform = skillShotAspect.SpawnTransform;
                
                ecb.SetComponent(skillShotAbility, spawnTransform);
                ecb.SetComponent(skillShotAbility, skillShotAspect.MobaTeam);
                ecb.RemoveComponent<AimSkillShotTag>(skillShotAspect.ChampionEntity);

                if (isServer) 
                    continue;

                skillShotAspect.AbilityCooldownTargetTicks.GetDataAtTick(currentTick,
                    out AbilityCooldownTargetTicks currentTargetTicks);

                NetworkTick newTargetTick = currentTick;
                newTargetTick.Add(skillShotAspect.CooldownTicks);
                currentTargetTicks.SkillShotAbility = newTargetTick;

                NetworkTick nextTick = currentTick;
                nextTick.Add(1);
                currentTargetTicks.Tick = nextTick;
                
                skillShotAspect.AbilityCooldownTargetTicks.AddCommandData(currentTargetTicks);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}