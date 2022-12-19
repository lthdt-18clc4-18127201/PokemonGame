using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public PokemonBase Base {
        get
        {
            return _base;
        }
    }
    public int Level { 
        get {
            return level;
        } 
    }

    public int HP { get; set; }
    public List<Skill> Skills { get; set; } 
    public void Init() {
        HP = MaxHp;

        //generate skills
        Skills = new List<Skill>();
        foreach (var skill in Base.LearnableSkills)
        {
            if (skill.Level <= Level)
                Skills.Add(new Skill(skill.Base));
            if (Skills.Count >= 4)
                break;
        }
    }
    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10; }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    public DamageDetails TakeDamage(Skill skill, Pokemon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f < 6.25f)
            critical = 2f;

        //get type of pokemon
        float type = TypeChart.GetEffectiveness(skill.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(skill.Base.Type, this.Base.Type2);
        
        //set damage details
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        //check if the attack is special attack
        float attack = (skill.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack;

        //check if the defense is special defense
        float defense = (skill.Base.IsSpecial) ? SpDefense : Defense;

        //count the dame base on type and critical
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Skill GetRandomSkill() {
        int i = Random.Range(0, Skills.Count);
        return Skills[i];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}