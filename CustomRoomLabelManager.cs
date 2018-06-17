using System.Collections.Generic;
using System.Linq;
using HugsLib.Utils;
using Verse;

namespace LabelsOnFloor
{
    public class CustomRoomLabelManager : UtilityWorldObject
    {
        private List<CustomRoomData> _roomLabels = new List<CustomRoomData>();


        public bool IsRoomCustomised(Room room)
        {
            return _roomLabels.Any(rl => rl.RoomObject == room);
        }

        public string GetCustomLabelFor(Room room)
        {
            var result = _roomLabels.FirstOrDefault(rl => rl.RoomObject == room)?.Label.ToUpper();

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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref _roomLabels, "roomLabels", LookMode.Deep);
        }
    }
}