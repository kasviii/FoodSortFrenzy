using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public GameObject foodSlotPrefab;
    public Transform boardParent;

    public LevelData currentLevel;

    private Dictionary<string, Color> foodColors = new Dictionary<string, Color>()
    {
        { "F01", new Color(0.8f, 0.3f, 0.1f) },
        { "F02", new Color(1f, 0.9f, 0.1f) },
        { "F03", new Color(0.2f, 0.2f, 0.2f) },
        { "F04", new Color(0.7f, 0.2f, 0.2f) },
        { "F05", new Color(0.2f, 0.7f, 0.2f) },
        { "F06", new Color(0.8f, 0.7f, 0.5f) },
        { "D01", new Color(1f, 0.4f, 0.7f) },
        { "D02", new Color(0.9f, 0.9f, 1f) },
        { "D03", new Color(0.6f, 0.3f, 0.8f) },
        { "D04", new Color(0.4f, 0.8f, 0.6f) },
        { "D05", new Color(1f, 0.5f, 0.2f) },
        { "D06", new Color(0.9f, 0.2f, 0.3f) },
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

    public void BuildBoard()
    {
        for (int i = boardParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(boardParent.GetChild(i).gameObject);

        for (int r = 0; r < currentLevel.rows; r++)
        {
            for (int s = 0; s < currentLevel.slots_per_row; s++)
            {
                string foodType = currentLevel.board_layout[r].slots[s];
                Vector3 pos = new Vector3(s * 1.2f, -r * 1.2f, 0);
                SpawnSlot(foodType, r, s, pos);
            }
        }

        Debug.Log("Board built successfully.");
    }

    public void SpawnSlot(string foodType, int row, int col, Vector3 pos)
    {
        GameObject slot = Instantiate(foodSlotPrefab, pos, Quaternion.identity, boardParent);
        slot.name = $"Slot_{row}_{col}_{foodType}";

        SpriteRenderer sr = slot.GetComponent<SpriteRenderer>();
        if (sr != null && foodColors.ContainsKey(foodType))
            sr.color = foodColors[foodType];

        FoodSlot fs = slot.GetComponent<FoodSlot>();
        fs.foodType = foodType;
        fs.row = row;
        fs.col = col;
    }

    public Color GetFoodColor(string foodType)
    {
        if (foodColors.ContainsKey(foodType))
            return foodColors[foodType];
        return Color.white;
    }

    public void CheckWinCondition()
    {
        int clearedRows = 0;
        int totalRows = currentLevel.rows;

        for (int r = 0; r < totalRows; r++)
        {
            List<FoodSlot> rowSlots = new List<FoodSlot>();

            foreach (Transform child in boardParent)
            {
                FoodSlot fs = child.GetComponent<FoodSlot>();
                if (fs != null && fs.row == r)
                    rowSlots.Add(fs);
            }

            if (rowSlots.Count != currentLevel.slots_per_row)
            {
                Debug.Log($"Row {r} skipped - slot count: {rowSlots.Count}");
                continue;
            }

            string firstType = rowSlots[0].foodType;
            bool rowComplete = true;

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
                Debug.Log($"Row {r} CLEARED!");
            }
        }

        if (clearedRows == totalRows)
            Debug.Log("LEVEL COMPLETE!");
    }
}