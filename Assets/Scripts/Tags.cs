namespace Assets.Scripts
{
    public static class Tags
    {
        public static string Player = "Player";
        public static string GravityField = "GravityField";
        public static string Interactable = "Interactable";
        public static string Mirror = "Mirror";
        public static string WorldBounds = "WorldBounds";


        public static class Layers
        {
            public static int Default = UnityEngine.LayerMask.NameToLayer("Default");
            public static int Interaction = UnityEngine.LayerMask.NameToLayer("Interaction");
        }

        public static class LayerMask
        {
            public static int Default = UnityEngine.LayerMask.GetMask("Default");
            public static int Interaction = UnityEngine.LayerMask.GetMask("Interaction");
        }
    }
}
