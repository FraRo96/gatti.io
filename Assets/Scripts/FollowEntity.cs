using DotsNav.Data;
using DotsNav.PathFinding.Data;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowEntity : MonoBehaviour
{
    public Entity EntityToFollow;
    public GameObject Followed;
    public float3 offset;

    private EntityManager _entityManager;

    private Animator _animator;

    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if (_entityManager.Exists(EntityToFollow))
        {
            if (_entityManager.GetComponentData<IsFollowingData>(EntityToFollow).Value)
            {
                _animator.SetBool("isRunning", true);
                Translation entityPosition = _entityManager.GetComponentData<Translation>(EntityToFollow);
                var to = _entityManager.GetComponentData<PathQueryComponent>(EntityToFollow).To;
                transform.position = entityPosition.Value + offset;
                var entityDir = _entityManager.GetComponentData<DirectionComponent>(EntityToFollow);
                var direction = to - entityPosition.Value;
                //var dir = new Vector3(entityDir.Value.x, 0, entityDir.Value.y);
                //var rot = Quaternion.LookRotation(dir, Vector3.up);
                var rot = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.05f);
            }

        }

        else
        {
            if (gameObject.tag == "Player")
            {
                GameManager.main.isPlayerDied = true;
            }
            Destroy(gameObject); // explosion animation
        }
    }
}
