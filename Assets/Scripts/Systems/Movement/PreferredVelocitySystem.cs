using DotsNav.Data;
using DotsNav.LocalAvoidance.Data;
using DotsNav.LocalAvoidance.Systems;
using DotsNav.PathFinding.Data;
using DotsNav.PathFinding.Systems;
using DotsNav.Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(DotsNavSystemGroup))]
[UpdateAfter(typeof(PathFinderSystem))]
[UpdateBefore(typeof(RVOSystem))]
partial class PreferredVelocitySystem : SystemBase
{

    protected override void OnUpdate()
    {
        Entities
            .WithBurst()
            .WithAny<AllyTag, ImmortalAllyTag>()
            .ForEach((
                      ref Translation translation,
                      ref PreferredVelocityComponent preferredVelocity,
                      in DirectionComponent direction,
                      in SpeedData steering,
                      in PathQueryComponent query,
                      in IsFollowingData isFollowing
                     ) =>
            {
                if (isFollowing.Value)
                {
                    var dist = math.length(query.To - translation.Value);
                    var speed = math.min(dist * steering.BrakeSpeed, steering.PreferredSpeed);
                    preferredVelocity.Value = direction.Value * speed;
                }

            })
            .ScheduleParallel();
    }
}