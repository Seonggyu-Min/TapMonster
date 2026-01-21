public readonly struct Cost
{
    public readonly CurrencyId Currency;
    public readonly BigNumber Amount;
    // TODO: 할인율 추가

    public Cost(CurrencyId currency, BigNumber amount)
    {
        Currency = currency;
        Amount = amount;
    }

    public override string ToString()
    {
        return $"Currency: {Currency.ToString()} / Amount: {BigNumberFormatter.ToString(Amount)}"; 
    }
}
