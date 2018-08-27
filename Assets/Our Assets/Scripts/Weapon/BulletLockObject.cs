using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLockObject : MonoBehaviour, IDamagable {

    [SerializeField] private GameObject m_toBeDeactivated;

    public void TakeDamage(int a_damage) {
        m_toBeDeactivated.SetActive(false);
    }

    public void TakeHit(int a_damage, RaycastHit a_hit) {
        TakeDamage(a_damage);
    }

    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile) {
        TakeHit(a_damage, a_hit);
    }

}
