using UnityEngine;
using System.IO;

public class GameBoard : MonoBehaviour
{
    public GameObject foodSlotPrefab;
    public Transform boardParent;

    private LevelData currentLevel;

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
        for (int r = 0; r < currentLevel.rows; r++)
        {
            for (int s = 0; s < currentLevel.slots_per_row; s++)
            {
                string foodType = currentLevel.board_layout[r].slots[s];

                // Position each slot in a grid
                Vector3 pos = new Vector3(s * 1.2f, -r * 1.2f, 0);

                GameObject slot = Instantiate(foodSlotPrefab, pos, Quaternion.identity, boardParent);
                slot.name = $"Slot_{r}_{s}_{foodType}";
            }
        }

        Debug.Log("Board built successfully.");
    }
}