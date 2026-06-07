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

/*
 * Simple Class for storing Asset References
 * */
public class GameAssets : MonoBehaviour {

    public static GameAssets instance;

    private void Awake() {
        instance = this;
    }

    public Transform pfEnemy;
    public Transform pfEnemyFlyingBody;
    public Transform pfSpidermanWeb;
    public Transform pfSpidermanWebProjectile;
    public Transform pfSpidermanWebFloor;

}
