using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image partySprite;

    [SerializeField] Color highlightedColor;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "lv." + pokemon.Level;
        partySprite.sprite = pokemon.Base.PartyScreenSprite;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
    }

    //show UI color selected
    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = highlightedColor;
            levelText.color = highlightedColor;
        }
        else
        {
            nameText.color = Color.black;
            levelText.color = Color.black;
        }
    }
}
