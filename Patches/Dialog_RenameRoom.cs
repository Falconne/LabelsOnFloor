using Verse;

namespace LabelsOnFloor
{
    public class Dialog_RenameRoom : Dialog_Rename
    {
        private readonly CustomRoomData _customRoomData;

        public Dialog_RenameRoom(CustomRoomData customRoomData)
        {
            _customRoomData = customRoomData;
            curName = customRoomData.Label;
        }

        protected override void SetName(string name)
        {
            _customRoomData.Label = name.ToUpper();
            Main.Instance.LabelPlacementHandler.SetDirty();
        }

        protected override AcceptanceReport NameIsValid(string name)
        {
            return true;
        }
    }
}