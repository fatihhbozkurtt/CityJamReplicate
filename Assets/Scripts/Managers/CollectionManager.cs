using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Managers
{
    public class CollectionManager : MonoSingleton<CollectionManager>
    {
        [Header("References")] [SerializeField]
        private List<PlacementPoint> points;

        public PlacementPoint GetAvailablePoint()
        {
            PlacementPoint point = null;

            foreach (var p in points)
            {
               if(p.isOccupied) continue;
               point = p;
            }
            
            return point;
        }
    }
}