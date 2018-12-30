using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemyBehaviour : MonoBehaviour {

    private INode _rootNode;
    private Transform _transform;
    private NavMeshAgent _navMeshAgent;
    private PlayerController _playerController;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float FOV;
    [SerializeField] private LayerMask _canSeePlayerLayerMask;
    [SerializeField] private float _punchReach;
    [SerializeField] private float _punchCoolDown;
    private float _punchCoolDownTimer = 0;

    private Vector3 _targetPosition;
    void Start()
    {
        _transform = GetComponent<Transform>();
        _targetPosition = _transform.position;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerController = _playerTransform.GetComponent<PlayerController>();

        //#region test

        //INode idk = new SelectorNode(new ConditionNode(IsHungry), new ConditionNode(IsHungry), new ConditionNode(IsHungry));
        //INode selectorNode = new SelectorNode(idk);
        //#endregion

        //ConditionNode.Condition hungryCondition = IsHungry;

        //ConditionNode IsHungrCondition = new ConditionNode(hungryCondition);


        INode punchPlayerAI = new SequenceNode(new ConditionNode(CanPunchPlayer), new ActionNode(PunchPlayer));
        INode followPlayerAI = new SequenceNode(new ConditionNode(SeesPlayer), new SequenceNode(new ActionNode(ChargeAtPlayer), punchPlayerAI));
        //INode sleepyyAI = new SequenceNode(new ConditionNode(IsCloseToPlayer), new ActionNode(Sleep));

        //INode SelectorRootNode = new SelectorNode(hungryAI, sleepyyAI, new ActionNode(Roam));

        //INode ParallelRootNode = new ParallelNode(ParallelAwaysSuccesPolicy, SelectorRootNode, new ActionNode(Blaah));
        //_rootNode = new ParallelNode(NSuccesIsSuccesAccumulator.Factory, SelectorRootNode, new ActionNode(Blaah));

        _rootNode = new ParallelNode(NSuccesIsSuccesAccumulator.Factory, followPlayerAI);
        //_rootNode = new SelectorNode(followPlayerAI);

        StartCoroutine(RunTree());
    }

    private IEnumerator<NodeResult> Roam()
    {
        Debug.Log("Roaming");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> Blaah()
    {
        Debug.Log("Blaaaaah");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> Sleep()
    {

        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Sleeping: " + i);
            yield return NodeResult.Running;
        }

        Debug.Log("Done Sleeping");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> ChargeAtPlayer()
    {
        if (Vector3.Magnitude(_playerTransform.position - _transform.position) > 1)
            _navMeshAgent.SetDestination(_playerTransform.position);
        else _navMeshAgent.SetDestination(_transform.position);

        Debug.Log("Charging");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> PunchPlayer()
    {
        _playerController.TakePunch();
        Debug.Log("TakePunch");
        yield return NodeResult.Succes;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RunTree()
    {
        while (Application.isPlaying)
        {
            yield return _rootNode.Tick();
        }
    }

    bool SeesPlayer()
    {
        Vector3 directionPlayer = _playerTransform.position - _transform.position;
        if (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)) < FOV / 2)
        {
            Debug.Log("angle: " + Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)));
            RaycastHit hit;
            if(Physics.Raycast(_transform.position+new Vector3(0,1.6f,0),directionPlayer, out hit, 1000, _canSeePlayerLayerMask))
            {
                Debug.Log(hit.transform.name);
                if (hit.transform.gameObject.layer == 9)
                {
                    Debug.Log("I see player");
                    return true;
                }
            }
        }

        return false;
    }

    bool CanPunchPlayer()
    {
        if (Vector3.Magnitude(_playerTransform.position - _transform.position) <= _punchReach)
        {
            if (_punchCoolDownTimer > _punchCoolDown)
            {
                Debug.Log("punch");
                _punchCoolDownTimer = 0;
                return true;
            }
            _punchCoolDownTimer += Time.deltaTime;
        }
        else
        _punchCoolDownTimer = 0;
        return false;
    }
}
