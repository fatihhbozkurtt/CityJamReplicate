using Data;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace Controllers
{
    public class BuildingController : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private BuildingType type;
        [Header("Debug")] public bool isSelected;
        [SerializeField] private PlacementPoint currentPoint;

        private void OnMouseDown()
        {
            if (!GameManager.instance.isLevelActive) return;
            if (isSelected) return;

            // demand available placement point from collection manager
            PlacementPoint point = CollectionManager.instance.GetAvailablePoint();
            if (point == null) return;
            SetPoint(point, true);
        }


        private void SetPoint(PlacementPoint newPoint, bool performMoving)
        {
            currentPoint.SetFree();
            currentPoint = newPoint;
            currentPoint.SetOccupied(this);

            if (!performMoving) return;

            Move(currentPoint);
        }

        private void Move(PlacementPoint targetPoint)
        {
            transform.DOJump(targetPoint.GetCenter(), 10, 1, .75f);
        }


        public BuildingType GetType()
        {
            return type;
        }
    }
}