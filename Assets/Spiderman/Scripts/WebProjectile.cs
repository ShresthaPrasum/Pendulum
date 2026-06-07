/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class WebProjectile : MonoBehaviour {
    
    public static event EventHandler OnWebReachedMaxDistance;
    public static event EventHandler OnWebHitObject;

    public static WebProjectile Create(Vector3 position, Vector3 direction) {
        Transform webProjectileTransform = Instantiate(GameAssets.instance.pfSpidermanWebProjectile, position, Quaternion.identity);

        WebProjectile webProjectile = webProjectileTransform.GetComponent<WebProjectile>();
        webProjectile.Setup(direction);

        return webProjectile;
    }

    private const float SPEED = 150f;
    private const float DISTANCE_TRAVELLED_MAX = 70f;

    private Vector3 dir;
    private float distanceTravelled;


    private void Awake() {
        Setup(new Vector3(1, 0));
    }

    private void Setup(Vector3 dir) {
        this.dir = dir;
        transform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
    }

    private void Update() {
        transform.position += dir * SPEED * Time.deltaTime;

        distanceTravelled += SPEED * Time.deltaTime;
        if (distanceTravelled > DISTANCE_TRAVELLED_MAX) {
            // Projectile has travelled too much, destroy
            if (OnWebReachedMaxDistance != null) OnWebReachedMaxDistance(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (OnWebHitObject != null) OnWebHitObject(this, EventArgs.Empty);
        Destroy(gameObject);
    }
}

