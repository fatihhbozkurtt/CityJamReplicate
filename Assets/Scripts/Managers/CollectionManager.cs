using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;

namespace Managers
{
    public class CollectionManager : MonoSingleton<CollectionManager>
    {
        public event System.Action<BuildingController> NewBuildingPickedEvent;

        [Header("References")] [SerializeField]
        private List<PlacementPoint> points;

        [Header("Debug")] private const int MinMatchedCount = 3;

        public PlacementPoint GetAvailablePoint()
        {
            PlacementPoint point = null;

            foreach (var p in points)
            {
                if (p.isOccupied) continue;
                point = p;
                break;
            }

            return point;
        }

        public void OnNewBuildingPicked(BuildingController newB)
        {
            NewBuildingPickedEvent?.Invoke(newB);

            List<BuildingController> matchedList = GetBuildingsByType(newB);
            if (matchedList.Count < 3)
            {
                Debug.LogWarning("No enough buildings has matched");
                if (GetAvailablePoint() == null)
                    GameManager.instance.EndGame(false);
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                matchedList[i].Merge(centerB: matchedList[1]);
            }

            RearrangeBuildingsPos();
        }

        private void RearrangeBuildingsPos()
        {
            List<BuildingController> buildings = GetBuildings();
            for (int i = 0; i < buildings.Count; i++)
            {
                PlacementPoint targetPoint = points[i];
                BuildingController building = buildings[i];
                //   if (targetPoint == building.GetPoint()) return;

                building.GetPoint().SetFree();
                building.RepositionSelf(targetPoint: targetPoint);
            }
        }


        public List<PlacementPoint> GetPoints()
        {
            return points;
        }

        private List<BuildingController> GetBuildings()
        {
            List<BuildingController> buildings = new();

            foreach (var p in points)
            {
                if (!p.isOccupied) continue;
                buildings.Add(p.GetBuilding());
            }

            return buildings;
        }

        private List<BuildingController> GetBuildingsByType(BuildingController newB)
        {
            List<BuildingController> matchedBuildings = new();


            foreach (var p in points)
            {
                if (!p.isOccupied) continue;
                if (p.GetBuilding().GetType() != newB.GetType()) continue;

                matchedBuildings.Add(p.GetBuilding());
            }

            return matchedBuildings;
        }
    }
}