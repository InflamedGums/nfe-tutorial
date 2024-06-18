using UnityEngine;
using Unity.Entities;

namespace TMG.NFE_Tutorial
{
    [AddComponentMenu("TMG/NFE_Tutorial/MobaPrefabsAuthoring")]
    public class MobaPrefabsAuthoring : MonoBehaviour
    {
        [Header("Entities")]
        [SerializeField] private GameObject _champion;
        
        [Header("Game Objects")]
        [SerializeField] private GameObject _healthBar;        
        
        public class Baker : Baker<MobaPrefabsAuthoring> 
        {
            public override void Bake(MobaPrefabsAuthoring authoring)
            {
                Entity prefabs = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabs, new MobaPrefabs
                {
                    Champion = GetEntity(authoring._champion, TransformUsageFlags.Dynamic)
                });
                AddComponentObject(prefabs, new UIPrefabs
                {
                    HealthBar = authoring._healthBar
                });
            }
        }
    }
}