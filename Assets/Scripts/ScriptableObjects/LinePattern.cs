using UnityEngine;

[CreateAssetMenu(fileName = "NewLinePattern", menuName = "SlotMachine/Line Pattern")]
public class LinePattern : ScriptableObject
{
    [Tooltip("Row index per reel (0 = top, 1 = middle, 2 = bottom)")]
    [Range(0, 2)]
    public int[] rows = new int[5];

    [Tooltip("Optional name for debugging")]
    public string patternName;
}

