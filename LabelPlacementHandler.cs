using System.Linq;
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
                if (!_ready)
                    _labelHolder.Clear();
            }
        }

        private readonly LabelHolder _labelHolder;

        private readonly LabelMaker _labelMaker = new LabelMaker();

        private readonly RoomRoleFinder _roomRoleFinder = new RoomRoleFinder();

        private readonly MeshHandler _meshHandler;


        private Map _map;
        private bool _ready;

        public LabelPlacementHandler(LabelHolder labelHolder, MeshHandler meshHandler)
        {
            _labelHolder = labelHolder;
            _meshHandler = meshHandler;
        }

        public void RegenerateIfNeeded()
        {
            if (_map == Find.VisibleMap && Ready)
                return;

            _map = Find.VisibleMap;
            _labelHolder.Clear();
            _ready = true;

            RegenerateRoomLabels();
            RegenerateZoneLabels();
        }

        public void AddRoom(Room room, RoomPlacementDataFinder roomPlacementDataFinder)
        {
            if (!Ready || room == null)
                return;

            _labelHolder.RemoveLabelForObject(room);

            if (room.Fogged || !_roomRoleFinder.IsImportantRoom(room))
                return;

            if (room.Map != _map)
                return;

            var text = _labelMaker.GetRoomLabel(room);
            if (roomPlacementDataFinder == null)
            {
                roomPlacementDataFinder = new RoomPlacementDataFinder(_map);
            }

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

        public void AddZone(Zone zone)
        {
            if (!Ready || zone == null)
                return;

            _labelHolder.RemoveLabelForObject(zone);

            var text = _labelMaker.GetZoneLabel(zone);
            if (string.IsNullOrEmpty(text))
                return;

            var label = new Label()
            {
                LabelMesh = _meshHandler.GetMeshFor(text),
                LabelPlacementData = GetLabelPlacementDataForZone(zone, text.Length),
                AssociatedObject = zone
            };

            if (label.LabelPlacementData != null)
                _labelHolder.Add(label);

        }

        private void RegenerateRoomLabels()
        {
            var roomPlacementDataFinder = new RoomPlacementDataFinder(_map);
            foreach (var room in _map.regionGrid.allRooms)
            {
                AddRoom(room, roomPlacementDataFinder);
            }
        }

        private void RegenerateZoneLabels()
        {
            foreach (var zone in _map.zoneManager.AllZones)
            {
                AddZone(zone);
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