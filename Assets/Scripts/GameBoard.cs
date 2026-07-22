using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public GameObject foodSlotPrefab;
    public Transform boardParent;

    private LevelData currentLevel;

    // Color map for food types
    private Dictionary<string, Color> foodColors = new Dictionary<string, Color>()
    {
        { "F01", new Color(0.8f, 0.3f, 0.1f) },  // Kebab - orange brown
        { "F02", new Color(1f, 0.9f, 0.1f) },      // Corn - yellow
        { "F03", new Color(0.2f, 0.2f, 0.2f) },    // Burger - dark brown
        { "F04", new Color(0.7f, 0.2f, 0.2f) },    // Sausage - red
        { "F05", new Color(0.2f, 0.7f, 0.2f) },    // Pepper - green
        { "F06", new Color(0.8f, 0.7f, 0.5f) },    // Mushroom - beige
        { "D01", new Color(1f, 0.4f, 0.7f) },      // Donut - pink
        { "D02", new Color(0.9f, 0.9f, 1f) },      // Ice cream - white
        { "D03", new Color(0.6f, 0.3f, 0.8f) },    // Cupcake - purple
        { "D04", new Color(0.4f, 0.8f, 0.6f) },    // Macaron - mint
        { "D05", new Color(1f, 0.5f, 0.2f) },      // Candy - orange
        { "D06", new Color(0.9f, 0.2f, 0.3f) },    // Cake - red
    };

    void Start()
    {
        LoadLevel(1);
    }

    void LoadLevel(int levelId)
    {
        string path = Path.Combine(Application.dataPath, "Levels", $"level_0{levelId}.json");
        if (!File.Exists(path))
        {
            Debug.LogError($"Level file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        currentLevel = JsonUtility.FromJson<LevelData>(json);
        Debug.Log($"Loaded Level {currentLevel.level_id} | Rows: {currentLevel.rows} | Constraint: {currentLevel.constraint}");
        BuildBoard();
    }

    void BuildBoard()
    {
        foreach (Transform child in boardParent)
            Destroy(child.gameObject);

        for (int r = 0; r < currentLevel.rows; r++)
        {
            for (int s = 0; s < currentLevel.slots_per_row; s++)
            {
                string foodType = currentLevel.board_layout[r].slots[s];
                Vector3 pos = new Vector3(s * 1.2f, -r * 1.2f, 0);

                GameObject slot = Instantiate(foodSlotPrefab, pos, Quaternion.identity, boardParent);
                slot.name = $"Slot_{r}_{s}_{foodType}";

                // Set color based on food type
                SpriteRenderer sr = slot.GetComponent<SpriteRenderer>();
                if (sr != null && foodColors.ContainsKey(foodType))
                    sr.color = foodColors[foodType];

                // Add FoodSlot component with data
                FoodSlot fs = slot.AddComponent<FoodSlot>();
                fs.foodType = foodType;
                fs.row = r;
                fs.col = s;
            }
        }

        Debug.Log("Board built successfully.");
    }
    public void CheckWinCondition()
    {
        int clearedRows = 0;

        foreach (Transform rowParent in boardParent)
        {
            // Get all FoodSlot children
        }

        // We'll check per row using slot naming
        bool[,] checked_ = new bool[currentLevel.rows, currentLevel.slots_per_row];

        for (int r = 0; r < currentLevel.rows; r++)
        {
            string firstType = null;
            bool rowComplete = true;
            List<FoodSlot> rowSlots = new List<FoodSlot>();

            foreach (Transform child in boardParent)
            {
                FoodSlot fs = child.GetComponent<FoodSlot>();
                if (fs != null && fs.row == r)
                    rowSlots.Add(fs);
            }

            if (rowSlots.Count == 0) continue;

            firstType = rowSlots[0].foodType;
            foreach (FoodSlot fs in rowSlots)
            {
                if (fs.foodType != firstType)
                {
                    rowComplete = false;
                    break;
                }
            }

            if (rowComplete)
            {
                clearedRows++;
                foreach (FoodSlot fs in rowSlots)
                    fs.GetComponent<SpriteRenderer>().color = Color.white;
                Debug.Log($"Row {r} cleared!");
            }
        }

        if (clearedRows == currentLevel.rows)
            Debug.Log("LEVEL COMPLETE!");
    }
    public void SpawnSlot(string foodType, int row, int col, Vector3 pos)
    {
        GameObject slot = Instantiate(foodSlotPrefab, pos, Quaternion.identity, boardParent);
        slot.name = $"Slot_{row}_{col}_{foodType}";

        SpriteRenderer sr = slot.GetComponent<SpriteRenderer>();
        if (sr != null && foodColors.ContainsKey(foodType))
            sr.color = foodColors[foodType];

        FoodSlot fs = slot.AddComponent<FoodSlot>();
        fs.foodType = foodType;
        fs.row = row;
        fs.col = col;
    }
}
