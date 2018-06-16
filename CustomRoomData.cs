using Verse;

namespace LabelsOnFloor
{
    public class CustomRoomData
    {
        public string Label;

        public readonly Room RoomObject;

        private readonly Map _map;

        private readonly IntVec3 _keyCell;


        public CustomRoomData(Room roomObject, Map map, string label, IntVec3 keyCell)
        {
            RoomObject = roomObject;
            _map = map;
            Label = label;
            _keyCell = keyCell;
        }

        public bool IsRoomStillValid()
        {
            if (RoomObject == null || _map == null)
                return false;

            return _keyCell.GetRoom(_map) == RoomObject;
        }
    }
}