using DG.Tweening;
using UnityEngine;

namespace Controllers
{
    public class PlacementPoint : MonoBehaviour
    {
        [Header("Debug")] public bool isOccupied;
        [SerializeField] private BuildingController currentBuilding;

        #region Getters & Setters

        public void SetOccupied(BuildingController building)
        {
            currentBuilding = building;
            isOccupied = true;
        }

        public void SetFree()
        {
            currentBuilding = null;
            isOccupied = false;
        }

        public Vector3 GetCenterOffset()
        {
            return Vector3.up / 2;
        }

        public BuildingController GetBuilding()
        {
            return currentBuilding;
        }

        public void AnimateUpDown()
        {
            // Store the initial Y position
            float startY = transform.localPosition.y;
            float moveDistance = 1f; // How much to move up
            float duration = 0.25f; // Time to move up or down

            // Move up first, then move down back to the start position
            transform.DOLocalMoveY(startY + moveDistance, duration / 2)
                .OnComplete(() =>
                {
                    // After moving up, move back down to the start position
                    transform.DOLocalMoveY(startY, duration / 2);
                })
                .SetEase(Ease.InOutSine); // Smooth ease for natural movement
        }

        #endregion
    }
}