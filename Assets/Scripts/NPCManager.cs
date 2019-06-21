using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour
{
    public NPC _npc;

    [SerializeField]
    private Text _nameText;

    [SerializeField]
    private Text _levelText;

    [SerializeField]
    private List<Text> _skillText;

    [SerializeField]
    private List<GameObject> _skillBar;

    [SerializeField]
    private GameObject _healthBar;

    [SerializeField]
    private GameObject _TrustBar;

    [SerializeField]
    private Image _image;

    public void SetStats(NPC npc)
    {
        _npc = npc;
        _nameText.text = npc._name;
        _image.sprite = npc._image;
    }

    public void SetSkills(int fighting, int farming, int gathering, int sacavenging, int health, int trust)
    {
        _skillText[0].text = (fighting + " / 10");
        _skillBar[0].transform.localScale = new Vector3(fighting, 1, 1);
        _skillText[1].text = (farming + " / 10");
        _skillBar[1].transform.localScale = new Vector3(farming, 1, 1);
        _skillText[2].text = (gathering + " / 10");
        _skillBar[2].transform.localScale = new Vector3(gathering, 1, 1);
        _skillText[3].text = (sacavenging + " / 10");
        _skillBar[3].transform.localScale = new Vector3(sacavenging, 1, 1);

        int level = fighting + farming + gathering + sacavenging;
        _levelText.text = (level + "");

        _healthBar.transform.localScale = new Vector3(1, health, 1);
        _TrustBar.transform.localScale = new Vector3(1, trust, 1);
    }

    public void OnClick()
    {
        ManagementMenu._managementMenu.SelectNPC(_npc);
    }
}
