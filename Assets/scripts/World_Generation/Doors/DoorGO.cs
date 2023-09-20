﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoorNs {
    public class DoorGO : MonoBehaviour {
        //toDO remove serialisable field..
        [SerializeField] private DoorType doorType;
        [SerializeField] private Vector2Int NeighBoorDoor;
        [SerializeField] private DirectionalEnum direction;
        [SerializeField] private Vector3Int localPosition;
        [SerializeField] private Sprite sprite;
        private SpriteRenderer spriteRenderer;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetBiome(Biome biome) {
            spriteRenderer.sprite = biome.GetDoorSpriteForDirection(direction);
        }

        // [SerializeField] private DoorEnum doorType;

        public delegate void OnDoorEnter(DoorGO doorGO);
        public static event OnDoorEnter OnChangeRoom;

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

        public void SetLocalPosition(Vector3Int worldPosition) {
            Debug.Log("worldPosition" + worldPosition);
            localPosition = worldPosition;
        }
        public Vector3 GetLocalPosition() {
            print(transform.localPosition.y);
            print(transform.localPosition.x);
            return transform.localPosition;
        }

        public void SetSprite(Sprite sprite) {
            this.sprite = sprite;
        }

    }

}