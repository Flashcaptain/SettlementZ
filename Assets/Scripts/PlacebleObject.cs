using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacebleObject : MonoBehaviour
{
    public Vector3 _location;
    public bool _isStackeble;
    public bool _isHouse;
    public bool _isDecoration;
    public bool _isEnvironment;
    public bool _multipleVersions;

    [SerializeField]
    private List<GameObject> _versions;

    private int _currentversions = 0;

    public void NextVersion()
    {
        _versions[_currentversions].SetActive(false);

        if (_currentversions == (_versions.Count -1))
        {
            _currentversions = 0;
        }
        else
        {
            _currentversions++;
        }

        _versions[_currentversions].SetActive(true);
    }
}
