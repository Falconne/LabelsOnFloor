using HugsLib.Settings;
using HugsLib.Utils;
using RimWorld.Planet;
using Verse;

namespace LabelsOnFloor
{
    public class Main : HugsLib.ModBase
    {

        public readonly LabelPlacementHandler LabelPlacementHandler;

        internal new ModLogger Logger => base.Logger;

        internal static Main Instance { get; private set; }

        public override string ModIdentifier => "LabelsOnFloor";

        private SettingHandle<bool> _enabled;

        private SettingHandle<bool> _showRoomLabels;

        private SettingHandle<bool> _showZoneLabels;

        private readonly LabelHolder _labelHolder = new LabelHolder();

        private readonly LabelDrawer _labelDrawer;

        private readonly FontHandler _fontHandler = new FontHandler();

        public Main()
        {
            Instance = this;
            LabelPlacementHandler = new LabelPlacementHandler(
                _labelHolder,
                new MeshHandler(_fontHandler));

            _labelDrawer = new LabelDrawer(_labelHolder, _fontHandler);
        }

        public void Draw()
        {
            if (!_enabled
                || Current.ProgramState != ProgramState.Playing
                || Find.VisibleMap == null
                || WorldRendererUtility.WorldRenderedNow)
            {
                LabelPlacementHandler.SetDirty();
                return;
            }

            LabelPlacementHandler.RegenerateIfNeeded();
            _labelDrawer.Draw();

        }

        public bool ShowRoomNames()
        {
            return _showRoomLabels;
        }

        public bool ShowZoneNames()
        {
            return _showZoneLabels;
        }

        public override void OnGUI()
        {
            if (WorldRendererUtility.WorldRenderedNow)
                LabelPlacementHandler?.SetDirty();

            base.OnGUI();
        }

        public override void WorldLoaded()
        {
            LabelPlacementHandler.SetDirty();
        }

        public override void DefsLoaded()
        {
            _enabled = Settings.GetHandle(
                "enabled", "FALCLF.Enabled".Translate(),
                "FALCLF.EnabledDesc".Translate(), true);

            _showRoomLabels = Settings.GetHandle(
                "showRoomLabels", "FALCLF.ShowRoomLabels".Translate(),
                "FALCLF.ShowRoomLabelsDesc".Translate(), true);

            _showZoneLabels = Settings.GetHandle(
                "showZoneLabels", "FALCLF.ShowZoneLabels".Translate(),
                "FALCLF.ShowZoneLabelsDesc".Translate(), true);


            _enabled.OnValueChanged = val => { LabelPlacementHandler.SetDirty(); };

            _showRoomLabels.OnValueChanged = val => { LabelPlacementHandler.SetDirty(); };

            _showZoneLabels.OnValueChanged = val => { LabelPlacementHandler.SetDirty(); };

        }
    }
}
