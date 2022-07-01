using DotsNav.Data;
using DotsNav.LocalAvoidance.Data;
using DotsNav.LocalAvoidance.Systems;
using DotsNav.Systems;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(DotsNavSystemGroup))]
[UpdateAfter(typeof(RVOSystem))]
partial class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities
            .WithBurst()
            .WithAny<AllyTag, EnemyTag, ImmortalAllyTag>()
            .ForEach((
                      Entity e,
                      ref Translation translation,
                      ref VelocityComponent velocity,
                      ref Rotation rotation,
                      in IsFollowingData isFollowing
                     ) =>
            {
                if (isFollowing.Value || HasComponent<EnemyTag>(e))
                {
                    translation.Value += velocity.WorldSpace * deltaTime;
                }               
            })
            .ScheduleParallel();
    }
}