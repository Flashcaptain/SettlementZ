using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridSystem : MonoBehaviour
{
    public static GridSystem _gridSystem;

    public bool _hasObject = false;

    [SerializeField]
    private NavMeshSurface _surface;

    [SerializeField]
    private GameObject _player;

    [Header("Camera Settings")]
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private int _outsideFOV;

    [SerializeField]
    private int _houseFOV;

    [Header("Map Spawning")]

    [SerializeField]
    private List<PlacebleObject> _tiles;

    [SerializeField]
    private int _width;

    [SerializeField]
    private int _height;

    [Header("Tree Spawning")]

    [SerializeField]
    private Texture2D _noiseImage;

    [SerializeField]
    private List<PlacebleObject> _PineTrees;

    [SerializeField]
    [Range(0,1)]
    private float _pineDensity;

    [SerializeField]
    private List<PlacebleObject> _oakTrees;

    [SerializeField]
    [Range(0,1)]
    private float _oakDensity;

    [Header("Placing Objects")]
    [SerializeField]
    private LayerMask _mouseIgnoreLayerMask;

    [SerializeField]
    private Color _fadeColor;

    [SerializeField]
    private Color _occupiedColor;

    [Header("UI")]
    public bool _inBuildMenu = false;
    public bool _inInventory = false;
    public bool _inManagementMenu = false;

    private Material _material;

    private PlacebleObject _Object;
    private PlacebleObject _placementObject;
    private Transform _target;
    private Vector3 _truePosition;
    private Vector3 _lastPlayerPosition;
    private float _currentRotation;
    private List<PlacebleObject> _occupiedPositions = new List<PlacebleObject>();

    private int _gridSize = 1;
    private int _placementHieght = 1;
    private Collider _collider;
    private bool _invertMask;

    void Start()
    {
        _gridSystem = this;
        DontDestroyOnLoad(this);
        TerainSpawner();
        ForestSpawner();
        _surface.Bake();
    }

    private void TerainSpawner()
    {
        for (int x = -1 * (_width / 2); x < _width / 2; x++)
        {
            for (int z = -1 * (_height / 2); z < _width / 2; z++)
            {
                Vector3 spawnPosition = new Vector3(x, 0, z);
                int tileNumber = Random.Range(0, _tiles.Count);
                PlacebleObject tile = Instantiate(_tiles[tileNumber], spawnPosition, transform.rotation);
                tile._location = spawnPosition;
                _occupiedPositions.Add(tile);
            }
        }
    }

    private void ForestSpawner()
    {
        _oakDensity =(-1 * _oakDensity) + 1;

        for (int x = -1 * (_width / 2); x < _width / 2; x++)
        {
            for (int z = -1 * (_height / 2); z < _width / 2; z++)
            {
                float chance = _noiseImage.GetPixel(x, z).r;
                if (_pineDensity >= chance)
                {

                    Vector3 spawnPosition = new Vector3(x, 1, z);
                    int treeNumber = Random.Range(0, _PineTrees.Count);
                    PlacebleObject tile = Instantiate(_PineTrees[treeNumber], spawnPosition, transform.rotation);
                    tile._location = spawnPosition;
                    _occupiedPositions.Add(tile);
                }
                else if (_oakDensity <= chance)
                {
                    Vector3 spawnPosition = new Vector3(x, 1, z);
                    int treeNumber = Random.Range(0, _oakTrees.Count);
                    PlacebleObject tile = Instantiate(_oakTrees[treeNumber], spawnPosition, transform.rotation);
                    tile._location = spawnPosition;
                    _occupiedPositions.Add(tile);
                }
            }
        }
    }

    public void GetObject(PlacebleObject placementObject)
    {
        _hasObject = true;
        _Object = placementObject;
        _placementHieght = 1;
        BuildMenu._buildMenu.gameObject.SetActive(false);
        _placementObject = Instantiate(placementObject, placementObject.transform.position, Quaternion.Euler(placementObject.transform.rotation.x, _currentRotation, placementObject.transform.rotation.z));
        _material = _placementObject.GetComponent<Renderer>().material;

        Collider collider = _placementObject.GetComponent<Collider>();

        if (collider != null)
        {
            _collider = collider;
            _collider.enabled = false;
        }
    }

    private void RotateObject()
    {
        _placementObject.transform.Rotate(0, +90, 0);
        _currentRotation = _placementObject.transform.rotation.eulerAngles.y;
    }

    private void FlipObject()
    {
        _placementObject.transform.localScale = new Vector3(_placementObject.transform.localScale.x * -1, 1, 1);
    }

    private void PlaceObject()
    {
        HoverObject(true);

        _hasObject = false;
        _placementObject.GetComponent<MeshRenderer>().enabled = false;

        if (_collider != null)
        {
            _collider.enabled = true;
        }
        _surface.Bake();
        //_surface.BuildNavMesh();
        GetObject(_Object);
    }

    private void CancelObject()
    {
        _hasObject = false;
        BuildMenu._buildMenu.gameObject.SetActive(true);
        Destroy(_placementObject.gameObject);
    }

    private void DestroyObject()
    {
        for (int i = 0; i < _occupiedPositions.Count; i++)
        {
            if (!_occupiedPositions[i]._isEnvironment && _truePosition == _occupiedPositions[i]._location)
            {
                Destroy(_occupiedPositions[i].gameObject);
                _occupiedPositions.Remove(_occupiedPositions[i]);
            }
        }
        _surface.Bake();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(_occupiedPositions.Count);
        }

        InHouse(_player.transform.position);

        if (Input.GetButtonDown("Inventory") && !_inBuildMenu && !_inManagementMenu)
        {
            ToggleInventory();
        }

        if (Input.GetButtonDown("Management Menu") && !_inBuildMenu && !_inInventory)
        {
            ManagementMenu._managementMenu.ResetManager();
            ToggleManagementMenu();
        }

        if (Input.GetButtonDown("Build Menu") && !_inManagementMenu && !_inInventory && !GridSystem._gridSystem._hasObject)
        {
            ToggleBuildMenu();
        }

        if (_hasObject)
        {
            BuildMenuControls();
        }
    }

    public void BuildMenuControls()
    {
        SetCollor();

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask newMask = ~(_invertMask ? ~_mouseIgnoreLayerMask.value : _mouseIgnoreLayerMask.value);

        if (Physics.Raycast(ray, out hit, 10000 ,newMask))
        {
            _target = (hit.transform);
            HoverObject(false);
        }

        if (Input.GetButtonDown("LeftMouseButtom") && CheckAvailable())
        {
            PlaceObject();
        }

        if (Input.GetButtonDown("RotateObject"))
        {
            RotateObject();
        }

        if (Input.GetButtonDown("FlipObject"))
        {
            FlipObject();
        }

        if (Input.GetButtonDown("SwitchVersion"))
        {
            if (_placementObject._multipleVersions)
            {
                _placementObject.NextVersion();
            }
        }

        if (Input.GetButtonDown("RightMouseButtom"))
        {
            CancelObject();
        }

        if (Input.GetButtonDown("Delete"))
        {
            DestroyObject();
        }

        if (Input.GetButtonDown("PlaceUp"))
        {
            _placementHieght++;
        }

        if (Input.GetButtonDown("PlaceDown"))
        {
            _placementHieght--;
        }
    }


    public void ToggleInventory()
    {
        _inInventory = _inInventory ? false : true;
        ResourcesList._resourcesList.gameObject.SetActive(_inInventory);
        Cursor.lockState = _inInventory ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void ToggleBuildMenu()
    {
        _inBuildMenu = _inBuildMenu ? false : true;
        BuildMenu._buildMenu.gameObject.SetActive(_inBuildMenu);
        Cursor.lockState = _inBuildMenu ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void ToggleManagementMenu()
    {
        _inManagementMenu = _inManagementMenu ? false : true;
        ManagementMenu._managementMenu.gameObject.SetActive(_inManagementMenu);
        Cursor.lockState = _inManagementMenu ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void SetCollor()
    {
        if (CheckAvailable())
        {
            _material.color = _fadeColor;
        }
        else
        {
            _material.color = _occupiedColor;
        }
    }

    public void InHouse(Vector3 playerPosition)
    {
        playerPosition.x += _gridSize / 2f;
        playerPosition.z += _gridSize / 2f;

        playerPosition.x = Mathf.Floor(playerPosition.x / _gridSize) * _gridSize;
        playerPosition.y = Mathf.Floor(playerPosition.y / _gridSize) * _gridSize;
        playerPosition.z = Mathf.Floor(playerPosition.z / _gridSize) * _gridSize;

        if (_lastPlayerPosition == playerPosition)
        {
            return;
        }
        _lastPlayerPosition = playerPosition;

        for (int i = 0; i < _occupiedPositions.Count; i++)
        {
            if (_occupiedPositions[i]._isHouse && playerPosition == _occupiedPositions[i]._location)
            {
                EnterHouse();
                return;
            }
        }
        ExitHouse();
    }

    public void EnterHouse()
    {
        _camera.fieldOfView = _houseFOV;
        for (int i = 0; i < _occupiedPositions.Count; i++)
        {
            if (_occupiedPositions[i]._isHouse && _lastPlayerPosition.y < _occupiedPositions[i]._location.y)
            {
                _occupiedPositions[i].gameObject.SetActive(false);
            }
            else if (_occupiedPositions[i]._isHouse && _lastPlayerPosition.y <= _occupiedPositions[i]._location.y)
            {
                _occupiedPositions[i].gameObject.SetActive(true);
            }
        }
    }

    public void ExitHouse()
    {
        _camera.fieldOfView = _outsideFOV;
        for (int i = 0; i < _occupiedPositions.Count; i++)
        {
            if (_occupiedPositions[i]._isHouse)
            {
                _occupiedPositions[i].gameObject.SetActive(true);
            }
        }
    }

    private void HoverObject(bool isPlacing)
    {
        _truePosition.x = Mathf.Floor(_target.transform.position.x / _gridSize) * _gridSize;
        _truePosition.y = Mathf.Floor(_target.transform.position.y / _gridSize) * _gridSize + _placementHieght;
        _truePosition.z = Mathf.Floor(_target.transform.position.z / _gridSize) * _gridSize;

        _placementObject.transform.position = _truePosition;

        if (isPlacing)
        {
            _placementObject._location = _truePosition;
            _occupiedPositions.Add(_placementObject);
        }
    }

    private bool CheckAvailable()
    {
        if (CheckOccupied() == !_placementObject._isDecoration || !CheckStackeble())
        {
            return false;
        }

        if (!CheckDecorationAvailable())
        {
            return false;
        }
        return true;
    }

    private bool CheckOccupied()
    {
        for (int i = 0; i < _occupiedPositions.Count; i++)
        {
            if (_truePosition == _occupiedPositions[i]._location)
            {
                return true;
            }

        }
        return false;
    }

    private bool CheckStackeble()
    {
        for (int i = 0; i < _occupiedPositions.Count; i++)
        {
            Vector3 belowPosition = new Vector3(_truePosition.x, _truePosition.y - 1, _truePosition.z);
            if (belowPosition == _occupiedPositions[i]._location && _occupiedPositions[i]._isStackeble)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckDecorationAvailable()
    {

        for (int i = 0; i < _occupiedPositions.Count; i++)
        {
            if ((!_occupiedPositions[i]._isDecoration && !_occupiedPositions[i]._isHouse) && _truePosition == _occupiedPositions[i]._location)
            {
                return false;
            }
        }
        return true;
    }
}
