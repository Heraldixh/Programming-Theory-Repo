using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    private int damage;
    private Vector2 direction;
    private bool isEnhanced;
    private ParticleSystem trailEffect;
    private float timer;

    public void Setup(Vector2 dir, int dmg, bool enhanced = false)
    {
        direction = dir.normalized;
        damage = dmg;
        isEnhanced = enhanced;
        timer = lifetime;

        GetComponent<SpriteRenderer>().flipX = direction.x < 0;

        if (isEnhanced && trailEffect != null)
        {
            var main = trailEffect.main;
            main.startColor = Color.blue;
        }
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer <= 0) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Character>(out var target) && !target.CompareTag("Player"))
        {
            var damageInfo = new DamageInfo
            {
                amount = damage,
                type = DamageType.Magical,
                source = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Character>()
            };

            target.TakeDamage(damageInfo);

            if (isEnhanced)
            {
                target.AddStatusEffect(new BurningEffect(5f, 5, 1f));
            }

            gameObject.SetActive(false);
        }
    }
}