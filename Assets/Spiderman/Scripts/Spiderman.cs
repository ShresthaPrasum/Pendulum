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

public class Spiderman : MonoBehaviour {

    public static Spiderman instance;

    private const float SPEED = 50f;

    [SerializeField] private Transform pfImpactEffect;
    private Spiderman_Base spidermanBase;
    private State state;
    private Vector3 webZipDir;
    private Vector3 webZipTargetPosition;
    private float webZipSpeed;
    private Transform spidermanWebLeft;
    private Transform spidermanWebRight;

    private enum State {
        Normal,
        Attacking,
        WebZippingStarting,
        WebZipping,
        WebZippingSliding,
        ShootingWebProjectile,
    }

    private void Awake() {
        instance = this;
        spidermanBase = gameObject.GetComponent<Spiderman_Base>();
        SetStateNormal();
    }

    private void Update() {
        switch (state) {
        case State.Normal:
            HandleMovement();
            HandleAttack();
            HandleWebZipStart();
            HandleShootWebProjectile();
            break;
        case State.Attacking:
            HandleAttack();
            break;
        case State.WebZippingStarting:
            break;
        case State.WebZipping:
            HandleWebZipping();
            break;
        case State.WebZippingSliding:
            HandleWebZippingSliding();
            break;
        case State.ShootingWebProjectile:
            HandleShootWebProjectile();
            break;
        }
    }
    
    private void SetStateNormal() {
        state = State.Normal;
    }

    private void SetStateAttacking() {
        state = State.Attacking;
    }

    private void HandleWebZippingSliding() {
        webZipSpeed -= webZipSpeed * Time.deltaTime * 10f;
        transform.position += webZipDir * webZipSpeed * Time.deltaTime;
        if (webZipSpeed <= 10f) {
            SetStateNormal();
        }
    }

    private void HandleWebZipping() {
        transform.position += webZipDir * webZipSpeed * Time.deltaTime;

        // Setup Web Left
        spidermanWebLeft.position = spidermanBase.GetHandLPosition();
        Vector3 webDir = (webZipTargetPosition - spidermanBase.GetHandLPosition()).normalized;
        spidermanWebLeft.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(webDir));
        
        SpriteRenderer spidermanWebSpriteRenderer = spidermanWebLeft.GetComponent<SpriteRenderer>();
        spidermanWebSpriteRenderer.size = new Vector2(
            Vector3.Distance(spidermanBase.GetHandLPosition(), webZipTargetPosition), 
            spidermanWebSpriteRenderer.size.y
        );
        
