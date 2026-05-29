using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHearts = 3;
    [SerializeField] private float invincibilityTime = 1.5f;

    [Header("UI Corazones")]
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartEmpty;
    [SerializeField] private Transform heartsContainer;

    [Header("Parpadeo")]
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Muerte")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string deathAnimationTrigger = "Die";
    [SerializeField] private float deathDelay = 2f;

    private int currentHearts;
    private bool isInvincible;
    private List<Image> heartImages = new List<Image>();
    private Renderer[] playerRenderers;

    private void Awake()
    {
        currentHearts = maxHearts;
        playerRenderers = GetComponentsInChildren<Renderer>();

        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        BuildHeartsUI();
    }

    private void BuildHeartsUI()
    {
        foreach (Transform child in heartsContainer)
            Destroy(child.gameObject);

        heartImages.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            GameObject go = new GameObject("Heart_" + i);
            go.transform.SetParent(heartsContainer, false);

            Image img = go.AddComponent<Image>();
            img.sprite = heartFull;
            img.preserveAspect = true;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);

            heartImages.Add(img);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHearts -= amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);

        UpdateHeartsUI();

        if (currentHearts <= 0)
        {
            StartCoroutine(DieSequence());
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Count; i++)
            heartImages[i].sprite = i < currentHearts ? heartFull : heartEmpty;
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibilityTime)
        {
            visible = !visible;
            SetRenderersVisible(visible);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        SetRenderersVisible(true);
        isInvincible = false;
    }

    private IEnumerator DieSequence()
    {
        isInvincible = true;

        if (playerAnimator != null)
            playerAnimator.SetTrigger(deathAnimationTrigger);

        
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        yield return new WaitForSeconds(deathDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetRenderersVisible(bool visible)
    {
        foreach (Renderer r in playerRenderers)
            r.enabled = visible;
    }
}