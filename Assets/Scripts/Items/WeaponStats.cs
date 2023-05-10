using UnityEngine;
using CoffeyUtils.Sound;

public class WeaponStats : MonoBehaviour
{
    [SerializeField] public ItemType itemType;
    [SerializeField] public int damage;
    [SerializeField] public int knockback;
    [SerializeField] private Sfx hitSFX;
    private DataReactor _theDataReactor;


	private void OnTriggerEnter(Collider other)
	{
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            Targetable _targetable = enemy.GetComponent<Targetable>();
            if (_targetable.Type != TargetableType.Witch)
            {
                enemy.GetComponent<Health>().Damage(damage);
                enemy._impactParticles.Play();
                Vector3 knockDir = transform.position - other.transform.position;
                knockDir.y = 0;
                enemy.KnockBack(knockback, knockDir.normalized);
                hitSFX.Play();
            }
            else if (_targetable.Type == TargetableType.Witch)
            {
                if (!_theDataReactor) _theDataReactor = FindObjectOfType<DataReactor>();

                else if (_theDataReactor) _theDataReactor.ForceTeleportLibrarian(true);
            }
        }
            
	}
}
