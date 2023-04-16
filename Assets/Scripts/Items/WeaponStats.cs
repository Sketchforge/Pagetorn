using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.SoundSystem;

public class WeaponStats : MonoBehaviour
{
    [SerializeField] public ItemType itemType;
    [SerializeField] public int damage;
    [SerializeField] public int knockback;
    [SerializeField] private Sfx hitSFX;


	private void OnTriggerEnter(Collider other)
	{
        EnemyBase enemy = other.GetComponent<EnemyBase>();
		if (enemy != null)
		{
            enemy.GetComponent<Health>().Damage(damage);
            Vector3 knockDir = transform.position - other.transform.position;
            knockDir.y = 0;
            enemy.KnockBack(knockback, knockDir.normalized);
            hitSFX.Play();
		}
	}
}
