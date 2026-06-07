/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey.MonoBehaviours;

public class GameHandler : MonoBehaviour {

    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Spiderman spiderman;

    private void Awake() {
        WebFloorParticles.Init();
        cameraFollow.Setup(GetCameraFollowPosition, () => 60f);
    }

    private void Start() {
        //FunctionPeriodic.Create(() => Enemy.Create(spiderman.GetPosition() + UtilsClass.GetRandomDir() * 30f), 2f);
        //Enemy enemy = Enemy.Create(new Vector3(30, 0));
        /*
        FunctionPeriodic.Create(() => {
            WebProjectile.Create(spiderman.GetPosition(), UtilsClass.GetRandomDir());
        }, .05f);
        */
    }

    private Vector3 GetCameraFollowPosition() {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 spidermanToMouseDir = mousePosition - spiderman.GetPosition();
        return spiderman.GetPosition() + spidermanToMouseDir * .3f;
    }

}
