using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;
using System.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using DotsNav.Hybrid;

public class Spawner : MonoBehaviour
{
    public float leftX;
    public float rightX;
    public float downZ;
    public float upZ;

    public Material Mat0;
    public Material Mat1;
    public Material Mat2;
    public Material Mat3;
    public Material Mat4;
    public Material Mat5;

    private Material[] _mats = new Material[6];

    public GameObject AllyPrefab;
    public GameObject AllyPrefabGO;

    public DotsNavPlane Plane;

    public int NumAllies = 20;
    public float AllyRadius = 5f;

    private EntityManager _entityManager;
    private BlobAssetStore _blobAssetStore;

    private void Start()
    {
        _mats[0] = Mat0;
        _mats[1] = Mat1;
        _mats[2] = Mat2;
        _mats[3] = Mat3;
        _mats[4] = Mat4;
        _mats[5] = Mat5;

        _blobAssetStore = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        AllyPrefab.GetComponent<DotsNavAgent>().Plane = Plane;
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(AllyPrefab, settings);

        for (int i = 0; i < NumAllies; i++)
        {
            var ally = _entityManager.Instantiate(prefab);
            var allyGO = Instantiate(AllyPrefabGO);
            allyGO.GetComponent<FollowEntity>().EntityToFollow = ally;
            allyGO.transform.GetChild(1).GetComponent<Renderer>().material = _mats[UnityEngine.Random.Range(0, _mats.Length)];
            allyGO.GetComponent<Animator>().SetInteger("Idle", UnityEngine.Random.Range(1,8));
            NoOverlapPositioning(allyGO, AllyRadius);
            _entityManager.SetComponentData<Translation>(ally, new Translation { Value = allyGO.transform.position });
        }
    }

    private void NoOverlapPositioning(GameObject obj, float radius)
    {
        bool positioned = false;
        int nTries = 0;
        int maxTries = 100;
        Collider[] overlaps;
        float3 position;

        while (!positioned && nTries < maxTries)
        {
            position = new float3
                (
                UnityEngine.Random.Range(leftX, rightX),
                obj.transform.localScale.y / 2,
                UnityEngine.Random.Range(downZ, upZ)
                );

            overlaps = Physics.OverlapSphere(new Vector3(position.x, 6, position.z), radius);

            if (overlaps.Length <= 0) // always overlaps with the plane
            {
                obj.transform.position = position;
                positioned = true;
            }

            else
            {
                nTries += 1;
            }
        }

        if (nTries >= maxTries) Debug.Log("max");
    }

    private void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }
}