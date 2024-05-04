using TMG.NFE_Tutorial.Common;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using UnityEngine;

namespace TMG.NFE_Tutorial.Server
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ProcessGameEntryRequestSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<MobaTeamRequest, ReceiveRpcCommandRequest>();
            
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) 
        {
    
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(allocator: Allocator.Temp);
            
            foreach (var (teamRequest, requestSource, requestEntity) in 
                     SystemAPI.Query<RefRO<MobaTeamRequest>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                ecb.DestroyEntity(e: requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(e: requestSource.ValueRO.SourceConnection);

                TeamType requestedTeam = teamRequest.ValueRO.Value;

                if (requestedTeam == TeamType.AutoAssign)
                {
                    requestedTeam = TeamType.Blue;
                }

                int clientId = SystemAPI.GetComponent<NetworkId>(entity: requestSource.ValueRO.SourceConnection).Value;
                
                Debug.Log($"Server is assigning Client-ID: {clientId} to team {requestedTeam.ToString()}");
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}