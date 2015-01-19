namespace Base_Tools45
{
    public class Templates
    {
        public class Singleton
        {
            private static readonly Singleton _singleton = new Singleton();
            private readonly string version;

            private Singleton()
            {
                version = "1.0";
            }

            public static Singleton singleton
            {
                get
                {
                    return _singleton;
                }
            }

            public string Version
            {
                get
                {
                    return version;
                }
            }
        }
    }
}
