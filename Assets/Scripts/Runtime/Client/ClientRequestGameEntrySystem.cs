using TMG.NFE_Tutorial.Common;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;

namespace TMG.NFE_Tutorial.Client
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct ClientRequestGameEntrySystem : ISystem
    {
        private EntityQuery _pendingNetworkIdQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<NetworkId>()
                .WithNone<NetworkStreamInGame>();

            _pendingNetworkIdQuery = state.GetEntityQuery(builder);
            state.RequireForUpdate(_pendingNetworkIdQuery);
            state.RequireForUpdate<ClientTeamRequest>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) 
        {
    
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            TeamType requestedTeam = SystemAPI.GetSingleton<ClientTeamRequest>().Value;
            EntityCommandBuffer ecb = new (Allocator.Temp);
            NativeArray<Entity> pendingNetworkIds = _pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity networkId in pendingNetworkIds)
            {
                ecb.AddComponent<NetworkStreamInGame>(networkId);

                Entity teamRequestEntity = ecb.CreateEntity();
                ecb.AddComponent(teamRequestEntity, new MobaTeamRequest { Value = requestedTeam });
                ecb.AddComponent(teamRequestEntity, new SendRpcCommandRequest { TargetConnection = networkId });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}