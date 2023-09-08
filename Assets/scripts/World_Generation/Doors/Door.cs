using System.Collections.Generic;
using UnityEngine;

namespace DoorNs { 
    public class Door {

        private DirectionalEnum direction;
        private Vector3 localPosition;
        private Vector3 doorNeighbor;
        private BiomeEnum biome;

        public Door(Vector3 localPosition, DirectionalEnum direction, BiomeEnum biome) {
            this.localPosition = localPosition;
            this.direction = direction;
            this.biome = biome;
        }

        public Vector3 LocalPosition => localPosition;

        public Vector3 DoorNeighbor {
            get { return doorNeighbor; }
            set { doorNeighbor = value; }
        }

        public DirectionalEnum GetDirection() {
            return direction;
        }

    }
}