using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    public int xpAmount = 1;
    public float moveSpeed = 5f;
    public float pickupDistance = 1.5f;

    private Transform player;
    private bool isFollowing = false;

    void Start() => player = GameObject.FindGameObjectWithTag("Player").transform;

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < pickupDistance) isFollowing = true;

        if (isFollowing)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            if (distance < 0.1f)
            {
                player.GetComponent<PlayerStats>().AddExperience(xpAmount);
                Destroy(gameObject);
            }
        }
    }
}
