namespace RoomUI {
    public class RoomUIService {

        private RoomUiTable roomUiTable;

        public RoomUIService(DatabaseManager dbManager) {
            roomUiTable = new RoomUiTable(dbManager);
            roomUiTable.CreateTableRoom();
        }

        public int SaveRoom(RoomUIModel roomUi) {
            int roomId = roomUi.Id;
            if (roomId == -1) {
                return roomUiTable.Insert(roomUi);
            }
            return roomUiTable.Update(roomUi);
        }

    }

}