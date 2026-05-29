using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int pointsOnDeath = 100;

    [Header("Parpadeo al recibir daño")]
    [SerializeField] private float blinkInterval = 0.05f;
    [SerializeField] private int blinkCount = 4;

    private NavMeshAgent agent;
    private Transform player;
    private int currentHealth;
    private float attackTimer;
    private bool isBlinking;
    private Renderer[] enemyRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        enemyRenderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) return;

        agent.SetDestination(player.position);
        attackTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && attackTimer <= 0f)
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);

            attackTimer = attackCooldown;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
        else if (!isBlinking)
            StartCoroutine(BlinkOnHit());
    }

    private IEnumerator BlinkOnHit()
    {
        isBlinking = true;

        for (int i = 0; i < blinkCount; i++)
        {
            SetRenderersVisible(false);
            yield return new WaitForSeconds(blinkInterval);
            SetRenderersVisible(true);
            yield return new WaitForSeconds(blinkInterval);
        }

        isBlinking = false;
    }

    private void Die()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(pointsOnDeath);

        Destroy(gameObject);
    }

    private void SetRenderersVisible(bool visible)
    {
        foreach (Renderer r in enemyRenderers)
            r.enabled = visible;
    }
}