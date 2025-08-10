using HugsLib.Settings;
using HugsLib.Utils;
using RimWorld.Planet;
using Verse;

namespace LabelsOnFloor
{
    public class Main : HugsLib.ModBase
    {

        public LabelPlacementHandler LabelPlacementHandler;

        internal new ModLogger Logger => base.Logger;

        internal static Main Instance { get; private set; }

        public override string ModIdentifier => "LabelsOnFloor";

        private SettingHandle<bool> _enabled;

        private SettingHandle<bool> _useLightText;

        private SettingHandle<int> _opacity;

        private SettingHandle<bool> _showRoomLabels;

        private SettingHandle<bool> _showZoneLabels;

        private SettingHandle<float> _maxFontScale;

        private SettingHandle<float> _minFontScale;

        private SettingHandle<CameraZoomRange> _maxAllowedZoom;

        private readonly LabelHolder _labelHolder = new LabelHolder();

        private LabelDrawer _labelDrawer;

        private readonly FontHandler _fontHandler = new FontHandler();

        private CustomRoomLabelManager _customRoomLabelManager;


        public Main()
        {
            Instance = this;
        }

        public Dialog_RenameRoom GetRoomRenamer(Room room, IntVec3 loc)
        {
            return new Dialog_RenameRoom(
                _customRoomLabelManager.GetOrCreateCustomRoomDataFor(room, loc)
            );
        }

        public void Draw()
        {
            if (!IsModAcitve())
            {
                LabelPlacementHandler.SetDirty();
                return;
            }

            if (Find.CameraDriver.CurrentZoom > _maxAllowedZoom)
                return;

            LabelPlacementHandler.RegenerateIfNeeded(_customRoomLabelManager);
            _labelDrawer.Draw();
        }

        public bool IsModAcitve()
        {
            return _enabled
                   && Current.ProgramState == ProgramState.Playing
                   && Find.CurrentMap != null
                   && !WorldRendererUtility.WorldRendered;
        }

        public bool UseLightText()
        {
            return _useLightText;
        }

        public float GetOpacity()
        {
            return _opacity / 100f;
        }

        public bool ShowRoomNames()
        {
            return _showRoomLabels;
        }

        public bool ShowZoneNames()
        {
            return _showZoneLabels;
        }

        public float GetMaxFontScale()
        {
            return _maxFontScale;
        }

        public float GetMinFontScale()
        {
            return _minFontScale;
        }

        public override void OnGUI()
        {
            if (WorldRendererUtility.WorldRendered)
                LabelPlacementHandler?.SetDirty();

            base.OnGUI();
        }

        public override void WorldLoaded()
        {
            base.WorldLoaded();

            _customRoomLabelManager = 
                UtilityWorldObjectManager.GetUtilityWorldObject<CustomRoomLabelManager>();

            LabelPlacementHandler = new LabelPlacementHandler(
                _labelHolder,
                new MeshHandler(_fontHandler),
                new LabelMaker(_customRoomLabelManager),
                new RoomRoleFinder(_customRoomLabelManager)
            );

            _labelDrawer = new LabelDrawer(_labelHolder, _fontHandler);

        }

        public override void MapLoaded(Map map)
        {
            base.MapLoaded(map);
            LabelPlacementHandler.SetDirty();
        }

        public override void DefsLoaded()
        {
            _enabled = Settings.GetHandle(
                "enabled", "FALCLF.Enabled".Translate(),
                "FALCLF.EnabledDesc".Translate(), true);

            _useLightText = Settings.GetHandle(
                "useLightText", "FALCLF.UseLightText".Translate(),
                "FALCLF.UseLightTextDesc".Translate(), false);

            _useLightText.OnValueChanged = val => { _fontHandler?.Reset(); };

            _opacity = Settings.GetHandle(
                "opacity", "FALCLF.TextOpacity".Translate(),
                "FALCLF.TextOpacityDesc".Translate(), 30,
                Validators.IntRangeValidator(1, 100));

            _opacity.OnValueChanged = val => { _fontHandler?.Reset(); };


            _showRoomLabels = Settings.GetHandle(
                "showRoomLabels", "FALCLF.ShowRoomLabels".Translate(),
                "FALCLF.ShowRoomLabelsDesc".Translate(), true);

            _showZoneLabels = Settings.GetHandle(
                "showZoneLabels", "FALCLF.ShowZoneLabels".Translate(),
                "FALCLF.ShowZoneLabelsDesc".Translate(), true);

            _maxFontScale = Settings.GetHandle(
                "maxFontScale", "FALCLF.MaxFontScale".Translate(),
                "FALCLF.MaxFontScaleDesc".Translate(), 1f,
                Validators.FloatRangeValidator(0.1f, 5.0f));

            _minFontScale = Settings.GetHandle(
                "minFontScale", "FALCLF.MinFontScale".Translate(),
                "FALCLF.MinFontScaleDesc".Translate(), 0.2f,
                Validators.FloatRangeValidator(0.1f, 1.0f));

            _maxAllowedZoom = Settings.GetHandle(
                "maxAllowedZoom", "FALCLF.MaxAllowedZoom".Translate(),
                "FALCLF.MaxAllowedZoomDesc".Translate(), CameraZoomRange.Furthest,
                null, "FALCLF.enumSetting_");


            _enabled.OnValueChanged = val => { LabelPlacementHandler.SetDirty(); };

            _showRoomLabels.OnValueChanged = val => { LabelPlacementHandler.SetDirty(); };

            _showZoneLabels.OnValueChanged = val => { LabelPlacementHandler.SetDirty(); };

            _maxFontScale.OnValueChanged = val => { LabelPlacementHandler.SetDirty(); };

            _minFontScale.OnValueChanged = val => { LabelPlacementHandler.SetDirty(); };
        }
    }
}
