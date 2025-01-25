using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Character character;
    private Transform player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime;

    private void OnEnable()
    {
        character = GetComponent<Character>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        nextAttackTime = 0f;
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null) return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (player == null) return;
        if (player.CompareTag("Player") && player.TryGetComponent<Character>(out Character target))
        {
            var damageInfo = new DamageInfo
            {
                amount = 10,
                type = DamageType.Physical,
                source = character,
                isCritical = false
            };
            target.TakeDamage(damageInfo);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent enemy-enemy damage
        if (!other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other);
        }
    }
}