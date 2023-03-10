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


    /*------Properties------*/
    public int HP { get; set; }
    public List<Skill> Skills { get; set; } 
    //use dictionary to store key(Stat) and value(int)
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    //use queue to store list of status
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Condition Status { get; private set; }

    public bool HPChanged { get; set; }

    /*----------------------*/

    public void Init() {
        //generate skills
        Skills = new List<Skill>();
        foreach (var skill in Base.LearnableSkills)
        {
            if (skill.Level <= Level)
                Skills.Add(new Skill(skill.Base));
            if (Skills.Count >= 4)
                break;
        }

        CaculateStats();
        HP = MaxHp;
        ResetStatBoost();
    }

    void CaculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpDefense, 0},
            {Stat.SpAttack, 0},
            {Stat.Speed, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    // modify stat boost when status skill was perform
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");


            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp { get; private set; }

    public DamageDetails TakeDamage(Skill skill, Pokemon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f < 6.25f)
            critical = 2f;

        // get type of pokemon
        float type = TypeChart.GetEffectiveness(skill.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(skill.Base.Type, this.Base.Type2);
        
        // set damage details
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        // check if the attack is special attack
        float attack = (skill.Base.Category == SkillCategory.Special) ? attacker.SpAttack : attacker.Attack;

        // check if the defense is special defense
        float defense = (skill.Base.Category == SkillCategory.Special) ? SpDefense : Defense;

        // count the dame base on type and critical
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HPChanged = true;
    }

    public void SetStatus(ConditionID conditionID)
    {
        Status = ConditionsDB.Conditions[conditionID];
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
    }

    public Skill GetRandomSkill() {
        int i = Random.Range(0, Skills.Count);
        return Skills[i];
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this); // null-condition operator
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}
