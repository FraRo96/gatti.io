using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    public GameObject Camera;
    public GameObject Player;
    public GameObject FollowCamera;
    public Vector3 dir;
    public Vector3 CameraOffset;

    public int numScene;

    public int savedAllies;
    public int destroyedAllies;
    public int powerUpPoints;

    public bool isPlayerDied;

    private EntityManager _entityManager;
    private void Awake()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        Player = GameObject.FindGameObjectWithTag("Player");
        FollowCamera = GameObject.FindGameObjectWithTag("FollowCamera");
        CameraOffset = Camera.GetComponent<CameraOffset>().offset;
        main = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void LateUpdate()
    {

        //Camera.transform.LookAt(FollowCamera.transform.position);
        //Camera.transform.position = FollowCamera.transform.position - FollowCamera.transform.forward * 10f;
        //Camera.transform.LookAt(FollowCamera.transform.position);
        Camera.transform.position = FollowCamera.transform.position + CameraOffset;
        Camera.transform.LookAt(FollowCamera.transform);

        var playerFound = true;

        if (isPlayerDied)
        {
            playerFound = false;
            var allies = GameObject.FindGameObjectsWithTag("Ally");
            for (int i = 0; !playerFound && i < allies.Length; i++)
            {
                var entity = allies[i].GetComponent<FollowEntity>().EntityToFollow;
                if (_entityManager.GetComponentData<IsFollowingData>(entity).Value)
                {
                    allies[i].tag = "Player";
                    _entityManager.AddComponentData<PlayerTag>(entity, new PlayerTag());
                    playerFound = true;
                }
            }
            if ( !playerFound )
            {
                Debug.Log("You Lose");
                //ChangeScene(attuale, prossima);
            }
        }
    }
}


/*
    _entityManager.DestroyEntity(_entityManager.UniversalQuery);
    SceneManager.LoadScene("menu");
    numScene = 0;
*/