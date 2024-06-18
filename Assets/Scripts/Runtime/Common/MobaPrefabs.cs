
using Unity.Entities;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    public struct MobaPrefabs : IComponentData
    {
        public Entity Champion;
    }

    public class UIPrefabs : IComponentData
    {
        public GameObject HealthBar;
    }
}