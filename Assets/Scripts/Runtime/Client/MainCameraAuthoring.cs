using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace TMG.NFE_Tutorial
{
    public class MainCameraAuthoring : MonoBehaviour
    {
        public class Baker : Baker<MainCameraAuthoring>
        {
            public override void Bake(MainCameraAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new MainCamera());
                AddComponent<MainCameraTag>(entity);
            }
        }
    }
}