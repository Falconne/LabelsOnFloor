using System;
using System.Linq;
using Verse;

namespace LabelsOnFloor
{
    public class LabelPlacementHandler
    {
        private readonly LabelHolder _labelHolder;

        private readonly LabelMaker _labelMaker;

        private readonly RoomRoleFinder _roomRoleFinder = new RoomRoleFinder();

        private readonly MeshHandler _meshHandler;

        private Map _map;

        private bool _ready;

        public LabelPlacementHandler(LabelHolder labelHolder, MeshHandler meshHandler, 
            LabelMaker labelMaker)
        {
            _labelHolder = labelHolder;
            _meshHandler = meshHandler;
            _labelMaker = labelMaker;
        }

        public void SetDirty()
        {
            _labelHolder.Clear();
            _ready = false;
        }

        public void SetDirtyIfAreaIsOnMap(Map map)
        {
            if (map == _map)
                SetDirty();
        }

        public bool RegenerateIfNeeded()
        {
            if (_map == Find.VisibleMap && _ready)
                return false;

            _map = Find.VisibleMap;
            _labelHolder.Clear();
            _ready = true;

            RegenerateRoomLabels();
            RegenerateZoneLabels();

            return true;
        }

        public void AddOrUpdateRoom(Room room)
        {
            AddOrUpdateRoom(room, null);
        }

        public void AddOrUpdateRoom(Room room, PlacementDataFinderForRooms placementDataFinderForRooms)
        {
            if (!Main.Instance.ShowRoomNames())
                return;

            if (!_ready || room == null)
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

        public void AddOrUpdateZone(Zone zone)
        {
            if (!_ready || zone == null)
                return;

            if (zone.Map != _map)
                return;

            var text = _labelMaker.GetZoneLabel(zone);
            var addedLabel = 
                AddLabelForArea(zone, text, () => PlacementDataFinderForZones.GetData(zone, _map, text.Length));

            if (addedLabel != null)
                addedLabel.IsZone = true;
        }

        private Label AddLabelForArea(object area, string text, Func<PlacementData> placementDataGetter)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            _labelHolder.RemoveLabelForArea(area);

            var label = new Label
            {
                LabelMesh = _meshHandler.GetMeshFor(text),
                LabelPlacementData = placementDataGetter(),
                AssociatedArea = area
            };

            if (!label.IsValid())
                return null;

            _labelHolder.Add(label);
            return label;
        }

        private void RegenerateRoomLabels()
        {
            if (!Main.Instance.ShowRoomNames())
                return;

            var roomPlacementDataFinder = new PlacementDataFinderForRooms(_map);
            foreach (var room in _map.regionGrid.allRooms)
            {
                AddOrUpdateRoom(room, roomPlacementDataFinder);
            }
        }

        private void RegenerateZoneLabels()
        {
            if (!Main.Instance.ShowZoneNames())
                return;

            foreach (var zone in _map.zoneManager.AllZones)
            {
                AddOrUpdateZone(zone);
            }
        }

    }
}