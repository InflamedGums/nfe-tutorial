using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct DamageOnTriggerSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
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

            DamageOnTriggerJob job = new()
            {
                DamageOnTriggerLookup = SystemAPI.GetComponentLookup<DamageOnTrigger>(),
                MobaTeamLookup = SystemAPI.GetComponentLookup<MobaTeam>(),
                AlreadyDamagedLookup = SystemAPI.GetBufferLookup<AlreadyDamagedEntityBuffer>(),
                DamageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>(),
                ECB = ecb
            };
            SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = job.Schedule(simulationSingleton, state.Dependency);
        }

        [BurstCompile]
        public struct DamageOnTriggerJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentLookup<DamageOnTrigger> DamageOnTriggerLookup;
            [ReadOnly] public ComponentLookup<MobaTeam> MobaTeamLookup;
            [ReadOnly] public BufferLookup<AlreadyDamagedEntityBuffer> AlreadyDamagedLookup;
            [ReadOnly] public BufferLookup<DamageBufferElement> DamageBufferLookup;
            public EntityCommandBuffer ECB;
            
            public void Execute(TriggerEvent triggerEvent)
            {
                Entity damageDealingEntity;
                Entity damageReceivingEntity;

                if (DamageBufferLookup.HasBuffer(triggerEvent.EntityA) &&
                    DamageOnTriggerLookup.HasComponent(triggerEvent.EntityB))
                {
                    damageReceivingEntity = triggerEvent.EntityA;
                    damageDealingEntity = triggerEvent.EntityB;
                }
                else if (DamageOnTriggerLookup.HasComponent(triggerEvent.EntityA) &&
                         DamageBufferLookup.HasBuffer(triggerEvent.EntityB))
                {
                    damageDealingEntity = triggerEvent.EntityA;
                    damageReceivingEntity = triggerEvent.EntityB;
                }
                else
                {
                    return;
                }

                DynamicBuffer<AlreadyDamagedEntityBuffer> alreadyDamagedBuffer = 
                    AlreadyDamagedLookup[damageDealingEntity];

                foreach (AlreadyDamagedEntityBuffer entity in alreadyDamagedBuffer)
                {
                    if (entity.Value.Equals(damageReceivingEntity))
                        return;
                }

                if (MobaTeamLookup.TryGetComponent(damageDealingEntity, out MobaTeam dealingTeam) &&
                    MobaTeamLookup.TryGetComponent(damageReceivingEntity, out MobaTeam receivingTeam) &&
                    dealingTeam.Value == receivingTeam.Value)
                {
                    return;
                }

                DamageOnTrigger damageOnTrigger = DamageOnTriggerLookup[damageDealingEntity];
                ECB.AppendToBuffer(damageReceivingEntity, new DamageBufferElement {Value = damageOnTrigger.Value});
                ECB.AppendToBuffer(damageDealingEntity, new AlreadyDamagedEntityBuffer {Value = damageReceivingEntity});
            }
        }
    }
}