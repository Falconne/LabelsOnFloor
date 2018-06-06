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

        private readonly LabelDrawer _labelDrawer;

        private readonly FontHandler _fontHandler = new FontHandler();

        public Main()
        {
            Instance = this;
            _labelPlacementHandler = new LabelPlacementHandler(
                _labelHolder, 
                new MeshHandler(_fontHandler));

            _labelDrawer = new LabelDrawer(_labelHolder, _fontHandler);
        }

        public void SetDirty()
        {
            _labelPlacementHandler?.SetDirty();
        }
        
        public void Draw()
        {
            if (!_enabled)
                return;

            if (Current.ProgramState != ProgramState.Playing || Find.VisibleMap == null
                || WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            _labelPlacementHandler.RegenerateIfNeeded();
            _labelDrawer.Draw();

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
