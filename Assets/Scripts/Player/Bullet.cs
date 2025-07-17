using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifespan = 2f; // Duration before auto-destroy

    private float timer = 0f;

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifespan)
        {
            Destroy(gameObject);
        }
    }
}
