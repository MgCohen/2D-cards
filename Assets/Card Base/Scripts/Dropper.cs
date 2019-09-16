using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dropper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameEvent cardDrop;

    public Holder holder;

    public bool getDummy = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventData.dragging)
        {
            return;
        }
        var obj = eventData.pointerDrag;
        var card = obj.GetComponent<CardBody>();
        if (card)
        {
            SetDrop(card);
        }
    }

    public void SetDrop(CardBody card)
    {
        card.SetDrop(this);
        if (card.dummy && getDummy)
        {
            card.dummy.transform.SetParent(transform);
            card.dummy.SetActive(true);
            if (holder)
            {
                holder.SetSpaces();
            }
        }
    }

    public void ReleaseDrop(CardBody card)
    {
        card.ExitDrop(this);
        if (card.dummy && getDummy)
        {
            card.dummy.SetActive(false);
            if (holder)
            {
                holder.SetSpaces();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.dragging)
        {
            return;
        }
        var obj = eventData.pointerDrag;
        var card = obj.GetComponent<CardBody>();
        if (card && card.dropper == this)
        {
            ReleaseDrop(card);
        }
    }

    public virtual void OnDrop(GameObject Card)
    {
        Debug.Log(Card.name + " Was Dropped on " + gameObject.name);
    }
}
