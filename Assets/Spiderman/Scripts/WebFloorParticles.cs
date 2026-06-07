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

public static class WebFloorParticles {

    public static void Init() {
        WebProjectile.OnWebReachedMaxDistance += WebProjectile_OnWebReachedMaxDistance;
        WebProjectile.OnWebHitObject += WebProjectile_OnWebHitObject;
    }

    private static void WebProjectile_OnWebHitObject(object sender, System.EventArgs e) {
        WebProjectile webProjectile = sender as WebProjectile;
        Object.Instantiate(GameAssets.instance.pfSpidermanWebFloor, webProjectile.transform.position, Quaternion.identity);
    }

    private static void WebProjectile_OnWebReachedMaxDistance(object sender, System.EventArgs e) {
        WebProjectile webProjectile = sender as WebProjectile;
        Object.Instantiate(GameAssets.instance.pfSpidermanWebFloor, webProjectile.transform.position, Quaternion.identity);
    }
}
