
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using RaycastHit = Unity.Physics.RaycastHit;

namespace TMG.NFE_Tutorial
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class ChampMoveInputSystem : SystemBase
    {
        private MobaInputActions _inputActions;
        private CollisionFilter _selectionFilter;

        protected override void OnCreate()
        {
            _inputActions = new MobaInputActions();
            _selectionFilter = new CollisionFilter
            {
                BelongsTo = 1 << 5, // Raycasts
                CollidesWith = 1 << 0 // Ground Plane
            };
            RequireForUpdate<OwnerChampTag>();
        }

        protected override void OnStartRunning()
        {
            _inputActions.Enable();
            _inputActions.GameplayMap.SelectMovePosition.performed += OnSelectMovePosition;
        }

        protected override void OnStopRunning()
        {
            _inputActions.GameplayMap.SelectMovePosition.performed -= OnSelectMovePosition;
            _inputActions.Disable();
        }

        protected override void OnUpdate()
        {
            
        }

        private void OnSelectMovePosition(InputAction.CallbackContext obj)
        {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            Entity cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            Camera mainCamera = EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 100f;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            RaycastInput selectionInput = new()
            {
                Start = mainCamera.transform.position,
                End = worldPosition,
                Filter = _selectionFilter
            };

            if (collisionWorld.CastRay(selectionInput, out RaycastHit closestHit))
            {
                Entity champEntity = SystemAPI.GetSingletonEntity<OwnerChampTag>();
                EntityManager.SetComponentData(champEntity, new ChampMoveTargetPosition
                {
                    Value = closestHit.Position
                });
            }
        }
    }
}