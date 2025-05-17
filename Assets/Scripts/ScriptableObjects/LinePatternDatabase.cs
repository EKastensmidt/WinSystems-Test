using UnityEngine;

[CreateAssetMenu(fileName = "LinePatternDatabase", menuName = "SlotMachine/Line Pattern Database")]
public class LinePatternDatabase : ScriptableObject
{
    public LinePattern[] patterns;
}
