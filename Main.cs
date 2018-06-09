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
                LabelPlacementHandler.Ready = false;
                return;
            }

            LabelPlacementHandler.RegenerateIfNeeded();
            _labelDrawer.Draw();

        }

        public override void OnGUI()
        {
            base.OnGUI();
            if (WorldRendererUtility.WorldRenderedNow && LabelPlacementHandler != null)
                LabelPlacementHandler.Ready = false;
        }

        public override void WorldLoaded()
        {
            LabelPlacementHandler.Ready = false;
        }

        public override void DefsLoaded()
        {
            _enabled = Settings.GetHandle(
                "showRoomLabels", "FALCLF.Enabled".Translate(),
                "FALCLF.EnabledDesc".Translate(), true);
        }
    }
}
