public static class PlayerMovementState
{
    public static bool IsMoving { get; private set; }

    public static bool TryBeginMove()
    {
        if (IsMoving) return false;
        IsMoving = true;
        return true;
    }

    public static void EndMove()
    {
        IsMoving = false;
    }
}
