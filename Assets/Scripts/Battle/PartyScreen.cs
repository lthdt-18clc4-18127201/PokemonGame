using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }


    //set data in party screen
    public void SetPartyData(List<Pokemon> pokemons)
    {

        this.pokemons = pokemons;
        for(int i =0; i<memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
                memberSlots[i].SetData(pokemons[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon!";
    }

    //show UI which pokemon is selecting
    public void UpdateMemberSelection(int selectedMemer)
    {
        for(int i=0; i< pokemons.Count; i++)
        {
            if (i == selectedMemer)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    //set message
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
