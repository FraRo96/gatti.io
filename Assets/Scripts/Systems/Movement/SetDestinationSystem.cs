using DotsNav.PathFinding.Data;
using DotsNav.PathFinding.Systems;
using DotsNav.Systems;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static DotsNav.PathFinding.PathQueryState;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Transforms;

[UpdateInGroup(typeof(DotsNavSystemGroup))]
[UpdateBefore(typeof(PathFinderSystem))]
public partial class SetDestinationSystem : SystemBase
{
    PlayerControls playerControls;
    public float Timer = 5;
    public float3 EnemyDest = new float3(0, 0, 0);

    private float3 _destination;
    private bool _isNewDestination;

    protected override void OnCreate()
    {
        playerControls = new PlayerControls();
        playerControls.Navigation.Enable();
        playerControls.Navigation.PlayerMovement.performed += SetDestination;
    }

    protected override void OnUpdate()
    {
        if (_isNewDestination)
        {
            _isNewDestination = false;

            var dest = _destination;

            Dependency =
                Entities
                    .WithBurst()
                    .WithAny<AllyTag, EnemyTag, ImmortalAllyTag>()
                    .ForEach((
                              ref PathQueryComponent query,
                              in IsSavedData isSaved,
                              in IsFollowingData isFollowing
                             ) =>
                    {
                        if (!isSaved.Value && isFollowing.Value)
                        {
                            query.State = Pending;
                            query.To = dest;
                        }

                    }).ScheduleParallel(Dependency);

        }
    }


    private void SetDestination(InputAction.CallbackContext context) //raycasting destination
    {

        Vector2 to = playerControls.Navigation.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(to);
        //Debug.DrawLine(ray.origin, ray.direction * 1000, Color.red);

        GameManager.main.numScene += 1;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            _isNewDestination = true;
            _destination = new float3(hit.point.x, 0, hit.point.z);
        }

    }
}
