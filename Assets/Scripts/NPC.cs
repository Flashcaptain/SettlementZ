using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : Actor
{
    public NPCManager _statScreenManager;

    public Vector3 _target;

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private NavMeshAgent _agent;

    [SerializeField]
    private float _taskRange;

    public string _name;
    public Sprite _image;

    private int _trust;

    private int _fightingSkill;
    private int _farmingSkill;
    private int _gatheringSkill;
    private int _scavangingSkill;

    private int _maxSkill = 10;
    private int _minSkill = 0;

    private int _maxTrust = 100;
    private int _maxStartTrust = 50;
    private int _minTrust = 0;

    private bool _defending;
    private Interacteble _interacteble;

    void Start()
    {
        SetValues();
    }

    void SetValues()
    {
        _fightingSkill = Random.Range(_minSkill, _maxSkill);
        _farmingSkill = Random.Range(_minSkill, _maxSkill);
        _gatheringSkill = Random.Range(_minSkill, _maxSkill);
        _scavangingSkill = Random.Range(_minSkill, _maxSkill);

        if (20 <= (_fightingSkill + _farmingSkill + _gatheringSkill + _scavangingSkill))
        {
            _fightingSkill -= 3;
            _farmingSkill -= 3;
            _gatheringSkill -= 3;
            _scavangingSkill -= 3;
        }
        _fightingSkill = Mathf.Clamp(_fightingSkill, _minSkill, _maxSkill);
        _farmingSkill = Mathf.Clamp(_farmingSkill, _minSkill, _maxSkill);
        _gatheringSkill = Mathf.Clamp(_gatheringSkill, _minSkill, _maxSkill);
        _scavangingSkill = Mathf.Clamp(_scavangingSkill, _minSkill, _maxSkill);

        _trust = Random.Range(_minTrust, _maxStartTrust);
        ManagementMenu._managementMenu.NewNPC(this, _fightingSkill, _farmingSkill, _gatheringSkill, _scavangingSkill, _health, _trust);
    }

    void Update()
    {
        if (_agent == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, _target);
  
        if (_taskRange < distance)
        {
            _agent.SetDestination(_target);
        }
        else if (_interacteble != null && !_defending)
        {
            _interacteble.Action(this);
        }
    }

    public void SelectDefendPosition(Vector3 position)
    {
        _defending = true;
        _target = position;
        _agent.SetDestination(_target);
    }

    public void SelectJob(Interacteble interacteble)
    {
        _defending = false;
        _interacteble = interacteble;
        _target = interacteble.gameObject.transform.position;
    }
}
