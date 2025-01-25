using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    [System.Serializable]
    public struct CharacterStats
    {
        public string characterName;
        public int maxHealth;
        public int maxMana;
        public float moveSpeed;
        public float attackSpeed;
        public int armor;
        public int magicResistance;
        public float criticalChance;
        public int attackDamage;
    }

    [SerializeField] protected CharacterStats baseStats;
    protected CharacterStats currentStats;
    protected Rigidbody2D rb;
    protected Vector2 movement;
    protected bool isStunned;
    protected float stunDuration;
    protected List<StatusEffect> activeEffects = new List<StatusEffect>();
    protected SpriteRenderer spriteRenderer;

    [SerializeField] protected float minX = -10f;
    [SerializeField] protected float maxX = 10f;
    [SerializeField] protected float minY = -10f;
    [SerializeField] protected float maxY = 10f;

    public int Health { get; protected set; }
    public int Mana { get; protected set; }
    public bool IsDead => Health <= 0;

    protected virtual void Awake()
    {
        currentStats = baseStats;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Health = currentStats.maxHealth;
        Mana = currentStats.maxMana;

    }

    protected virtual void Update()
    {
        UpdateStatusEffects();
        if (isStunned)
        {
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0) isStunned = false;
            return;
        }
        HandleMovement();
        HandleAbilities();
    }

    protected virtual void HandleMovement()
    {
        if (CompareTag("Player"))
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement = movement.normalized;
        }

        Vector2 newPosition = rb.position + movement * currentStats.moveSpeed * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        rb.MovePosition(newPosition);

        if (movement.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = movement.x < 0;
        }
    }

    protected virtual void HandleAbilities()
    {
        if (CompareTag("Player") && Input.GetKeyDown(KeyCode.Space))
        {
            if (HasEnoughMana(GetSpecialAbilityCost()))
            {
                UseSpecialAbility();
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!isStunned)
        {
            
        }
    }

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        float totalDamage = CalculateDamage(damageInfo);
        Health -= Mathf.RoundToInt(totalDamage);
        OnDamageTaken(damageInfo);
        CheckDeath();
    }

    protected virtual float CalculateDamage(DamageInfo damageInfo)
    {
        float damage = damageInfo.amount;
        switch (damageInfo.type)
        {
            case DamageType.Physical:
                damage *= (100f / (100f + currentStats.armor));
                break;
            case DamageType.Magical:
                damage *= (100f / (100f + currentStats.magicResistance));
                break;
        }
        return damage;
    }

    protected virtual void CheckDeath()
    {
        if (Health <= 0)
        {
            if (CompareTag("Enemy"))
            {
                Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (player != null)
                {
                    Health = currentStats.maxHealth;
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    transform.position = (Vector2)player.position + randomDirection * 10f;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        activeEffects.Add(effect);
        effect.OnApply(this);
    }

    protected virtual void UpdateStatusEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].Update(Time.deltaTime);
            if (activeEffects[i].IsExpired)
            {
                activeEffects[i].OnRemove(this);
                activeEffects.RemoveAt(i);
            }
        }
    }

    protected bool HasEnoughMana(int cost) => Mana >= cost;

    protected virtual void ConsumeMana(int amount)
    {
        Mana = Mathf.Max(0, Mana - amount);
    }

    protected virtual void RestoreMana(int amount)
    {
        Mana = Mathf.Min(currentStats.maxMana, Mana + amount);
    }

    protected abstract int GetSpecialAbilityCost();
    protected abstract void UseSpecialAbility();
    protected abstract void OnDamageTaken(DamageInfo damageInfo);
}
