using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.NFE_Tutorial
{
    public readonly partial struct SkillShotAspect : IAspect
    {
        public readonly Entity ChampionEntity;

        private readonly RefRO<AbilityInput> _abilityInput;
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        private readonly RefRO<AbilityCooldownTicks> _abilityCooldownTicks;
        private readonly RefRO<MobaTeam> _mobaTeam;
        private readonly RefRO<LocalTransform> _localTransform;
        private readonly DynamicBuffer<AbilityCooldownTargetTicks> _abilityCooldownTargetTicks;
        private readonly RefRO<AimInput> _aimInput;

        public bool BeginAttack => _abilityInput.ValueRO.SkillShotAbility.IsSet;
        public bool ConfirmAttack => _abilityInput.ValueRO.ConfirmSkillShotAbility.IsSet;
        public Entity AbilityPrefab => _abilityPrefabs.ValueRO.SkillShotAbility;
        public MobaTeam MobaTeam => _mobaTeam.ValueRO;
        
        public DynamicBuffer<AbilityCooldownTargetTicks> AbilityCooldownTargetTicks 
            => _abilityCooldownTargetTicks;

        public uint CooldownTicks => _abilityCooldownTicks.ValueRO.SkillShotAbility;
        public float3 AttackPosition => _localTransform.ValueRO.Position;
        
        public LocalTransform SpawnTransform => LocalTransform.FromPositionRotation(AttackPosition,
            quaternion.LookRotationSafe(_aimInput.ValueRO.Value, math.up()));
    }
}