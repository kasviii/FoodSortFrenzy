using UnityEngine;

public class FoodSlot : MonoBehaviour
{
    public string foodType;
    public int row;
    public int col;

    private Vector3 originalPosition;
    private static FoodSlot draggedSlot = null;
    private static Vector3 draggedOriginalPos;
    private static Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = GetMouseWorld();
            if (GetComponent<Collider2D>().OverlapPoint(mouseWorld))
            {
                draggedSlot = this;
                draggedOriginalPos = transform.position;
            }
        }

        if (Input.GetMouseButton(0) && draggedSlot == this)
        {
            transform.position = GetMouseWorld3D();
        }

        if (Input.GetMouseButtonUp(0) && draggedSlot == this)
        {
            Vector2 mouseWorld = GetMouseWorld();
            FoodSlot target = FindTarget(mouseWorld);

            if (target != null && target != this)
            {
                // Store all data before swap
                string typeA = this.foodType;
                string typeB = target.foodType;
                Color colorA = this.GetComponent<SpriteRenderer>().color;
                Color colorB = target.GetComponent<SpriteRenderer>().color;

                // Swap food types and colors IN PLACE - no destroy/respawn
                this.foodType = typeB;
                this.GetComponent<SpriteRenderer>().color = colorB;

                target.foodType = typeA;
                target.GetComponent<SpriteRenderer>().color = colorA;

                // Snap both back to their original grid positions
                this.transform.position = draggedOriginalPos;
                target.transform.position = target.originalPosition;

                GameBoard board = FindObjectOfType<GameBoard>();
                board.CheckWinCondition();
                GameManager.Instance.UseMove();
            }
            else
            {
                this.transform.position = draggedOriginalPos;
            }

            draggedSlot = null;
        }
    }

    FoodSlot FindTarget(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.5f);
        foreach (var hit in hits)
        {
            FoodSlot fs = hit.GetComponent<FoodSlot>();
            if (fs != null && fs != this)
                return fs;
        }
        return null;
    }

    Vector2 GetMouseWorld()
    {
        Vector3 m = mainCam.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            Mathf.Abs(mainCam.transform.position.z)));
        return new Vector2(m.x, m.y);
    }

    Vector3 GetMouseWorld3D()
    {
        Vector3 m = mainCam.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            Mathf.Abs(mainCam.transform.position.z)));
        m.z = 0;
        return m;
    }
}