using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour {

    [SerializeField]private Transform _horizontalAnchorTransform;
    [SerializeField] private Transform _verticalAnchorTransform;
    public Transform LeftHandIK;
    public Transform RightHandIK;

    public Transform VerticalAnchorTransform
    {
        get
        {
            return _verticalAnchorTransform;
        }
    }
    [SerializeField] private Transform _playerTransform;
    public Transform PlayerPosition
    {
        get { return _playerTransform; }
    }
    [SerializeField] private Transform _camDefaultTransform;
    public Transform CamDefaultTransform
    {
        get
        {
            return _camDefaultTransform;
        }
    }
    [SerializeField] private Transform _camAimingTransform;
    public Transform CamAimingTransform
    {
        get
        {
            return _camAimingTransform;
        }
    }

    [SerializeField] private float _fireRate;
    private float _fireTimer;

    [SerializeField] private GameObject _fireTurretEffect;
    private Coroutine _showFireEffect;

    [SerializeField] private int _bulletDamage;
    [SerializeField] private LayerMask _bulletLayermask;
    private Camera _cam;

    private float _verticalRotation=0;
    private float _horizontalRotation = 0;
    [SerializeField] private float _minVerticalAngle;
    [SerializeField] private float _maxVerticalAngle;
    [SerializeField] private float _minHorizontalAngle;
    [SerializeField] private float _maxHorizontalAngle;


    // Use this for initialization
    void Start () {
        _fireTimer = _fireRate;
        _cam = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FireTurret(bool isShooting)
    {
        if (isShooting)
        {
            if (_fireTimer >= _fireRate)
            {
                _fireTimer = 0;
                FireBullet();
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

    private void FireBullet()
    {
        Ray _centreScreenRay = _cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(_centreScreenRay, out hit, 1000, _bulletLayermask))
        {
            print("I'm looking at " + hit.transform.name);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hit.transform.GetComponent<EnemyBehaviour>())
                    hit.transform.GetComponent<EnemyBehaviour>().GetShot(_bulletDamage,_verticalAnchorTransform.position);

                //other enemies
            }

        }
        else
            print("I'm looking at nothing!");

        PlayEffect();
    }

    public Vector3 GetDirection()
    {
        return _verticalAnchorTransform.forward;
    }
    public Vector3 GetHorizontalDirection()
    {
        return _horizontalAnchorTransform.forward;
    }

    public void Rotate(float horizontalRotation, float verticalRotation)
    {
        //horizontal rotation
        _horizontalRotation += horizontalRotation; //get vertical rotation

        _horizontalRotation = Mathf.Clamp(_horizontalRotation, _minHorizontalAngle, _maxHorizontalAngle); //clamp vertical rotation

        transform.localEulerAngles = new Vector3(transform.eulerAngles.x, _horizontalRotation, transform.eulerAngles.z);

        //vertical rotation
        _verticalRotation += verticalRotation; //get vertical rotation

        _verticalRotation = Mathf.Clamp(_verticalRotation, _minVerticalAngle, _maxVerticalAngle); //clamp vertical rotation

        _verticalAnchorTransform.eulerAngles = new Vector3(_verticalRotation, _verticalAnchorTransform.eulerAngles.y, _verticalAnchorTransform.eulerAngles.z);
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