        // Setup Web Right
        spidermanWebRight.position = spidermanBase.GetHandRPosition();
        webDir = (webZipTargetPosition - spidermanBase.GetHandRPosition()).normalized;
        spidermanWebRight.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(webDir));
        
        spidermanWebSpriteRenderer = spidermanWebRight.GetComponent<SpriteRenderer>();
        spidermanWebSpriteRenderer.size = new Vector2(
            Vector3.Distance(spidermanBase.GetHandRPosition(), webZipTargetPosition), 
            spidermanWebSpriteRenderer.size.y
        );

        if (Vector3.Distance(GetPosition(), webZipTargetPosition) < 25f) {
            spidermanBase.PlaySlidingAnimation(webZipDir);
            Destroy(spidermanWebLeft.gameObject);
            Destroy(spidermanWebRight.gameObject);
            state = State.WebZippingSliding;
        }
    }

    private void HandleWebZipStart() {
        if (Input.GetKey(KeyCode.Space)) {
            webZipTargetPosition = UtilsClass.GetMouseWorldPosition();
            webZipDir = (webZipTargetPosition - GetPosition()).normalized;

            spidermanBase.PlayWebZipShootAnimation(webZipDir);

            spidermanWebLeft = Instantiate(GameAssets.instance.pfSpidermanWeb, spidermanBase.GetHandLPosition(), Quaternion.identity);
            Vector3 webDir = (webZipTargetPosition - spidermanBase.GetHandLPosition()).normalized;
            spidermanWebLeft.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(webDir));

            SpriteRenderer spidermanWebSpriteRenderer = spidermanWebLeft.GetComponent<SpriteRenderer>();
            Vector2 webZipStart = new Vector2(0, spidermanWebSpriteRenderer.size.y);
            Vector2 webZipEnd = new Vector2(Vector3.Distance(spidermanBase.GetHandLPosition(), webZipTargetPosition), spidermanWebSpriteRenderer.size.y);
            spidermanWebSpriteRenderer.size = webZipStart;
            float timeToReachTarget = 0f;
            FunctionUpdater.Create(() => {
                timeToReachTarget += Time.deltaTime * 8f;
                spidermanWebSpriteRenderer.size = Vector2.Lerp(webZipStart, webZipEnd, timeToReachTarget);
                if (timeToReachTarget >= 1f) {
                    // Start flying
                    webZipSpeed = 250f;
                    spidermanBase.PlayWebZipFlyingAnimation(webZipDir);
                    state = State.WebZipping;
                    return true;
                } else {
                    return false;
                }
            });

            
            spidermanWebRight = Instantiate(GameAssets.instance.pfSpidermanWeb, spidermanBase.GetHandRPosition(), Quaternion.identity);
            webDir = (webZipTargetPosition - spidermanBase.GetHandRPosition()).normalized;
            spidermanWebRight.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(webDir));

            SpriteRenderer spidermanWebSpriteRendererRight = spidermanWebRight.GetComponent<SpriteRenderer>();
            Vector2 webZipStartRight = new Vector2(0, spidermanWebSpriteRendererRight.size.y);
            Vector2 webZipEndRight = new Vector2(Vector3.Distance(spidermanBase.GetHandRPosition(), webZipTargetPosition), spidermanWebSpriteRendererRight.size.y);
            spidermanWebSpriteRendererRight.size = webZipStartRight;
            float timeToReachTargetRight = 0f;
            FunctionUpdater.Create(() => {
                timeToReachTargetRight += Time.deltaTime * 8f;
                spidermanWebSpriteRendererRight.size = Vector2.Lerp(webZipStartRight, webZipEndRight, timeToReachTargetRight);
                if (timeToReachTargetRight >= 1f) {
                    return true;
                } else {
                    return false;
                }
            });

            state = State.WebZippingStarting;
        }
    }

    private void HandleMovement() {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveX = +1f;
        }

        Vector3 moveDir = new Vector3(moveX, moveY).normalized;
        bool isIdle = moveX == 0 && moveY == 0;
        if (isIdle) {
            spidermanBase.PlayIdleAnim();
        } else {
            spidermanBase.PlayMoveAnim(moveDir);
            transform.position += moveDir * SPEED * Time.deltaTime;
        }
    }

    private void HandleAttack() {
        if (Input.GetMouseButtonDown(0)) {
            SetStateAttacking();

            Vector3 dirToMouse = (UtilsClass.GetMouseWorldPosition() - GetPosition()).normalized;
            float attackPositionDistance = 12f;
            Vector3 attackPosition = GetPosition() + dirToMouse * attackPositionDistance;
            
            Vector3 attackDir;
            bool hitEnemy;

            // Find closest enemy to attack position
            float attackRange = 15f;
            Enemy enemy = Enemy.GetClosestEnemy(attackPosition, attackRange);
            if (enemy != null) {
                // There is an enemy in range, attack him
                hitEnemy = true;
                attackPosition = enemy.GetPosition();
                attackDir = (attackPosition - transform.position).normalized;
                enemy.Damage(GetPosition());

                if (enemy.IsDead()) {
                    float dashForwardAmount = 5f;
                    transform.position += attackDir * dashForwardAmount;
                    UtilsClass.ShakeCamera(1.5f, .1f);
                } else {
                    float distanceToEnemy = 10f;
                    transform.position = enemy.GetPosition() + (attackDir * -1f) * distanceToEnemy;
                    UtilsClass.ShakeCamera(.5f, .05f);
                }
            } else {
                // No enemy in range
                hitEnemy = false;
                attackDir = (attackPosition - transform.position).normalized;
                float dashForwardAmount = 5f;
                transform.position += attackDir * dashForwardAmount;
            }


            // Play attack animation
            if (spidermanBase.IsPlayingPunchAnimation()) {
                // Play Kick animation since punch animation is currently active
                spidermanBase.PlayKickAnimation(attackDir, (Vector3 impactPosition) => {
                    if (hitEnemy) {
                        Transform impactEffect = Instantiate(pfImpactEffect, impactPosition, Quaternion.identity);
                        impactEffect.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(attackDir));
                    }
                }, SetStateNormal);
            } else {
                // Play Punch animation
                spidermanBase.PlayPunchAnimation(attackDir, (Vector3 impactPosition) => {
                    if (hitEnemy) {
                        Transform impactEffect = Instantiate(pfImpactEffect, impactPosition, Quaternion.identity);
                        impactEffect.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(attackDir));
                    }
                }, SetStateNormal);
            }
        }
    }

    private void HandleShootWebProjectile() {
        if (Input.GetKeyDown(KeyCode.F)) {
            Vector3 projectileDir = (UtilsClass.GetMouseWorldPosition() - GetPosition()).normalized;
            Vector3 spawnProjectilePosition = GetPosition() + projectileDir * Random.Range(1f, 10f);
            WebProjectile.Create(spawnProjectilePosition, projectileDir);
            state = State.ShootingWebProjectile;
            spidermanBase.PlayShootWebAnimation(projectileDir, () => {
                state = State.Normal;
            });
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
}
