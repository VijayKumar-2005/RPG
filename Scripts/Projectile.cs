using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject explosionEffectPrefab;

    void OnCollisionEnter(Collision collision)
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
