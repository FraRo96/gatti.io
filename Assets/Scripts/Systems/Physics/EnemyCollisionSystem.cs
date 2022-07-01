using DotsNav.PathFinding.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

partial class EnemyCollisionSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    private BeginFixedStepSimulationEntityCommandBufferSystem _commandBufferSystem;

    protected override void OnCreate()
    {
        _commandBufferSystem = World.GetOrCreateSystem<BeginFixedStepSimulationEntityCommandBufferSystem>();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        //RequireSingletonForUpdate<PlayerTag>();
    }

    protected override void OnUpdate()
    {
        PhysicsWorld _world = _buildPhysicsWorld.PhysicsWorld;
        EntityCommandBuffer.ParallelWriter _entityCommandBuffer = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entity player = GetSingletonEntity<PlayerTag>();
        Entity immortalAlly = GetSingletonEntity<ImmortalAllyTag>();

        var to = GetComponent<PathQueryComponent>(player).To;

        Dependency = Entities
            .WithBurst()
            .WithAll<EnemyTag>()
            .WithReadOnly(_world)
            .ForEach((
                        int entityInQueryIndex,
                        Entity e,
                        ref LifeData life,
                        ref PathQueryComponent path,
                        in JoinDistanceData joinDist,
                        in LocalToWorld ltw,
                        in IsFollowingData isFollowing
                    ) =>
            {
                CollisionFilter enemyAllyFilter = new CollisionFilter()
                {
                    BelongsTo = 1u << 3,
                    CollidesWith = 1u << 2,
                    GroupIndex = 0
                };

                if (isFollowing.Value)
                {
                    NativeList<DistanceHit> allyHits = new NativeList<DistanceHit>(Allocator.Temp);
                    _world.OverlapSphere(ltw.Position, joinDist.Value, ref allyHits, enemyAllyFilter);

                    Entity ally;
                    for (int i = 0; i < allyHits.Length; i++)
                    {
                        ally = allyHits[i].Entity;
                        if (HasComponent<AllyTag>(ally) && GetComponent<IsFollowingData>(ally).Value
                            && !GetComponent<IsSavedData>(ally).Value
                            //&& !HasComponent<PlayerTag>(ally)
                            )
                        {
                            _entityCommandBuffer.DestroyEntity(entityInQueryIndex, allyHits[i].Entity);
                            //destroyedAllies[0] += 1;
                            life.Value -= 1;
                            if (life.Value <= 0)
                            {
                                _entityCommandBuffer.DestroyEntity(entityInQueryIndex, e);
                            }
                        }
                    }
                }
            })
            .ScheduleParallel(Dependency);
        _commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}