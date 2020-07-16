using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public sealed class InvItem : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform canvasGUI;
    public Transform canvasWorld;
    public Inventory inventory;

    private Transform originalParent;
    private Vector3 startPosition;
    private bool canThrought = false;
    private bool inInventory = false;
    private Transform player;
    private Animator animator;

    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = player.GetComponent<Animator>();
    }
   
    public Animator getAnimator()
    {
        return animator;
    }

    public Vector3 GetStartPosition()
    {
        if (startPosition[1] == 0)
        {
            startPosition[1] = 10;
        }
        return startPosition;
    }

    public void SetInInventory(bool value)
    {
        this.inInventory = value;
    }

    public void SetCanThrought(bool value)
    {
        this.canThrought = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this.inInventory)
        {
            startPosition = transform.localPosition;
            originalParent = transform.parent;
            transform.SetParent(canvasGUI, false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (this.inInventory)
        {
            this.transform.position = eventData.pressEventCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.inInventory)
        {
            var inv = GetWorldCorners(GameObject.FindGameObjectWithTag("Inventory").transform.GetComponent<RectTransform>());
            var canvas = GetWorldCorners(GameObject.FindGameObjectWithTag("GUI_Canvas").transform.GetComponent<RectTransform>());
            var trans = GetWorldCorners(transform.GetComponent<RectTransform>());

            float invXL = inv[0].x;
            float invYT = inv[1].y;
            float invYB = inv[3].y;

            float transXL = trans[0].x;
            float transXR = trans[2].x;
            float transYT = trans[1].y;
            float transYB = trans[3].y;

            float canvasXL = canvas[0].x;
            float canvasYT = canvas[1].y;
            float canvasYB = canvas[3].y;

            if ((transXR <= invXL || transYB >= invYT || transYT <= invYB) &&
                (canvasYT >= transYT && canvasXL <= transXL && canvasYB <= transYB))
            {
                this.canThrought = true;
            }
            else
            {
                this.canThrought = false;
            }

            if (this.canThrought)
            {
                transform.SetParent(canvasWorld, false);
                this.GetComponentInChildren<Text>().fontSize = 1;
                inventory.objectsCount--;

                this.transform.position = eventData.pressEventCamera.ScreenToWorldPoint(Input.mousePosition);

                this.inInventory = false;
            }

            else
            {
                this.transform.SetParent(originalParent, false);
                this.transform.localPosition = startPosition;
            }
        }
    }

    Vector3[] GetWorldCorners(RectTransform rt)
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        return v;
    }

    public void OnClick()
    {
        if (!inInventory)
        {
            TakeObject();
        }
    }

    public void TakeObject()
    {
        //определяет положение персонажа
        int lastPlayerX = Mathf.RoundToInt(player.position.x);
        int lastPlayerY = Mathf.RoundToInt(player.position.y);

        //определяет положение объекта
        int thingX = Mathf.RoundToInt(transform.position.x);
        int thingY = Mathf.RoundToInt(transform.position.y);

        float objPosX = gameObject.transform.position.x;
        float objPosY = gameObject.transform.position.y;

        float playerPosX = player.transform.position.x;
        float playerPosY = player.transform.position.y;

        float distance = Mathf.Sqrt((objPosX - playerPosX) * (objPosX - playerPosX) +
            (objPosY - playerPosY) * (objPosY - playerPosY));

        if (distance <= 2)
        {
            StartCoroutine(TakeAnimation(animator));
        }
    }

    public IEnumerator TakeAnimation(Animator animator)
    {
        animator.SetBool("needToTake", true);

        yield return new WaitForSeconds(0.3f);

        animator.SetBool("isTaked", true);
        animator.SetBool("needToTake", false);

        inventory.AddItemIfPossible(this);
    }
}
