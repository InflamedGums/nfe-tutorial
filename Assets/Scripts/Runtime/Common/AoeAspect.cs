using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace TMG.NFE_Tutorial
{
    public readonly partial struct AoeAspect : IAspect
    {
        private readonly RefRO<AbilityInput> _abilityInput;
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        private readonly RefRO<MobaTeam> _mobaTeam;
        private readonly RefRO<LocalTransform> _transform;
        private readonly RefRO<AbilityCooldownTicks> _abilityCooldownTicks;
        private readonly DynamicBuffer<AbilityCooldownTargetTicks> _abilityCooldownTargetTicks;
        public DynamicBuffer<AbilityCooldownTargetTicks> AbilityCooldownTargetTicks => _abilityCooldownTargetTicks;

        public bool ShouldAttack => _abilityInput.ValueRO.AoeAbility.IsSet;
        public Entity AbilityPrefab => _abilityPrefabs.ValueRO.AoeAbility;
        public MobaTeam Team => _mobaTeam.ValueRO;
        public float3 AttackPosition => _transform.ValueRO.Position;
        public uint CooldownTicks => _abilityCooldownTicks.ValueRO.AoeAbility;
    }
}