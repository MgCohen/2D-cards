using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using DG.Tweening;

public class CardBody : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static CardBody dragged;

    public GameEvent cardDrag;
    public GameEvent cardDrop;

    public List<Dropper> currentDroppers = new List<Dropper>();

    public Dropper dropper;
    private Dropper lastDropper;

    public CardInteraction interaction;

    public float xValue
    {
        get
        {
            if (isDummy)
            {
                return originCard.transform.localPosition.x;
            }
            else
            {
                return transform.localPosition.x;
            }
        }
    }

    [Header("Dummy")]
    public bool isDummy = false;
    public GameObject dummyPrefab;
    public GameObject fixedDummyPrefab;
    public GameObject dummy;
    public CardBody originCard;


    [HideInInspector] public BoxCollider2D box;

    private void OnEnable()
    {
        box = GetComponent<BoxCollider2D>();
        if (!isDummy)
        {
            cardDrop.Register(SetBox);
            cardDrag.Register(ResetBox);
        }
    }

    private void OnDisable()
    {
        if (!isDummy)
        {
            cardDrop.Unregister(SetBox);
            cardDrag.Unregister(ResetBox);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetDummy();
        var index = transform.GetSiblingIndex();
        dummy.transform.SetSiblingIndex(index);
        dropper = transform.parent.GetComponent<Dropper>();
        dragged = this;
        lastDropper = dropper;
        transform.SetParent(null);
        cardDrag.Raise();
        transform.DOScale(1.25f, 0.2f);
        //sortGroup.sortingOrder = 1;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = -3;
        transform.position = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dropper == null)
        {
            dropper = lastDropper;
        }
        transform.DOScale(1, 0.2f);
        dragged = null;
        transform.SetParent(dropper.transform);
        if (dummy)
        {
            transform.SetSiblingIndex(dummy.transform.GetSiblingIndex());
            dummy.gameObject.SetActive(false);
            Destroy(dummy);
        }
        cardDrop.Raise();
        dropper.OnDrop(gameObject);
        //sortGroup.sortingOrder = 0;
    }

    public void MoveTo(Vector3 pos)
    {
        transform.DOMove(pos, 0.25f);
    }

    public void Draw(Transform targetHolder, float drawTime = 0.75f, Ease ease = Ease.Linear)
    {
        box.enabled = false;
        SetDummy(targetHolder, true);
        dummy.transform.SetAsLastSibling();
        cardDrop.Raise();
        ActionDelayer.DelayAction(() => { transform.DOMove(dummy.transform.position, 0.5f).SetEase(ease).OnComplete(() => { box.enabled = true; transform.SetParent(dummy.transform.parent); transform.SetSiblingIndex(dummy.transform.GetSiblingIndex()); dummy.gameObject.SetActive(false); Destroy(dummy); }); }, drawTime);
    }

    public void SetDummy(Transform targetHolder = null, bool isFixed = false)
    {
        if (targetHolder == null)
        {
            targetHolder = transform.parent;
        }
        if (isFixed)
        {
            dummy = Instantiate(fixedDummyPrefab);
        }
        else
        {
            dummy = Instantiate(dummyPrefab);
        }
        dummy.GetComponent<CardBody>().originCard = this;
        dummy.transform.SetParent(targetHolder);
    }

    public void SetBox()
    {
        if (box)
            box.enabled = true;
    }

    public void ResetBox()
    {
        if (box && dragged == this)
            box.enabled = false;
    }

    private void OnDestroy()
    {
        var tweens = DOTween.TweensByTarget(transform, true);
        if (tweens != null)
        {
            foreach (var t in tweens)
            {
                t.Kill();
            }
        }
    }

    public void SetDrop(Dropper drop)
    {
        currentDroppers.Add(drop);
        dropper = drop;
    }

    public void ExitDrop(Dropper drop)
    {
        currentDroppers.Remove(drop);
        if (dropper == drop)
        {
            if (currentDroppers.Count > 0)
            {
                currentDroppers[currentDroppers.Count - 1].SetDrop(this);
            }
            else
            {
                lastDropper.SetDrop(this);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        interaction.OnClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        interaction.OnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interaction.OnEndHover();
    }
}
