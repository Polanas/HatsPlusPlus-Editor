namespace HatsPlusPlusEditor;

static class Engine
{
    public static float DeltaTime { get; private set; }

    public static void Init(float deltaTime)
    {
        DeltaTime = deltaTime;
    }
}