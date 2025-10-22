public interface ITestable
{
    public bool enabledForTesting { get; }
    public void UpdateWithDelta(float delta);
    public T As<T>() where T : class {
        return this as T;
    }
}