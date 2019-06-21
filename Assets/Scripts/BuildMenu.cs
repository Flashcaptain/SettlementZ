using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    public static BuildMenu _buildMenu;

    public List<GameObject> _buildingCategories;

    void Start()
    {
        _buildMenu = this;
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
    }

    public void SelectCategory(int category)
    {
        _buildingCategories[category].SetActive(true);

        for (int i = 0; i < _buildingCategories.Count; i++)
        {
            if (i!=category)
            {
                _buildingCategories[i].SetActive(false);
            }
        }
    }
}
