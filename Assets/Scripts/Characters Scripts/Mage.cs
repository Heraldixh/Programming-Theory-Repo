using UnityEngine;

public class Mage : Character
{
    [SerializeField] private float channelTime = 1.5f;
    [SerializeField] private ParticleSystem channelEffect;
    [SerializeField] private int specialAbilityCost = 50;
    private bool isChanneling;
    private float currentChannelTime;

    protected override void Update()
    {
        base.Update();
        if (isChanneling) UpdateChanneling();
    }

    private void UpdateChanneling()
    {
        currentChannelTime += Time.deltaTime;
        if (currentChannelTime >= channelTime)
        {
            CompleteChanneling();
        }
    }

    private void CompleteChanneling()
    {
        isChanneling = false;
        currentChannelTime = 0;
        CastEnhancedFireball();
    }

    protected override int GetSpecialAbilityCost() => specialAbilityCost;

    protected override void UseSpecialAbility()
    {
        if (!HasEnoughMana(GetSpecialAbilityCost())) return;
        StartChanneling();
    }

    private void StartChanneling()
    {
        isChanneling = true;
        if (channelEffect != null) channelEffect.Play();
    }

    private void CastEnhancedFireball()
    {
        ConsumeMana(GetSpecialAbilityCost());

        for (int i = 0; i < 5; i++)
        {
            float angle = i * 72f;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            GameObject fireball = ObjectPool.Instance.SpawnFromPool("Fireball", transform.position, Quaternion.identity);
            if (fireball != null)
            {
                fireball.GetComponent<Fireball>().Setup(direction, 40, true);
            }
        }
    }

    protected override void OnDamageTaken(DamageInfo damageInfo)
    {
        if (isChanneling)
        {
            isChanneling = false;
            currentChannelTime = 0;
            if (channelEffect != null) channelEffect.Stop();
        }
    }
}