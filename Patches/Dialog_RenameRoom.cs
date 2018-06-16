using Verse;

namespace LabelsOnFloor
{
    public class Dialog_RenameRoom : Dialog_Rename
    {
        private CustomRoomLabelManager _customRoomLabelManager;

        private CustomRoomData _customRoomData;

        public Dialog_RenameRoom(CustomRoomLabelManager customRoomLabelManager,
            Room room, IntVec3 loc)
        {
            _customRoomLabelManager = customRoomLabelManager;
            var customRoomData = _customRoomLabelManager.GetOrCreateCustomRoomDataFor(room, loc);
            curName = customRoomData.Label;
        }

        protected override void SetName(string name)
        {
            _customRoomData.Label = name;

        }

        protected override AcceptanceReport NameIsValid(string name)
        {
            return true;
        }
    }
}