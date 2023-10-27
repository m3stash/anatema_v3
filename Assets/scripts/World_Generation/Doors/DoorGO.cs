using RoomNs;
using UnityEngine;

namespace DoorNs {
    public class DoorGO : MonoBehaviour {

        public delegate void OnDoorEnter(DoorGO doorGO);
        public static event OnDoorEnter OnChangeRoom;
        private DirectionalEnum direction;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteConfig spriteConfig;
        // private PoolConfig config;

        private void Awake() {
            if(spriteConfig == null) {
                Debug.LogError("DoorGO: SpriteConfig are not serialisable");
            }
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.name == "Player") {
                OnChangeRoom(this);
            }
        }

        public DirectionalEnum GetDirection() {
            return direction;
        }

        public void Setup(Vector3 localPosition, DirectionalEnum direction, RoomTypeEnum roomType, BiomeEnum biome) {
            this.direction = direction;
            //spriteRenderer.sprite = sprite;
            transform.localPosition = localPosition;
        }

    }

}