using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float currentDamage = 1f;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] tears;
    [SerializeField]private AudioClip tearsSound;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown)
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    public void AddDamage(float amount)
    {
        currentDamage += amount;
        Debug.Log("Zwiêkszono obra¿enia do: " + currentDamage);
    }

    private void Attack()
    {
        SoundManager.instance.PlaySound(tearsSound);
        anim.SetTrigger("Attack");
        cooldownTimer = 0;

        int index = FindTear();
        tears[FindTear()].transform.position = firePoint.position;
        tears[FindTear()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        tears[index].GetComponent<Projectile>().damage = currentDamage;
        tears[index].SetActive(true);
    }

    private int FindTear()
    {
        for(int i = 0; i < tears.Length; i++)
        {
            if (!tears[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
