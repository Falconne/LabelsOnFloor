﻿using Verse;

namespace LabelsOnFloor
{
    public class Dialog_RenameRoom : Dialog_Rename<IRenameable>
    {
        private readonly CustomRoomData _customRoomData;

        public Dialog_RenameRoom(CustomRoomData customRoomData) : base(null)
        {
            _customRoomData = customRoomData;
            curName = customRoomData.Label;
        }

        protected override void OnRenamed(string name)
        {
            _customRoomData.Label = name;
            Main.Instance.LabelPlacementHandler.SetDirty();
        }

        protected override AcceptanceReport NameIsValid(string name)
        {
            return true;
        }
    }
}