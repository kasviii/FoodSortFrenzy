using UnityEngine;

public class FoodSlot : MonoBehaviour
{
    public string foodType;
    public int row;
    public int col;

    private Vector3 originalPosition;
    private static FoodSlot draggedSlot = null;
    private static Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log("Mouse clicked at: " + Input.mousePosition);
        {
            Vector3 mouse3D = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCam.transform.position.z));
            Vector2 mousePos = new Vector2(mouse3D.x, mouse3D.y);
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                draggedSlot = this;
                originalPosition = transform.position;
            }
        }

        if (Input.GetMouseButton(0) && draggedSlot == this)
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCam.transform.position.z));
            mousePos.z = 0;
            transform.position = mousePos;
        }

        if (Input.GetMouseButtonUp(0) && draggedSlot == this)
        {
            Vector3 mouse3D = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCam.transform.position.z));
            Vector2 mousePos = new Vector2(mouse3D.x, mouse3D.y);
            Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, 0.4f);

            FoodSlot target = null;
            foreach (var hit in hits)
            {
                FoodSlot other = hit.GetComponent<FoodSlot>();
                if (other != null && other != this)
                {
                    target = other;
                    break;
                }
            }

            if (target != null)
            {
                // Swap colors
                SpriteRenderer mySR = GetComponent<SpriteRenderer>();
                SpriteRenderer targetSR = target.GetComponent<SpriteRenderer>();
                Color tempColor = targetSR.color;
                targetSR.color = mySR.color;
                mySR.color = tempColor;

                // Swap food type data
                string tempType = target.foodType;
                target.foodType = this.foodType;
                this.foodType = tempType;

                // Snap both to their grid positions
                transform.position = target.originalPosition;
                this.originalPosition = target.originalPosition;

                target.transform.position = originalPosition;
                target.originalPosition = originalPosition;
            }
            else
            {
                transform.position = originalPosition;
            }

            draggedSlot = null;
        }
    }
}