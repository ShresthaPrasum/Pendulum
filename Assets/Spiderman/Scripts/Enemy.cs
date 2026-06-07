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

public class Enemy : MonoBehaviour {

    // Spawn an Enemy
    public static Enemy Create(Vector3 spawnPosition) {
        Transform enemyTransform = Instantiate(GameAssets.instance.pfEnemy, spawnPosition, Quaternion.identity);

        Enemy enemy = enemyTransform.GetComponent<Enemy>();

        return enemy;
    }

    // Return closest enemy within maxRange of position
    public static Enemy GetClosestEnemy(Vector3 position, float maxRange) {
        Enemy closest = null;
        foreach (Enemy enemy in enemyList) {
            if (enemy.IsDead()) continue;
            if (Vector3.Distance(position, enemy.GetPosition()) <= maxRange) {
                if (closest == null) {
                    closest = enemy;
                } else {
                    if (Vector3.Distance(position, enemy.GetPosition()) < Vector3.Distance(position, closest.GetPosition())) {
                        closest = enemy;
                    }
                }
            }
        }
        return closest;
    }

    // List of all Enemies
    private static List<Enemy> enemyList = new List<Enemy>();

    private const float SPEED = 30f;

    // Base Enemy class
    private Enemy_Base enemyBase;

    private int health;
    private Spiderman target;
    private State state;

    private enum State {
        Normal,
        Busy,
    }

    private void Awake() {
        enemyList.Add(this);
        enemyBase = gameObject.GetComponent<Enemy_Base>();
        health = 3;
        SetStateNormal();
    }

    private void Start() {
        target = Spiderman.instance;
    }

    private void Update() {
        switch (state) {
        case State.Normal:
            HandleMovement();
            break;
        case State.Busy:
            break;
        }
    }

    private void HandleMovement() {
        float stopMovingDistance = 12f;
        if (Vector3.Distance(GetPosition(), target.GetPosition()) >= stopMovingDistance) {
            // Target is far away, move closer
            Vector3 targetDir = (target.GetPosition() - GetPosition()).normalized;
            transform.position += targetDir * SPEED * Time.deltaTime;
            enemyBase.PlayMoveAnim(targetDir);
        } else {
            // Too close to target, stop
            enemyBase.PlayIdleAnim();
        }
    }
    
    private void SetStateNormal() {
        state = State.Normal;
    }

    private void SetStateBusy() {
        state = State.Busy;
    }

    // Cause Damage to this Enemy
    public void Damage(Vector3 attackerPosition) {
        health -= 1;

        Vector3 dirToAttacker = (attackerPosition - GetPosition()).normalized;

        if (IsDead()) {
            FlyingBody.Create(GameAssets.instance.pfEnemyFlyingBody, GetPosition(), dirToAttacker * -1f);
            Destroy(gameObject);
        } else {
            // Enemy is still alive
            SetStateBusy();
            float knockbackDistance = 5f;
            transform.position += dirToAttacker * -1f * knockbackDistance;
            enemyBase.PlayHitAnimation(dirToAttacker, SetStateNormal);
        }
    }

    public bool IsDead() {
        return health <= 0;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
}
