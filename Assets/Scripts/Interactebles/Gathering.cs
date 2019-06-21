using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gathering : Interacteble
{
    [SerializeField]
    private TaskType _taskType;

    [SerializeField]
    private int _AmountOfResources;

    [SerializeField]
    private int _ResourcesPerMinute;

    private NPC _npc;
    private bool _onCooldown;

    void AddResources()
    {
        switch (_taskType)
        {
            case TaskType.Food:
                ResourcesList._resourcesList._food++;
                break;
            case TaskType.Wood:
                ResourcesList._resourcesList._wood++;
                break;
            case TaskType.Stone:
                ResourcesList._resourcesList._stone++;
                break;
            case TaskType.SteelScrap:
                ResourcesList._resourcesList._steelScrap++;
                break;
            case TaskType.random:
                break;
        }
        _AmountOfResources--;
        if (_AmountOfResources == 0)
        {
            Destroy(this.gameObject);
        }
    }

    public override void Action(NPC npc)
    {
        if (!_onCooldown)
        {
            _onCooldown = true;
            AddResources();
            StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(60/_ResourcesPerMinute);
        _onCooldown = false;
    }
}
