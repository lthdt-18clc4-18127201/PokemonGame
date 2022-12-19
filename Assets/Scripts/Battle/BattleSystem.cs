using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState 
{ 
    Start, 
    ActionSelection,
    TurnSelecion, 
    PerformTurn, 
    Busy,
    PartyScreen,
    BattleOver
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen  partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentSkill;
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        dialogBox.SetSkillNames(playerUnit.Pokemon.Skills);

        partyScreen.Init();
        

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");

        ActionSelection();
    }

    //trigger that the battle is over
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog($"What will {playerUnit.Pokemon.Base.Name} do?"));
        dialogBox.EnableActionSelector(true);
    }
    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void TurnSelection()
    {
        state = BattleState.TurnSelecion;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableSkillSelector(true);
    }


    IEnumerator PlayerTurn()
    {
        //set state for playerTurn because the
        //player can change skill during function
        state = BattleState.PerformTurn;

        var skill = playerUnit.Pokemon.Skills[currentSkill];
        yield return RunTurn(playerUnit, enemyUnit, skill);

        //if the battle start was not changed by RunTurn, then go to next step
        if(state == BattleState.PerformTurn)
            StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.PerformTurn;
        var skill = enemyUnit.Pokemon.GetRandomSkill();
        yield return RunTurn(enemyUnit, playerUnit, skill);

        //if the battle start was not changed by RunTurn, then go to next step
        if (state == BattleState.PerformTurn)
            ActionSelection();
    }

    //play the turn of current player or enemy
    IEnumerator RunTurn(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill)
    {
        skill.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {skill.Base.Name}");
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(0.25f);
        targetUnit.PlayHitAnimation();

        //check if enemy's pokemon is fainted
        var damageDetails = targetUnit.Pokemon.TakeDamage(skill, sourceUnit.Pokemon);
        yield return targetUnit.Hud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} fainted");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);

            CheckForBattleOver(targetUnit);
        }
    }

    //check for battle is over
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            //check if player have another healthy pokemon
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else
                //if not, end battle
                BattleOver(false);
        }
        else
            BattleOver(true);
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }

    public void HandleUpdate()
    {
        if(state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.TurnSelecion)
        {
            HandleSkillSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction == 0)
            {
                //Fight
                TurnSelection();
            }
            else if (currentAction == 1)
            {
                //Bag

            }
            else if (currentAction == 2)
            {
                //Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Run

            }
        }
    }

    void HandleSkillSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentSkill;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentSkill;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentSkill += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentSkill -= 2;

        currentSkill = Mathf.Clamp(currentSkill, 0, playerUnit.Pokemon.Skills.Count - 1);


        dialogBox.UpdateSkillSelection(currentSkill, playerUnit.Pokemon.Skills[currentSkill]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableSkillSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerTurn());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableSkillSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    //handle party screen 
    void HandlePartyScreenSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("This pokemon was fainted!");
                return;
            }
            if(selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("This pokemon already in battle!");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ActionSelection();
            partyScreen.gameObject.SetActive(false);
        }
    }

    //switch pokemon
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        dialogBox.EnableActionSelector(false);

        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Comeback {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
        }

        playerUnit.Setup(newPokemon);

        dialogBox.SetSkillNames(newPokemon.Skills);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        StartCoroutine(EnemyTurn());
    }
}
