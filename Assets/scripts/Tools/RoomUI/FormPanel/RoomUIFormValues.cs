namespace RoomUI {
    public class RoomUIFormValues {
        private string name;
        private string shape;
        private string difficulty;
        private string biome;

        public RoomUIFormValues(string name, string shape, string difficulty, string biome) {
            this.name = name;
            this.biome = biome;
            this.shape = shape;
            this.difficulty = difficulty;
        }

        public string Name { get => name; }
        public string Shape { get => shape; }
        public string Difficulty { get => difficulty; }
        public string Biome { get => biome; }
    }
}
