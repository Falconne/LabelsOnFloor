using HugsLib.Settings;
using HugsLib.Utils;
using RimWorld.Planet;
using Verse;

namespace LabelsOnFloor
{
    public class Main : HugsLib.ModBase
    {

        internal new ModLogger Logger => base.Logger;

        internal static Main Instance { get; private set; }

        public override string ModIdentifier => "LabelsOnFloor";

        private SettingHandle<bool> _enabled;

        private readonly LabelHolder _labelHolder = new LabelHolder();
        
        private readonly LabelPlacementHandler _labelPlacementHandler;

        public Main()
        {
            Instance = this;
            _labelPlacementHandler = new LabelPlacementHandler(_labelHolder);
        }

        public override void OnGUI()
        {
            if (!_enabled)
                return;

            if (Current.ProgramState != ProgramState.Playing || Find.VisibleMap == null
                || WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            _labelPlacementHandler.RegenerateIfNeeded();
        }

        public override void WorldLoaded()
        {
            _labelHolder.Clear();
        }

        public override void DefsLoaded()
        {
            _enabled = Settings.GetHandle(
                "showRoomLabels", "FALCLF.Enabled".Translate(),
                "FALCLF.EnabledDesc".Translate(), true);
        }
    }
}
