namespace SgHook
{
    public interface ISgHookBase
    {
        bool IsEnabled();
        void InitConfig();
        void Run();
    }
}
