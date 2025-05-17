using UnityEngine;

[CreateAssetMenu(fileName = "SymbolPayoutTable", menuName = "SlotMachine/Symbol Payout Table")]
public class SymbolPayoutTable : ScriptableObject
{
    [SerializeField] private SymbolPayout[] symbols;

    public SymbolPayout[] Symbols { get => symbols; }

    public int GetPayout(Sprite symbol, int matchCount)
    {
        foreach (var entry in symbols)
        {
            if (entry.symbol == symbol)
            {
                matchCount = Mathf.Clamp(matchCount, 1, 5);
                return entry.payouts[matchCount - 1];
            }
        }
        return 0;
    }

    [System.Serializable]
    public class SymbolPayout
    {
        public Sprite symbol;
        public int[] payouts = new int[5]; // PAYOUT[0] = 1 SYMBOL, PAYOUT[1] = 2 SYMBOL,etc.
    }

}
