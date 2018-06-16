using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LabelsOnFloor
{
    public class CustomRoomLabelManager
    {
        private readonly List<CustomRoomData> _roomLabels = new List<CustomRoomData>();


        public bool IsRoomCustomised(Room room)
        {
            return _roomLabels.Any(rl => rl.RoomObject == room);
        }

        public string GetCustomLabelFor(Room room)
        {
            return _roomLabels.FirstOrDefault(rl => rl.RoomObject == room)?.Label;
        }

        public string GetOrCreateCustomRoomDataFor(Room room, IntVec3 loc)
        {
            var result = GetCustomLabelFor(room);
            if (result == null)
            {
                result = new CustomRoomData(room, Find.VisibleMap, "", loc);
            }
        }

        public void CleanupMissingRooms()
        {
            _roomLabels.RemoveAll(rl => !rl.IsRoomStillValid());
        }
    }
}