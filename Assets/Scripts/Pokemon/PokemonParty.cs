using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons {
        get {
            return pokemons;
        }
    }

    private void Start()
    {
        foreach(var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        //loop for all pokemon available and return first one healthy
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }
}
