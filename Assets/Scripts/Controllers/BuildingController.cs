using System.Collections.Generic;
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
            if (point == null)
            {
                Debug.LogError("COLLECTION IS FULL !!!");
                GameManager.instance.EndGame(false);
                return;
            }

            SetPoint(point, true);
        }

        private void SetPoint(PlacementPoint newPoint, bool performMoving)
        {
            if (currentPoint != null) currentPoint.SetFree();
            currentPoint = newPoint;
            currentPoint.SetOccupied(this);


            if (!performMoving) return;

            MoveToPoint(currentPoint);
        }

        private void MoveToPoint(PlacementPoint targetPoint)
        {
            Sequence sq = DOTween.Sequence();
            sq.Append(transform.DOJump(targetPoint.GetCenter(), 10, 1, .25f));
            sq.Join(transform.DOScale(Vector3.one / 7f, 0.25f));
            sq.OnComplete(() =>
            {
                transform.SetParent(currentPoint.transform);
                currentPoint.AnimateUpDown();
                CollectionManager.instance.OnNewBuildingPicked(this);
            });
        }

        public void Merge(BuildingController centerB)
        {
            currentPoint.SetFree();
            Sequence sq = DOTween.Sequence();
            sq.Append(transform.DOMoveY(transform.position.y + 5, 0.25f));
            if (centerB != this)
            {
                sq.Append(transform.DOJump(centerB.transform.position + Vector3.up * 3,
                    5, 1, .25f));
            }

            sq.Append(transform.DOScale(Vector3.zero, 0.25f));
            sq.OnComplete((() => { Destroy(gameObject); }));
        }

        public void RepositionSelf(PlacementPoint targetPoint)
        {
            SetPoint(targetPoint, false);
            transform.DOJump(targetPoint.GetCenter(),
                5, 1, .25f);
        }

        public PlacementPoint GetPoint()
        {
            return currentPoint;
        }

        public BuildingType GetType()
        {
            return type;
        }
    }
}