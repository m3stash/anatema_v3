using System.Collections.Generic;
using UnityEngine;

namespace DoorNs { 
    public class PseudoDoor {

        private DirectionalEnum direction;
        private Vector3 localPosition;
        private Vector3 doorNeighbor;

        public PseudoDoor(Vector3 localPosition, DirectionalEnum direction) {
            this.localPosition = localPosition;
            this.direction = direction;
        }

        public Vector3 GetLocalPosition() {
            return localPosition;
        }

        public void SetDoorNeighbor(Vector3 doorNeighbor) {
            this.doorNeighbor = doorNeighbor;
        }

        public Vector3 SetDoorNeighbor() {
            return doorNeighbor;
        }
        public DirectionalEnum GetDirection() {
            return direction;
        }

    }
}