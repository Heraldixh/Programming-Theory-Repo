using UnityEngine;

public abstract class StatusEffect
{
    public float Duration { get; protected set; }
    public bool IsExpired => Duration <= 0;

    public abstract void OnApply(Character target);
    public abstract void Update(float deltaTime);
    public abstract void OnRemove(Character target);
}

public class BurningEffect : StatusEffect
{
    private readonly int damagePerTick;
    private readonly float tickRate;
    private float tickTimer;

    public BurningEffect(float duration, int damagePerTick, float tickRate)
    {
        Duration = duration;
        this.damagePerTick = damagePerTick;
        this.tickRate = tickRate;
    }

    public override void OnApply(Character target)
    {
        // Add visual effect 
    }

    public override void Update(float deltaTime)
    {
        Duration -= deltaTime;
        tickTimer += deltaTime;

        if (tickTimer >= tickRate)
        {
            tickTimer = 0;
           
        }
    }

    public override void OnRemove(Character target)
    {
        // Remove visual effect
    }
}