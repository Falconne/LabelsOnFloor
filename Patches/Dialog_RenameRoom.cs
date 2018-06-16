using Verse;

namespace LabelsOnFloor
{
    public class Dialog_RenameRoom : Dialog_Rename
    {
        private CustomRoomData _customRoomData;

        public Dialog_RenameRoom(CustomRoomData customRoomData)
        {
            _customRoomData = customRoomData;
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