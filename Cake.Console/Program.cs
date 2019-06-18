namespace Cake.Console
{
    public class Program
    {
        private static readonly Core.Main Constant = GetInstance()._cake;

        private static void Main() => Constant.StartAsync().GetAwaiter().GetResult();

        private static Program _instance; // Singleton instance

        private Core.Main _cake;

        public static Program GetInstance() // Singleton pattern
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new Program();
            _instance.Initialize();

            return _instance;
        }

        public void Initialize()
        {
            _cake = new Core.Main();
        }
    }
}
