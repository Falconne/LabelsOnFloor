using Verse;

namespace LabelsOnFloor
{
    public class LabelPlacementHandler
    {
        public bool Ready
        {
            get => _ready;

            set
            {
                _ready = value;
                if (_ready)
                    return;

                SetDirty();
                _labelHolder.Clear();
            }
        }

        private readonly LabelHolder _labelHolder;

        private readonly LabelMaker _labelMaker = new LabelMaker();

        private readonly RoomRoleFinder _roomRoleFinder = new RoomRoleFinder();

        private readonly MeshHandler _meshHandler;


        private int _nextUpdateTick;

        private Map _map;
        private bool _ready;

        public LabelPlacementHandler(LabelHolder labelHolder, MeshHandler meshHandler)
        {
            _labelHolder = labelHolder;
            _meshHandler = meshHandler;
        }

        public void SetDirty()
        {
            _labelHolder.Clear();
            _nextUpdateTick = 0;
        }

        public void RegenerateIfNeeded()
        {
            var tick = Find.TickManager.TicksGame;
            if (_map == Find.VisibleMap && tick < _nextUpdateTick)
                return;

            _nextUpdateTick = tick + 200;
            Regenerate();
        }

        private void Regenerate()
        {
            _map = Find.VisibleMap;
            _labelHolder.Clear();

            RegenerateRoomLabels();
            RegenerateZoneLabels();
        }

        private void RegenerateRoomLabels()
        {
            var roomPlacementDataFinder = new RoomPlacementDataFinder(_map);
            foreach (var room in _map.regionGrid.allRooms)
            {
                if (room == null || room.Fogged || !_roomRoleFinder.IsImportantRoom(room))
                    continue;

                var text = _labelMaker.GetRoomLabel(room);
                var label = new Label()
                {
                    LabelMesh = _meshHandler.GetMeshFor(text),
                    LabelPlacementData = 
                        roomPlacementDataFinder.GetLabelPlacementDataForRoom(room, text.Length),
                    AssociatedObject = room
                };

                if (label.IsValid())
                    _labelHolder.Add(label);
            }
        }

        public void AddRoom(Room room)
        {
            if (!Ready)
                return;

            if (room == null || room.Fogged || !_roomRoleFinder.IsImportantRoom(room))
                return;

            if (room.Map != _map)
                return;

            var text = _labelMaker.GetRoomLabel(room);
            var label = new Label()
            {
                LabelMesh = _meshHandler.GetMeshFor(text),
                LabelPlacementData = 
                    roomPlacementDataFinder.GetLabelPlacementDataForRoom(room, text.Length),
                AssociatedObject = room
            };

            if (label.IsValid())
                _labelHolder.Add(label);

        }

        private void RegenerateZoneLabels()
        {
            foreach (var zone in _map.zoneManager.AllZones)
            {
                var text = _labelMaker.GetZoneLabel(zone);
                if (string.IsNullOrEmpty(text))
                    continue;

                var label = new Label()
                {
                    LabelMesh = _meshHandler.GetMeshFor(text),
                    LabelPlacementData = GetLabelPlacementDataForZone(zone, text.Length),
                    AssociatedObject = zone
                };

                if (label.LabelPlacementData != null)
                    _labelHolder.Add(label);
            }
        }

        private PlacementData GetLabelPlacementDataForZone(Zone zone, int labelLength)
        {
            return EdgeFinder.GetBestPlacementData(
                zone.Cells,
                c => c.Fogged(_map),
                c => true,
                c => true,
                labelLength
            );
        }
    }
}