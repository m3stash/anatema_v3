using System.Collections.Generic;

namespace RoomUI {
    public class RoomUIModel {
        private string name;
        private string shape;
        private string biome;
        private string difficulty;
        private int id;

        private List<GridElementModel> topLayer = new List<GridElementModel>();
        private List<GridElementModel> bottomLayer = new List<GridElementModel>();
        private List<GridElementModel> middleLayer = new List<GridElementModel>();

        public RoomUIModel(string name, string shape, string biome, string difficulty, int id, List<GridElementModel> topLayer, List<GridElementModel> bottomLayer, List<GridElementModel> middleLayer) {
            this.name = name;
            this.shape = shape;
            this.biome = biome;
            this.difficulty = difficulty;
            this.id = id;
            this.topLayer = topLayer;
            this.bottomLayer = bottomLayer;
            this.middleLayer = middleLayer;
        }

        public string Name { get => name; set => name = value; }
        public string Shape { get => shape; }
        public string Difficulty { get => difficulty; }
        public string Biome { get => biome; }
        public int Id { get => id; set => id = value; }
        public List<GridElementModel> TopLayer { get => topLayer; }
        public List<GridElementModel> BottomLayer { get => bottomLayer; }
        public List<GridElementModel> MiddleLayer { get => middleLayer; }
    }
}