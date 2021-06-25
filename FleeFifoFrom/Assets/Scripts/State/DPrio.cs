public class DPrio
{
    public enum PrioValue
    {
        High = 3,
        Medium = 2,
        Low = 1
    }

    public Observable<PrioValue> Value;

    public bool IsIncreasable
    {
        get => (int)Value.Current < (int)PrioValue.High;
    }

    public bool IsDecreasable
    {
        get => (int)Value.Current > (int)PrioValue.Low;
    }

    public DPrio(PrioValue initValue)
    {
        Value = new Observable<PrioValue>(initValue);
    }

    public void Increase()
    {
        if (IsIncreasable)
            Value.Current++;
    }

    public void Decrease()
    {
        if (IsDecreasable)
            Value.Current--;
    }
}
