using TMG.NFE_Tutorial.Common;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace TMG.NFE_Tutorial
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntryRequestSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MobaPrefabs>();

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
            Entity championPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Champion;
            
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
                Entity newChamp = ecb.Instantiate(championPrefab);
                ecb.SetName(newChamp, "Champion");

                float3 spawnPosition = requestedTeam switch
                {
                    TeamType.Blue => new float3(-50f, 1f, -50f),
                    TeamType.Red => new float3(50f, 1f, 50f),
                    _ => math.up()
                };
                
                LocalTransform newTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(newChamp, newTransform);
                ecb.SetComponent(newChamp, new GhostOwner
                {
                    NetworkId = clientId
                });
                ecb.SetComponent(newChamp, new MobaTeam
                {
                    Value = requestedTeam
                });
                
                ecb.AppendToBuffer(requestSource.ValueRO.SourceConnection, new LinkedEntityGroup
                {
                    Value = newChamp
                });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}