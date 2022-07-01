/*using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics.Systems;
using Unity.Physics;
using DotsNav.PathFinding.Data;
using DotsNav.Systems;
using DotsNav.PathFinding.Systems;
using static DotsNav.PathFinding.PathQueryState;

[UpdateInGroup(typeof(DotsNavSystemGroup))]
[UpdateBefore(typeof(PathFinderSystem))]

partial class EnemyPatrolSystem : SystemBase
{
    public float Timer = 5;

    protected override void OnUpdate()
    {
        var dest = new float3(0, 0, 0);
        Timer -= Time.DeltaTime;
        //Debug.Log(Timer);
        if(Timer < 0)
        {
            dest = new float3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(-3, 3));
            Timer = 5;

            //Debug.Log(dest);
        }


        Dependency = Entities
       .WithBurst()
       .WithAll<EnemyTag>()
       .ForEach((
                 int entityInQueryIndex,
                 Entity e,
                 ref LocalToWorld ltw,
                 ref PathQueryComponent path,
                 in IsFollowingData isFollowing
                ) =>
       {
           if ( !isFollowing.Value )
           {
               path.State = Pending;
               path.To = new float3(ltw.Position.x + dest.x, 0, ltw.Position.z + dest.z);
               if (dest.x > 0f)
                   Debug.Log(path.To);
           }

       })
        .ScheduleParallel(Dependency);

    }
} */
