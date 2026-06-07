using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    [Tooltip("Check this to constantly leave a trail. Uncheck if you only want it on via code.")]
    public bool isEmitting = false;
    
    [Tooltip("How often a ghost clone is spawned (seconds)")]
    public float spawnInterval = 0.05f;
    
    [Tooltip("How long it takes for a ghost to fade away (seconds)")]
    public float ghostDuration = 0.4f;
    
    [Tooltip("The starting color and opacity of the ghost clones")]
    public Color ghostColor = new Color(1f, 1f, 1f, 0.5f);

    private SpriteRenderer sr;
    private float spawnTimer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isEmitting) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnGhost();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnGhost()
    {
    
        GameObject ghostObj = new GameObject("PlayerGhost");
        ghostObj.transform.position = transform.position;
        ghostObj.transform.rotation = transform.rotation;
        ghostObj.transform.localScale = transform.localScale;

    
        SpriteRenderer ghostSr = ghostObj.AddComponent<SpriteRenderer>();
        ghostSr.sprite = sr.sprite;
        ghostSr.sortingLayerName = sr.sortingLayerName;
        ghostSr.sortingOrder = sr.sortingOrder - 1; 
        ghostSr.color = ghostColor;

     
        GhostFade fader = ghostObj.AddComponent<GhostFade>();
        fader.Setup(ghostDuration);
    }
}

public class GhostFade : MonoBehaviour
{
    private float fadeDuration;
    private SpriteRenderer sr;
    private Color startColor;

    public void Setup(float duration)
    {
        fadeDuration = duration;
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            
            float currentAlpha = Mathf.Lerp(startColor.a, 0f, timer / fadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, currentAlpha);
            
            yield return null;
        }

        Destroy(gameObject);
    }
}