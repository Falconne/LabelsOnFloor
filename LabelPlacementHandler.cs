using System;
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

        public void AddRoom(Room room, PlacementDataFinderForRooms placementDataFinderForRooms)
        {
            if (!Ready || room == null)
                return;

            if (room.Map != _map)
                return;

            if (room.Fogged || !_roomRoleFinder.IsImportantRoom(room))
                return;

            var text = _labelMaker.GetRoomLabel(room);
            if (placementDataFinderForRooms == null)
            {
                placementDataFinderForRooms = new PlacementDataFinderForRooms(_map);
            }

            AddLabelForArea(room, text, () => placementDataFinderForRooms.GetData(room, text.Length));
        }

        public void AddZone(Zone zone)
        {
            if (!Ready || zone == null)
                return;

            if (zone.Map != _map)
                return;

            var text = _labelMaker.GetZoneLabel(zone);
            AddLabelForArea(zone, text, () => PlacementDataFinderForZones.GetData(zone, _map, text.Length));
        }

        private void AddLabelForArea(object area, string text, Func<PlacementData> placementDataGetter)
        {
            if (string.IsNullOrEmpty(text))
                return;

            _labelHolder.RemoveLabelForArea(area);

            var label = new Label
            {
                LabelMesh = _meshHandler.GetMeshFor(text),
                LabelPlacementData = placementDataGetter(),
                AssociatedArea = area
            };

            if (label.IsValid())
                _labelHolder.Add(label);
        }

        private void RegenerateRoomLabels()
        {
            var roomPlacementDataFinder = new PlacementDataFinderForRooms(_map);
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

    }
}