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

        public override string ModIdentifier => "LabelsOn";

        private SettingHandle<bool> _showRoomLabels;

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

        }

        public override void WorldLoaded()
        {
            _infoCollector.Reset();
        }

        public override void DefsLoaded()
        {
            _showRoomLabels = Settings.GetHandle(
                "showRoomLabels", "FALCLF.ShowRoomLabels".Translate(),
                "FALCLF.ShowRoomLabelsDesc".Translate(), true);
        }
    }
}
