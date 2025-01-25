using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float respawnRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float nextAttackTime;
    private Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override void Update()
    {
        base.Update();
        if (playerTransform != null)
        {
            UpdateBehavior();
        }
    }

    private void UpdateBehavior()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        movement = distanceToPlayer > attackRange ? directionToPlayer : Vector2.zero;

        if (spriteRenderer != null && directionToPlayer.x != 0)
        {
            spriteRenderer.flipX = directionToPlayer.x < 0;
        }

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (playerTransform.TryGetComponent<Character>(out var playerCharacter))
        {
            var damageInfo = new DamageInfo
            {
                amount = attackDamage,
                type = DamageType.Physical,
                source = this,
                isCritical = Random.value < currentStats.criticalChance
            };
            playerCharacter.TakeDamage(damageInfo);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    protected override void OnDamageTaken(DamageInfo damageInfo)
    {
        // Flash red or play hurt animation
    }

    protected override int GetSpecialAbilityCost() => 0;

    protected override void UseSpecialAbility()
    {
        // Enemies could have special abilities 
    }

    public void SetDifficultyMultiplier(float multiplier)
    {
        currentStats.maxHealth = Mathf.RoundToInt(baseStats.maxHealth * multiplier);
        attackDamage = Mathf.RoundToInt(baseStats.attackDamage * multiplier);
    }

    protected override void CheckDeath()
    {
        if (Health <= 0)
        {
            if (playerTransform != null)
            {
                Health = currentStats.maxHealth;
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                transform.position = (Vector2)playerTransform.position + randomDirection * respawnRange;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}