using System;
using System.Collections.Generic;

public enum CurrencyId { Gold = 1, Gem = 2 }

public class WalletModel
{
    private readonly Dictionary<CurrencyId, BigNumber> _values = new();
    public IReadOnlyDictionary<CurrencyId, BigNumber> Values => _values;

    public event Action<CurrencyId, BigNumber> OnCurrencyChanged;


    public BigNumber Get(CurrencyId id)
        => _values.TryGetValue(id, out BigNumber v) ? v : BigNumber.Zero;

    public void Set(CurrencyId id, BigNumber value)
    {
        _values[id] = value;
        OnCurrencyChanged?.Invoke(id, value);
    }

    public void Add(CurrencyId id, BigNumber delta) => Set(id, Get(id) + delta);
}
