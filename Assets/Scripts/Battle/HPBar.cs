using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        float curHP = health.transform.localScale.x;
        float changeAmt = curHP - newHP;

        //loop til diff between curHp and newHP is very small
        while(curHP - newHP > Mathf.Epsilon)
        {
            //change a small value of HPBar
            curHP -= changeAmt * Time.deltaTime;

            //change UI
            health.transform.localScale = new Vector3(curHP, 1f);
            yield return null;
        }

        //set newHP
        health.transform.localScale = new Vector3(newHP, 1f);
    }
}
