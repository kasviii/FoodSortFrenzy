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
        // Pick up
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = GetMouseWorld();
            if (GetComponent<Collider2D>().OverlapPoint(mouseWorld))
            {
                draggedSlot = this;
                draggedOriginalPos = transform.position;
            }
        }

        // Drag
        if (Input.GetMouseButton(0) && draggedSlot == this)
        {
            Vector3 m = GetMouseWorld3D();
            transform.position = m;
        }

        // Drop
        if (Input.GetMouseButtonUp(0) && draggedSlot == this)
        {
            Vector2 mouseWorld = GetMouseWorld();
            FoodSlot target = FindTarget(mouseWorld);

            if (target != null && target != this)
            {
                Vector3 posA = draggedOriginalPos;
                Vector3 posB = target.originalPosition;
                string typeA = this.foodType;
                string typeB = target.foodType;

                // Destroy both
                Destroy(this.gameObject);
                Destroy(target.gameObject);

                // Respawn with swapped data
                FindObjectOfType<GameBoard>().SpawnSlot(typeB, this.row, this.col, posA);
                FindObjectOfType<GameBoard>().SpawnSlot(typeA, target.row, target.col, posB);

                FindObjectOfType<GameBoard>().CheckWinCondition();

                draggedSlot = null;
                return;
            }
            else
            {
                // Snap back to original
                this.transform.position = draggedOriginalPos;
            }

            draggedSlot = null;
        }
    }

    FoodSlot FindTarget(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.5f);
        Debug.Log($"Drop pos: {pos} | Hits found: {hits.Length}");
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