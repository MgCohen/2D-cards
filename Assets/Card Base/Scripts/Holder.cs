using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Holder : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;

    public Transform cardHolder;

    public GameEvent cardDrag;
    public GameEvent cardDrop;

    public List<CardBody> cards = new List<CardBody>();

    private void OnEnable()
    {
        cardDrag.Register(SetSpaces);
        cardDrop.Register(SetSpaces);
        SetSpaces();
    }
    private void OnDisable()
    {
        cardDrag.Unregister(SetSpaces);
        cardDrop.Unregister(SetSpaces);
    }

    public Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return a + (b - a) * t;
    }
    public Vector3 QuadraticCurve(float t)
    {
        Vector3 p0 = Lerp(pointA.position, pointB.position, t);
        Vector3 p1 = Lerp(pointB.position, pointC.position, t);
        return Lerp(p0, p1, t);
    }

    public void SetSpaces()
    {
        cards.Clear();
        foreach (Transform child in cardHolder)
        {
            var body = child.GetComponent<CardBody>();
            if (body && child.gameObject.activeInHierarchy)
            {
                cards.Add(body);
            }
        }
        int amount = cards.Count;
        float spacing = 1f / amount;
        for (int i = 0; i < amount; i++)
        {
            var pos = QuadraticCurve((spacing / 2) + spacing * i);
            if (cards[i])
                if (cards[i].isDummy)
                {
                    cards[i].transform.position = pos;
                }
                else
                {
                    cards[i].MoveTo(pos);
                }
        }
    }

    [Header("Draw Options")]
    public DrawType drawType;
    public enum DrawType { Instant, Up, Follow, Custom };
    public Transform DrawSpot;
    public Transform FlipPoint;
    public GameObject cardPrefab;
    public float flipTime = 0.75f;

    public bool drawing = false;

    public void Draw(GameObject Card, int amount = 1)
    {
        if (drawType == DrawType.Follow)
        {
            drawing = true;
            amount -= 1;
            var body = Card.GetComponent<CardBody>();
            Card.transform.position = DrawSpot.position;
            Card.transform.rotation = DrawSpot.rotation;
            Card.transform.localScale = DrawSpot.localScale;
            body.Draw(cardHolder, flipTime);
            Card.transform.DOMove(FlipPoint.position, flipTime);
            Card.transform.DOLocalRotate(Vector3.zero, flipTime);
            Card.transform.DOScale(1, flipTime);
            if (amount == 0)
            {
                ActionDelayer.DelayAction(() => drawing = false, flipTime);
            }
            ActionDelayer.DelayAction(SetSpaces, (flipTime * 2));
            if (amount >= 1)
            {
                ActionDelayer.DelayAction(() => { Draw(amount); }, flipTime);
            }
        }
        else if (drawType == DrawType.Instant)
        {
            amount -= 1;
            var body = Card.GetComponent<CardBody>();
            Card.transform.position = cardHolder.transform.position;
            Card.transform.SetParent(cardHolder);
            SetSpaces();
            if (amount >= 1)
            {
                ActionDelayer.DelayAction(() => { Draw(amount); }, flipTime);
            }
        }
        else if (drawType == DrawType.Up)
        {
            flipTime = 0f;
            drawing = true;
            amount -= 1;
            var body = Card.GetComponent<CardBody>();
            body.Draw(cardHolder, flipTime, Ease.OutBack);
            Card.transform.position = body.dummy.transform.position + Vector3.down * 3;
            Card.transform.rotation = Quaternion.identity;
            Card.transform.localScale = Vector3.one;
            if (amount == 0)
            {
                ActionDelayer.DelayAction(() => drawing = false, flipTime);
            }
            ActionDelayer.DelayAction(SetSpaces, (flipTime * 2));
            if (amount >= 1)
            {
                ActionDelayer.DelayAction(() => { Draw(amount); }, flipTime);
            }
        }
        else if (drawType == DrawType.Custom)
        {
            //define here
        }
    }

    [ContextMenu("Draw")]
    public void TestDraw()
    {
        Draw();
    }

    public void Draw(int amount = 1)
    {
        if (drawing)
        {
            return;
        }
        var obj = Instantiate(cardPrefab);
        Draw(obj, amount);
    }

    [Header("Discard Options")]
    public Transform finalPoint;
    public enum DiscardType { Instant, Down, Follow, Custom };
    public DiscardType discardType;
    public float discardTime;
    public float delayBetweenDiscards;

    public void Discard(GameObject card, bool setSpaces = true)
    {
        if (discardType == DiscardType.Instant)
        {
            card.SetActive(false);
            Destroy(card);
            if (setSpaces)
                SetSpaces();
        }
        else if (discardType == DiscardType.Follow)
        {
            if (setSpaces) ActionDelayer.DelayAction(SetSpaces, discardTime / 2);
            card.transform.SetParent(null);
            card.transform.DOMove(finalPoint.position, discardTime).OnComplete(() => { card.SetActive(false); Destroy(card); });
            card.transform.DORotate(finalPoint.rotation.eulerAngles, discardTime);
            card.transform.DOScale(finalPoint.localScale, discardTime);
        }
        else if (discardType == DiscardType.Down)
        {
            card.transform.DOMoveY(card.transform.position.y - 3f, discardTime).OnComplete(() => { card.SetActive(false); /*Destroy(card);*/if (setSpaces) SetSpaces(); }).SetEase(Ease.InBack);

        }
        else if (discardType == DiscardType.Custom)
        {
            //define custom draw here
        }
    }

    [ContextMenu("Discard")]
    private void Discard()
    {
        Discard(cards[0].gameObject);
    }

    [ContextMenu("Discard Many")]
    private void DiscardMany()
    {
        List<CardBody> newCards = new List<CardBody>();
        SetSpaces();
        newCards.Add(cards[0]);
        newCards.Add(cards[2]);
        newCards.Add(cards[3]);
        Discard(newCards);
    }

    public void Discard(CardBody card)
    {
        Discard(card.gameObject);
    }

    public void Discard(List<GameObject> newcards)
    {
        newcards = newcards.OrderBy(x => x.transform.GetSiblingIndex()).ToList();
        for (int i = 0; i < newcards.Count; i++)
        {
            //Discard(newcards[i]);
            if (i > 0 && i < newcards.Count - 1)
            {
                var obj = newcards[i];
                ActionDelayer.DelayAction(() => { Discard(obj, false); }, i * delayBetweenDiscards);
            }
            else if (i == 0)
            {
                Discard(newcards[i], false);
            }
            else
            {
                var obj = newcards[i];
                ActionDelayer.DelayAction(() => { Discard(obj, true); }, i * delayBetweenDiscards);
            }
        }
    }

    public void Discard(List<CardBody> newcards)
    {
        List<GameObject> cardObjs = new List<GameObject>();
        foreach (var card in newcards)
        {
            cardObjs.Add(card.gameObject);
        }
        Discard(cardObjs);
    }
}
