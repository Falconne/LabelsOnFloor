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
            var result = _roomLabels.FirstOrDefault(rl => rl.RoomObject == room)?.Label;

            return result;
        }

        public CustomRoomData GetOrCreateCustomRoomDataFor(Room room, IntVec3 loc)
        {
            var result = _roomLabels.FirstOrDefault(rl => rl.RoomObject == room);
            if (result != null)
                return result;

            result = new CustomRoomData(room, Find.VisibleMap, "", loc);
            _roomLabels.Add(result);
            result = _roomLabels.FirstOrDefault(rl => rl.RoomObject == room);

            return result;
        }

        public void CleanupMissingRooms()
        {
            _roomLabels.ForEach(d => d.AllocateRoomObjectIfNeeded());
            _roomLabels.RemoveAll(data => !data.IsRoomStillValid() || string.IsNullOrEmpty(data.Label));
        }
    }
}