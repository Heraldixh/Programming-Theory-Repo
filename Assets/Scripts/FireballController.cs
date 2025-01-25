using UnityEngine;

public class FireballController : MonoBehaviour
{
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int fireballDamage = 30;
    [SerializeField] private Transform fireballSpawnPoint;
    private float nextFireTime;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            FireballAttack();
        }
    }

    private void FireballAttack()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)fireballSpawnPoint.position).normalized;
        GameObject fireball = ObjectPool.Instance.SpawnFromPool("Fireball", fireballSpawnPoint.position, Quaternion.identity);
        if (fireball != null)
        {
            fireball.GetComponent<Fireball>().Setup(direction, fireballDamage, false);
        }
        nextFireTime = Time.time + fireRate;
    }
}
