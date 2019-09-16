using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInteraction : MonoBehaviour
{

    public GameEvent CardClick;
    public GameEvent CardHover;
    public GameEvent CardDrag;

    public virtual void OnClick()
    {
        CardClick.Raise(true);
    }

    public virtual void OnHover()
    {
        CardHover.Raise();
    }

    public virtual void OnEndHover()
    {
        CardHover.Close();
    }

    public virtual void OnDrag()
    {
        CardDrag.Raise();
    }

    public virtual void OnEndDrag()
    {
        CardDrag.Close();
    }
}
