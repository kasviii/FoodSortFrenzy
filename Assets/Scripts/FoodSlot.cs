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
            Vector3 m = GetMouseWorld3D();
            transform.position = m;
        }

        if (Input.GetMouseButtonUp(0) && draggedSlot == this)
        {
            Vector2 mouseWorld = GetMouseWorld();
            FoodSlot target = FindTarget(mouseWorld);

            if (target != null && target != this)
            {
                int rowA = this.row;
                int colA = this.col;
                int rowB = target.row;
                int colB = target.col;
                string typeA = this.foodType;
                string typeB = target.foodType;
                Vector3 posA = draggedOriginalPos;
                Vector3 posB = target.originalPosition;

                GameBoard board = FindObjectOfType<GameBoard>();

                Destroy(this.gameObject);
                Destroy(target.gameObject);

                board.SpawnSlot(typeB, rowA, colA, posA);
                board.SpawnSlot(typeA, rowB, colB, posB);

                board.Invoke("CheckWinCondition", 0.1f);
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