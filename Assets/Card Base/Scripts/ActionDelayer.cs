using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionDelayer : MonoBehaviour
{

    private static ActionDelayer mDelayer;
    public static ActionDelayer Delayer
    {
        get
        {
            if (!mDelayer)
            {
                var dummy = new GameObject("Delayer");
                mDelayer = dummy.AddComponent<ActionDelayer>();
            }
            return mDelayer;
        }
    }

    public static void DelayAction(UnityAction act, float time)
    {
        Delayer.StartCoroutine(DelayCO(act, time));
    }

    static IEnumerator DelayCO(UnityAction act, float time)
    {
        yield return new WaitForSeconds(time);
        act.Invoke();
    }
}
