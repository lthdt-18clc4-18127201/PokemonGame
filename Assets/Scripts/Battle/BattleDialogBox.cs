using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    [SerializeField] Color hightlightedColor;

    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] GameObject skillDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> skillTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/ letterPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enable)
    {
        dialogText.enabled = enable;
    }

    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    public void EnableSkillSelector(bool enable)
    {
        skillSelector.SetActive(enable);
        skillDetails.SetActive(enable);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i=0; i<actionTexts.Count; i++)
        {
            if (i == selectedAction)
                actionTexts[i].color = hightlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateSkillSelection(int selectedSkill, Skill skill)
    {
        for(int i=0;i<skillTexts.Count; i++)
        {
            if (i == selectedSkill)
                skillTexts[i].color = hightlightedColor;
            else
                skillTexts[i].color = Color.black;
        }

        ppText.text = $"PP {skill.PP}/{skill.Base.PP}";
        typeText.text = skill.Base.Type.ToString();
    }

    public void SetSkillNames(List<Skill> skills)
    {
        for ( int i=0; i < skillTexts.Count; i++)
        {
            if (i < skills.Count)
                skillTexts[i].text = skills[i].Base.Name;
            else
                skillTexts[i].text = "-";
        }
    }
}
