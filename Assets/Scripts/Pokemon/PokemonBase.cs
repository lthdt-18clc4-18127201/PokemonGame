using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]

public class PokemonBase : ScriptableObject
{
    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite partyScreenSprite;
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableSkill> learnableSkills;
    public string GetName()
    {
        return name;
    }

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public Sprite PartyScreenSprite
    {
        get { return partyScreenSprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }


    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableSkill> LearnableSkills
    {
        get { return learnableSkills; }
    }
}

[System.Serializable]

public class LearnableSkill
{
    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;

    public SkillBase Base
    {
        get { return skillBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Poision,
    Dragon,
    Ground,
    Flying,
    Fighting,
    Ice,
    Rock
}

public class TypeChart
{
    static float[][] chart =
    {
        //                   Nor   Fir   Wat   Ele   Gra   Poi   Dra   Gro   Fly   Fig   Ice   Roc
        /*Nor*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f },
        /*Fir*/ new float[] { 1f, 0.5f, 0.5f,   1f,   2f,   1f, 0.5f,   1f,   1f,   1f,   2f, 0.5f },
        /*Wat*/ new float[] { 1f,   2f, 0.5f,   1f, 0.5f,   1f, 0.5f,   2f,   1f,   1f,   1f,   2f },
        /*Ele*/ new float[] { 1f,   1f,   2f, 0.5f, 0.5f,   1f, 0.5f,   0f,   2f,   1f,   1f,   1f },
        /*Gra*/ new float[] { 1f, 0.5f,   2f,   1f, 0.5f, 0.5f, 0.5f,   2f, 0.5f,   1f,   1f,   2f },
        /*Poi*/ new float[] { 1f,   1f,   1f,   1f,   2f, 0.5f,   1f, 0.5f,   1f,   1f,   1f, 0.5f },
        /*Dra*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   1f,   1f },
        /*Gro*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   0f,   1f,   1f,   2f },
        /*Fly*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f, 0.5f },
        /*Fig*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f,   2f,   2f },
        /*Ice*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   2f,   1f, 0.5f,   1f },
        /*Roc*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   2f, 0.5f,   2f, 0.5f,   2f,   1f },
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if(attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;

        //get the row and column of type
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row][col];
    }
}
