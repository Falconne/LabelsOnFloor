using HugsLib.Settings;
using HugsLib.Utils;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public class Main : HugsLib.ModBase
    {

        internal new ModLogger Logger => base.Logger;

        internal static Main Instance { get; private set; }

        public override string ModIdentifier => "LabelsOnFloor";

        private SettingHandle<bool> _enabled;

        private LabelPlacementHandler _labelPlacementHandler =
            new LabelPlacementHandler();

        public Main()
        {
            Instance = this;
        }

        public override void OnGUI()
        {
            if (Current.ProgramState != ProgramState.Playing || Find.VisibleMap == null
                || WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            _labelPlacementHandler.Draw();
        }

        public override void WorldLoaded()
        {
            // _infoCollector.Reset();
        }

        public override void DefsLoaded()
        {
            _enabled = Settings.GetHandle(
                "showRoomLabels", "FALCLF.Enabled".Translate(),
                "FALCLF.EnabledDesc".Translate(), true);
        }
    }
}
