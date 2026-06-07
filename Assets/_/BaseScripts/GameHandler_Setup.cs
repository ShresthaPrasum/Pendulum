using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using CodeMonkey.MonoBehaviours;
using GridPathfindingSystem;

public class GameHandler_Setup : MonoBehaviour {

    public static GridPathfinding gridPathfinding;
    //[SerializeField] private CameraFollow cameraFollow;
    //[SerializeField] private GameObject igetPositionGameObject;
    private IGetPosition igetPosition;

    private void Start() {
        //igetPosition = igetPositionGameObject.GetComponent<IGetPosition>();

        //Sound_Manager.Init();
        //cameraFollow.Setup(GetCameraPosition, () => 60f);// 80f);

        //FunctionPeriodic.Create(SpawnEnemy, 1.5f);
        
        gridPathfinding = new GridPathfinding(new Vector3(-400, -400), new Vector3(400, 400), 5f);
        gridPathfinding.RaycastWalkable();

        //EnemyHandler.Create(new Vector3(0, 0));
    }

    private Vector3 GetCameraPosition() {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 playerToMouseDirection = mousePosition - igetPosition.GetPosition();
        return igetPosition.GetPosition() + playerToMouseDirection * .3f;
    }

    private void SpawnEnemy() {
        Vector3 spawnPosition = igetPosition.GetPosition() + UtilsClass.GetRandomDir() * 100f;
        EnemyHandler.Create(spawnPosition);
    }

    public interface IGetPosition {

        Vector3 GetPosition();

    }

}
