using DotsNav.PathFinding.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static DotsNav.PathFinding.PathQueryState;

partial class FollowingSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    private BeginInitializationEntityCommandBufferSystem _commandBufferSystem;  

    protected override void OnCreate()
    {
        _commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        RequireSingletonForUpdate<PlayerTag>();
    }

    protected override void OnUpdate()
    {
        PhysicsWorld _world = _buildPhysicsWorld.PhysicsWorld;
        EntityCommandBuffer.ParallelWriter _entityCommandBuffer = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        Entity player = GetSingletonEntity<PlayerTag>();
        var to = GetComponent<PathQueryComponent>(player).To;
        var pos = GetComponent<Translation>(player).Value;

        var savedAllies = 0;


        Dependency = Entities
           .WithBurst()
           .WithAll<AllyTag>()
           .WithReadOnly(_world)
           .ForEach((
                     Entity e,
                     int entityInQueryIndex,
                     ref LocalToWorld ltw,
                     in JoinDistanceData joinDist,
                     in IsSavedData isSaved
                    ) =>
           {
               if (GetComponent<IsFollowingData>(e).Value && !isSaved.Value)
               {
                   CollisionFilter followingAllyFilter = new CollisionFilter()
                   {
                       BelongsTo = 1u << 2,
                       CollidesWith = 1u << 2, // collides with other allies
                       GroupIndex = 0
                   };

                   CollisionFilter followingEnemyFilter = new CollisionFilter()
                   {
                       BelongsTo = 1u << 2,
                       CollidesWith = 1u << 3,
                       GroupIndex = 0
                   };


                   Entity ally, enemy;

                   NativeList<DistanceHit> allyHits = new NativeList<DistanceHit>(Allocator.Temp);
                   _world.OverlapSphere(ltw.Position, joinDist.Value, ref allyHits, followingAllyFilter);
                   for (int i = 0; i < allyHits.Length; i++)
                   {
                       ally = allyHits[i].Entity;
                       if ( !GetComponent<IsFollowingData>(ally).Value )
                       // you shouldn't put a moving ally in ecb: an enemy in the meantime may destroy the entity in another ecb
                       {
                           
                           _entityCommandBuffer.SetComponent<IsFollowingData>
                               (entityInQueryIndex, ally, new IsFollowingData { Value = true });
                           _entityCommandBuffer.SetComponent<PathQueryComponent>
                               (entityInQueryIndex, ally, new PathQueryComponent { To = to, State = Pending });
                       }

                   }

                   NativeList<DistanceHit> enemyHits = new NativeList<DistanceHit>(Allocator.Temp);
                   _world.OverlapSphere(ltw.Position, 20, ref enemyHits, followingEnemyFilter);
                   for (int i = 0; i < enemyHits.Length; i++)
                   {
                       enemy = enemyHits[i].Entity;
                       if (!GetComponent<IsFollowingData>(enemy).Value)
                       // you shouldn't put a moving ally in ecb: an enemy in the meantime may destroy the entity in another ecb
                       {

                           _entityCommandBuffer.SetComponent<IsFollowingData>
                               (entityInQueryIndex, enemy, new IsFollowingData { Value = true });
                           _entityCommandBuffer.SetComponent<PathQueryComponent>
                               (entityInQueryIndex, enemy, new PathQueryComponent { To = to, State = Pending });
                       }

                   }
               }

               if (isSaved.Value && Vector3.Distance(ltw.Position, new float3(-11, 1f, 13.4f)) < 2)
               {
                   _entityCommandBuffer.DestroyEntity(entityInQueryIndex, e);
                   savedAllies += 1;
               }

           })
           .ScheduleParallel(Dependency);
        _commandBufferSystem.AddJobHandleForProducer(Dependency);

        //GameManager.main.savedAllies += savedAllies;

    }
}
