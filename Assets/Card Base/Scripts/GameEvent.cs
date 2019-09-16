using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Event", menuName = "Event")]
public class GameEvent : ScriptableObject
{
    private UnityEvent uEventPlay = new UnityEvent(); //called when event started
    private UnityEvent uEventClose = new UnityEvent(); //called when event finished
    private UnityEvent uEventFinal = new UnityEvent(); //called when playingCount returns to zero

    private int PlayingCount = 0; //number of instances of this event currently happening( or happened if never closed)

    private void OnDisable()
    {
        PlayingCount = 0;
        uEventClose.RemoveAllListeners();
        uEventFinal.RemoveAllListeners();
        uEventPlay.RemoveAllListeners();
    }

    public void Register(UnityAction act) //register to standard Play event
    {
        uEventPlay.AddListener(act);
    }

    public void Unregister(UnityAction act)//unregister to standard Play event
    {
        uEventPlay.RemoveListener(act);
    } 

    public void RegisterClosure(UnityAction act)//register to action finished event
    {
        uEventClose.AddListener(act);
    } 

    public void UnregisterClosure(UnityAction act) //unregister to action finished event
    {
        uEventClose.RemoveListener(act);
    }

    public void RegisterFinal(UnityAction act) //register to all instances of this event finishing
    {
        uEventFinal.AddListener(act);
    }

    public void UnregisterFinal(UnityAction act) //unregister to all instances of this event finishing
    {
        uEventFinal.RemoveListener(act);
    }

    public void RegisterAll(UnityAction act)//register to all events
    {
        uEventClose.AddListener(act);
        uEventPlay.AddListener(act);
        uEventFinal.AddListener(act);
    } 

    public void UnregisterAll(UnityAction act) //register to all events
    {
        uEventClose.RemoveListener(act);
        uEventPlay.RemoveListener(act);
        uEventFinal.AddListener(act);
    }

    public void Raise(bool CloseImmediately = false) //invoke event - uEventPlay
    {
        PlayingCount += 1;
        uEventPlay.Invoke();
        if (CloseImmediately)
        {
            uEventClose.Invoke();
        }
    }

    public void Close() //close event - uEventClose e tenta chamar uEventFinal
    {
        PlayingCount -= 1;
        uEventClose.Invoke();
        if(PlayingCount <= 0)
        {
            uEventFinal.Invoke();
        }
    }

    public void CloseAll()
    {
        PlayingCount = 0;
        uEventClose.Invoke();
        uEventFinal.Invoke();
    }

    public int InstanceCount() //retorna quantidade de instancias do evento
    {
        return PlayingCount;
    }

    public bool IsPlaying() //is there is a open event
    {
        if(PlayingCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
