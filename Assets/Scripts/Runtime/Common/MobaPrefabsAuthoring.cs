using UnityEngine;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/MobaPrefabsAuthoring")]
    public class MobaPrefabsAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _champion;
        
        public class Baker : Baker<MobaPrefabsAuthoring> 
        {
            public override void Bake(MobaPrefabsAuthoring authoring)
            {
                Entity prefabs = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabs, new MobaPrefabs
                {
                    Champion = GetEntity(authoring._champion, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}