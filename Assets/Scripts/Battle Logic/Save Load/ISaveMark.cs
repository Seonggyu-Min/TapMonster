public interface ISaveMark
{
    void MarkDirty(SaveDirtyFlags flags);
    void RequestSave();
}
