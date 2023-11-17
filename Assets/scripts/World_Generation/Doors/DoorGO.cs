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
                Debug.LogError("DoorGO: SpriteConfig are not serialised !!!");
            }
            setSpriteRender();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.name == "Player") {
                OnChangeRoom(this);
            }
        }

        public DirectionalEnum GetDirection() {
            return direction;
        }

        private void setSpriteRender() {
            if (spriteRenderer == null) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Setup(Vector3 localPosition, DirectionalEnum direction, RoomTypeEnum roomType, BiomeEnum biome) {
            setSpriteRender();
            this.direction = direction;
            SpriteConfig.BiomeRoomTypeSprites spriteConf = spriteConfig.GetSpriteConfig(biome, roomType);
            switch (direction) {
                case DirectionalEnum.T:
                case DirectionalEnum.B:
                    spriteRenderer.sprite = spriteConf.topSprite;
                    break;
                case DirectionalEnum.L:
                case DirectionalEnum.R:
                    spriteRenderer.sprite = spriteConf.leftSprite;
                    break;
            }
            transform.localPosition = localPosition;
        }

    }

}