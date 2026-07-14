using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int level_id;
    public int episode;
    public string constraint; // "time" or "moves"
    public int time_limit;
    public int move_limit;
    public int rows;
    public int slots_per_row;
    public List<string> food_types;
    public List<Row> board_layout;
}

[System.Serializable]
public class Row
{
    public List<string> slots;
}