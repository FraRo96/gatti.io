using DotsNav.PathFinding.Data;
using DotsNav.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
partial class BarnPlayerCollisionSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    //private EndSimulationEntityCommandBufferSystem _commandBufferSystem;


    protected override void OnCreate()
    {
        //_commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        RequireSingletonForUpdate<PlayerTag>();
    }

    protected override void OnUpdate()
    {
        PhysicsWorld _world = _buildPhysicsWorld.PhysicsWorld;
        //EntityCommandBuffer.ParallelWriter _entityCommandBuffer = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        Entity player = GetSingletonEntity<PlayerTag>();
        var pos = GetComponent<Translation>(player);

        CollisionFilter barnFilter = new CollisionFilter()
        {
            BelongsTo = 1u << 2,
            CollidesWith = 1u << 6,
            GroupIndex = 0
        };

        PointDistanceInput barnPDI = new PointDistanceInput()
        {
            Position = pos.Value,
            MaxDistance = 6, //da parametrizzare
            Filter = barnFilter
        };

        if (_world.CalculateDistance(barnPDI))
        {
            Dependency = Entities
               .WithBurst()
               .WithAll<AllyTag>()
               .WithNone<PlayerTag>()
               .ForEach((
                         ref PathQueryComponent path,
                         ref IsSavedData isSaved,
                         in IsFollowingData isFollowing
                        ) =>
               {
                   if (isFollowing.Value)
                   {
                       isSaved.Value = true;
                       path.To = new Unity.Mathematics.float3(-11, 1f, 13.4f);
                       path.State = DotsNav.PathFinding.PathQueryState.Pending;
                   }
               })
               .ScheduleParallel(Dependency);
        }
    }
}
