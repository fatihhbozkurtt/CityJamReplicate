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

        public Vector3 GetCenter()
        {
            return transform.position;
        }

        #endregion
    }
}