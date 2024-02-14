namespace RoomUI {
    public class RoomUIFormValues {
        private string name;
        private string shape;
        private string difficulty;
        private string biome;
        private int id;

        public RoomUIFormValues(string name, string shape, string difficulty, string biome, int id) {
            this.name = name;
            this.biome = biome;
            this.shape = shape;
            this.difficulty = difficulty;
            this.id = id;
        }

        public string Name { get => name; }
        public string Shape { get => shape; }
        public string Difficulty { get => difficulty; }
        public string Biome { get => biome; }

        public int Id { get => id; set => id = value; }
    }
}
