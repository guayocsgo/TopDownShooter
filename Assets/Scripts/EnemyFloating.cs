using UnityEngine;

public class EnemyFloating : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stoppingDistance = 1.5f;

    [Header("Flotacion")]
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.4f;
    [SerializeField] private float floatHeight = 2.5f;

    [Header("Daño")]
    [SerializeField] private int damage = 2;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float attackRange = 3f;

    private Transform player;
    private PlayerHealth playerHealth;
    private Vector3 bobOffset;
    private float attackTimer;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (player == null) return;

        MoveTowardsPlayer();
        ApplyBob();
        TryAttack();
    }

    private void MoveTowardsPlayer()
    {
        Vector3 targetPosition = new Vector3(
            player.position.x,
            player.position.y + floatHeight,
            player.position.z
        );

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > stoppingDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0f;
        if (lookDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    private void ApplyBob()
    {
        bobOffset.y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position += bobOffset * Time.deltaTime;
    }

    private void TryAttack()
    {
        attackTimer -= Time.deltaTime;

        
        Vector3 flatPlayerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        float horizontalDistance = Vector3.Distance(transform.position, flatPlayerPos);

        if (horizontalDistance <= attackRange && attackTimer <= 0f)
        {
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            attackTimer = attackCooldown;
        }
    }
}