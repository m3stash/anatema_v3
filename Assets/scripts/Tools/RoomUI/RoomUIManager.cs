using UnityEngine;
namespace RoomUI {
    public class RoomUIManager : MonoBehaviour {

        [SerializeField] GameObject roomGrid;
        [SerializeField] GameObject itemGrid;
        [SerializeField] GameObject stateManager;

        private RoomGridManager roomGridManager;
        private ItemGridManager itemGridManager;
        private RoomStateManager roomStateManager;
        //private

        private void Awake() {
            InitComponents();
        }

        private void InitComponents() {
            roomGridManager = roomGrid.GetComponent<RoomGridManager>();
            itemGridManager = itemGrid.GetComponent<ItemGridManager>();
            roomStateManager = stateManager.GetComponent<RoomStateManager>();
            if (roomGridManager == null) {
                Debug.Log("Error: SerializeField roomGrid not Set !");
            }
            if (itemGridManager == null) {
                Debug.Log("Error: SerializeField itemGridManager not Set !");
            }
            if (stateManager == null) {
                Debug.Log("Error: SerializeField stateManager not Set !");
            }
        }

    }

}
