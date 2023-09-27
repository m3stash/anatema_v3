using UnityEngine;

namespace DoorNs {
    public class DoorGO : MonoBehaviour {

        public delegate void OnDoorEnter(DoorGO doorGO);
        public static event OnDoorEnter OnChangeRoom;
        private DoorType doorType;
        private DirectionalEnum direction;
        private SpriteRenderer spriteRenderer;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSpriteRender(Biome biome) {
            spriteRenderer.sprite = biome.GetDoorSpriteForDirection(direction);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.name == "Player") {
                OnChangeRoom(this);
            }
        }

        public DoorType GetDoorType() {
            return doorType;
        }

        public void SetDoorType(DoorType doorType) {
            this.doorType = doorType;
        }

        public void SetDirection(DirectionalEnum direction) {
            this.direction = direction;
        }

        public DirectionalEnum GetDirection() {
            return direction;
        }

    }

}