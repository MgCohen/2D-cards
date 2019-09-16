using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayZone : Dropper
{
    public override void OnDrop(GameObject Card)
    {
        Debug.Log(Card.GetComponent<CardBody>().name);
    }
}
