using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesList : MonoBehaviour
{
    public static ResourcesList _resourcesList;

    [Header("Resources")]
    public int _food;
    public int _meds;
    public int _wood;
    public int _stone;
    public int _steelScrap;
    public int _electronicComponents;
    public int _electricity;

    [Header("Melee")]
    public int _knifes;
    public int _sharpMelee;
    public int _dullMelee;

    [Header("Guns")]
    public int _shotguns;
    public int _pistols;
    public int _snipers;
    public int _machineGuns;

    [Header("Ammo")]
    public int _shotgunAmmo;
    public int _pistolAmmo;
    public int _sniperAmmo;
    public int _machineGunAmmo;

    [Header("UI")]
    public List<GameObject> _ResourceCategories;

    private void Start()
    {
        _resourcesList = this;
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
    }

    public void SelectCategory(int category)
    {
        _ResourceCategories[category].SetActive(true);

        for (int i = 0; i < _ResourceCategories.Count; i++)
        {
            if (i != category)
            {
                _ResourceCategories[i].SetActive(false);
            }
        }
    }
}
