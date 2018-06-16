using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LabelsOnFloor
{
    public class CustomRoomLabelManager
    {
        private readonly List<CustomRoomLabel> _roomLabels = new List<CustomRoomLabel>();


        public bool IsRoomCustomised(Room room)
        {
            return _roomLabels.Any(rl => rl.RoomObject == room);
        }

        public string GetCustomLabelFor(Room room)
        {
            return _roomLabels.FirstOrDefault(rl => rl.RoomObject == room)?.Label;
        }

        public void CleanupMissingRooms()
        {
            _roomLabels.RemoveAll(rl => !rl.IsRoomStillValid());
        }
    }
}