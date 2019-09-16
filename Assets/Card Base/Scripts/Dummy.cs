using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dummy : MonoBehaviour
{
    public GameEvent cardDrag;
    // Update is called once per frame


    void Update()
    {
        
        var posX = CardBody.dragged.transform.position.x;
        var holder = transform.parent.GetComponent<Dropper>().holder;

        if (holder)
        {
            var cards = holder.cards;
            cards = cards.OrderBy(x => x.xValue).ToList();
            foreach (var card in cards)
            {
                card.transform.SetSiblingIndex(cards.IndexOf(card));
            }
        }
        cardDrag.Raise();
    }
}
