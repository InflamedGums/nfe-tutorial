using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace TMG.NFE_Tutorial
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarSystem : ISystem 
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<UIPrefabs>();
        }

        public void OnDestroy(ref SystemState state) 
        {
    
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // spawn health bars for entities that require them
            foreach (var (transform, healthBarOffset, maxHitPoints, entity) in 
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<HealthBarOffset>, RefRO<MaxHitPoints>>()
                         .WithNone<HealthBarUIReference>()
                         .WithEntityAccess())
            {
                GameObject healthBar = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().HealthBar;
                Vector3 spawnPosition = transform.ValueRO.Position + healthBarOffset.ValueRO.Value;
                GameObject newHealthBar = Object.Instantiate(healthBar, spawnPosition, Quaternion.identity);
                SetHealthBar(newHealthBar, maxHitPoints.ValueRO.Value, maxHitPoints.ValueRO.Value);
                ecb.AddComponent(entity, new HealthBarUIReference {Value = newHealthBar});
            }

            // Update position and values of healthbars
            foreach (var (transform, 
                         healthBarOffset, 
                         currentHitPoints, 
                         maxHitPoints, 
                         healthBarUIReference) in 
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<HealthBarOffset>, RefRO<CurrentHitPoints>, 
                         RefRO<MaxHitPoints>, HealthBarUIReference>())
            {
                float3 healthBarPosition = transform.ValueRO.Position + healthBarOffset.ValueRO.Value;
                healthBarUIReference.Value.transform.position = healthBarPosition;
                SetHealthBar(healthBarUIReference.Value, currentHitPoints.ValueRO.Value, maxHitPoints.ValueRO.Value);
            }
            
            // Cleanup health bar
            foreach (var (healthBarUI, entity) in SystemAPI.Query<HealthBarUIReference>()
                         .WithNone<LocalTransform>().WithEntityAccess())
            {
                Object.Destroy(healthBarUI.Value);
                ecb.RemoveComponent<HealthBarUIReference>(entity);
            }
        }

        private void SetHealthBar(GameObject healthBarCanvasObject, int currentHitPoints, int maxHitPoints)
        {
            var healthBarSlider = healthBarCanvasObject.GetComponentInChildren<Slider>();
            healthBarSlider.minValue = 0;
            healthBarSlider.maxValue = maxHitPoints;
            healthBarSlider.value = currentHitPoints;
        }
    }
}