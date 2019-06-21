using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementMenu : MonoBehaviour
{
    public static ManagementMenu _managementMenu;

    [SerializeField]
    private NPCManager _StatScreenManager;

    [SerializeField]
    private Sprite _image;

    [SerializeField]
    private GameObject _npcList;

    [SerializeField]
    private GameObject _TaskList;

    [SerializeField]
    private GameObject _MissionList;

    [SerializeField]
    private LayerMask _jobLayer;

    private NPC _npc;

    private bool _invertMask;
    private bool _findDefenceSpot = false;
    private bool _findJob = false;

    void Start()
    {
        _managementMenu = this;
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (_findDefenceSpot)
        {
            SelectDefenceSpot();
        }
        if (_findJob)
        {
            SelectJob();
        }
    }

    public void NewNPC(NPC npc, int fighting, int farming, int gathering, int sacavenging, int health, int trust)
    {
        NPCManager statScreenManager = Instantiate(_StatScreenManager, transform);
        statScreenManager.SetStats(npc);
        statScreenManager.SetSkills(sacavenging, farming, gathering, sacavenging, health, trust);
        statScreenManager.gameObject.transform.SetParent(_npcList.gameObject.transform);
        npc._statScreenManager = statScreenManager;
    }

    public void ResetManager()
    {
        _npcList.SetActive(true);
        _TaskList.SetActive(false);
        _MissionList.SetActive(false);
        _findDefenceSpot = false;
        _findJob = false;
        _npc = null;
    }

    public void SelectNPC(NPC npc)
    {
        _npcList.SetActive(false);
        _TaskList.SetActive(true);
        _npc = npc;
    }

    public void SelectWorking()
    {
        _TaskList.SetActive(false);
        _findDefenceSpot = false;
        _findJob = true;
    }

    public void SelectDefence()
    {
        _TaskList.SetActive(false);
        _findJob = false;
        _findDefenceSpot = true;
    }

    public void SelectMission()
    {
        _TaskList.SetActive(false);
        _MissionList.SetActive(true);
    }

    private void SelectDefenceSpot()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 10000))
        {
            if (Input.GetButtonDown("LeftMouseButtom"))
            {
                _npc.SelectDefendPosition(hit.transform.position);
                ResetManager();
            }
        }
    }

    private void SelectJob()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask newMask = ~(_invertMask ? ~_jobLayer.value : _jobLayer.value);

        if (Physics.Raycast(ray, out hit, 10000, newMask))
        {
            Interacteble interacteble = hit.collider.GetComponent<Interacteble>();
            if (interacteble != null && Input.GetButtonDown("LeftMouseButtom"))
            {
                _npc.SelectJob(interacteble);
                ResetManager();
            }
        }
    }
}
