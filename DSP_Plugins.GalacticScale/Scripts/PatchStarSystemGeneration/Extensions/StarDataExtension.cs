namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public static class StarDataExtension {
        public static bool IsStartingStar(this StarData star) {
            return star.index == 0;
        }
    }
}