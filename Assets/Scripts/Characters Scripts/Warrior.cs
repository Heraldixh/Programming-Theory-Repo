using System.Collections;
using UnityEngine;

public class Warrior : Character
{
    public bool IsUsingSpecialAbility { get; private set; }

    [SerializeField] private float rageMeter;
    [SerializeField] private float maxRage = 100f;
    [SerializeField] private float rageGainOnHit = 10f;
    [SerializeField] private float rageDrainRate = 5f;
    [SerializeField] private ParticleSystem rageEffect;
    [SerializeField] private int specialAbilityCost = 30;


    protected override void Update()
    {
        base.Update();
        UpdateRage();
    }

    private void UpdateRage()
    {
        rageMeter = Mathf.Max(0, rageMeter - rageDrainRate * Time.deltaTime);
        if (rageMeter >= maxRage) ActivateBerserkMode();
    }

    private void ActivateBerserkMode()
    {
        if (rageEffect != null) rageEffect.Play();
        currentStats.attackSpeed *= 1.5f;
        currentStats.moveSpeed *= 1.2f;
        rageMeter = 0;
    }

    protected override void OnDamageTaken(DamageInfo damageInfo)
    {
        rageMeter = Mathf.Min(maxRage, rageMeter + rageGainOnHit);
    }

    protected override int GetSpecialAbilityCost() => specialAbilityCost;

    protected override void UseSpecialAbility()
    {
        if (!HasEnoughMana(GetSpecialAbilityCost())) return;
        ConsumeMana(GetSpecialAbilityCost());
        StartCoroutine(WhirlwindAttack());
    }

    private IEnumerator WhirlwindAttack()
    {
        float duration = 2f;
        float radius = 3f;
        float rotationSpeed = 720f;


        while (duration > 0)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Character>(out var target) && target != this)
                {
                    target.TakeDamage(new DamageInfo
                    {
                        amount = 20,
                        type = DamageType.Physical,
                        source = this
                    });
                }
            }

            duration -= Time.deltaTime;
            yield return null;
        }
    }
}