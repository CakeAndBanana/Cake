namespace Cake.Console
{
    public class Program
    {
        private static readonly Core.Cake Constant = GetInstance().Cake;

        private static void Main() => Constant.StartAsync().GetAwaiter().GetResult();

        private static Program _instance; // Singleton instance

        public Core.Cake Cake;

        public static Program GetInstance() // Singleton pattern
        {
            if (_instance != null) return _instance;
            _instance = new Program();
            _instance.Initialize();

            return _instance;
        }

        public void Initialize()
        {
            Cake = new Core.Cake();
        }
    }
}
