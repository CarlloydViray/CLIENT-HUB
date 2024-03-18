namespace BPOI_HUB.modules.core
{
    public static class ConfigHelper
    {
        private static IConfiguration? config;

        public static IConfiguration? Config { get => config; set => config = value; }
    }
}
