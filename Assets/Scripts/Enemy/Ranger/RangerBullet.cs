using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerBullet : MonoBehaviour
{
    public Rigidbody rb;
    public int force;
    public float speed = 1f;
    public bool hitPlayer = false;
    public EnemyData parentData;
    [SerializeField] LayerMask hittable;

    [SerializeField]
    public Transform targetTransform;

    [SerializeField]
    float initialAngle;

    void Start()
    {
        Debug.Log("Bullet Exists");
        var rigid = GetComponent<Rigidbody>();

        Vector3 p = targetTransform.position;

        float gravity = Physics.gravity.magnitude;
        // Selected angle in radians
        float angle = initialAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.x > transform.position.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        // Fire!
        rigid.velocity = finalVelocity;

        // Alternative way:
        // rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((hittable.value & (1 << collision.transform.gameObject.layer)) != 0)
        {
            if (collision.transform == targetTransform)
            {
                //is player
                PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, Random.Range(parentData.AttackDamage / 2, parentData.AttackDamage));
                
            }

            Destroy(gameObject);
        }
        
        

    }

}
