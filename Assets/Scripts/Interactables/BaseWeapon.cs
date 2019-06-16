using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [Space]
    [Header("Bullet Parameters")]
    [SerializeField] private float _fireRate;
    private float _fireTimer;

    [SerializeField] private GameObject _fireTurretEffect;
    private Coroutine _showFireEffect;

    [SerializeField] private int _bulletDamage;

    protected virtual void Start()
    {
        _fireTimer = _fireRate;
    }

    public void FireWeapon(bool isShooting, Ray trajectory, LayerMask bulletLayerMask)
    {
        if (isShooting)
        {
            if (_fireTimer >= _fireRate)
            {
                _fireTimer = 0;
                FireBullet(trajectory, bulletLayerMask);
            }
            else
            {
                _fireTimer += Time.deltaTime;
            }
        }
        else
        {
            _fireTimer = _fireRate;
        }
    }

    private void FireBullet(Ray trajectory, LayerMask bulletLayerMask)
    {
        RaycastHit hit;
        if (Physics.Raycast(trajectory, out hit, 1000, bulletLayerMask))
        {
            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(_bulletDamage, hit.point);
        }

        PlayEffect();
    }

    private void PlayEffect()
    {
        if (_showFireEffect == null)
            _showFireEffect = StartCoroutine(ShowFireEffect());
        else
        {
            StopCoroutine(_showFireEffect);
            _showFireEffect = StartCoroutine(ShowFireEffect());
        }
    }

    private IEnumerator ShowFireEffect()
    {
        _fireTurretEffect.SetActive(true);
        yield return new WaitForSeconds(.05f);
        _fireTurretEffect.SetActive(false);
    }
}
