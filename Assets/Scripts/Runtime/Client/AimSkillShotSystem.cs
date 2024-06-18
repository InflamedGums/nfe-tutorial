using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace TMG.NFE_Tutorial
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct AimSkillShotSystem : ISystem
    {
        private CollisionFilter _collisionFilter;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainCameraTag>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            
            _collisionFilter = new CollisionFilter
            {
                BelongsTo = 1 << 5, // Raycast,
                CollidesWith = 1 << 0, // GroundPlane
            };
        }

        public void OnDestroy(ref SystemState state) 
        {
    
        }

        public void OnUpdate(ref SystemState state) 
        {
            foreach (var (aimInput, transform) in 
                     SystemAPI.Query<RefRW<AimInput>, RefRO<LocalTransform>>()
                         .WithAll<AimSkillShotTag, OwnerChampTag>())
            {
                CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                Entity cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
                Camera mainCamera = state.EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;

                float3 mousePosition = Input.mousePosition;
                mousePosition.z = 1000f;
                float3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

                RaycastInput selectionInput = new()
                {
                    Start = mainCamera.transform.position,
                    End = worldPosition,
                    Filter = _collisionFilter
                };

                if (collisionWorld.CastRay(selectionInput, out RaycastHit closestHit))
                {
                    float3 directionToTarget = closestHit.Position - transform.ValueRO.Position;
                    directionToTarget.y = transform.ValueRO.Position.y;
                    directionToTarget = math.normalize(directionToTarget);
                    aimInput.ValueRW.Value = directionToTarget;
                }
                else
                {
                    Debug.Log("No Hit!");
                }
            }
        }
    }
}