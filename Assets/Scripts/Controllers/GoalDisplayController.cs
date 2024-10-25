using System;
using Data;
using EssentialManagers.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class GoalDisplayController : MonoBehaviour
    {
        [Header("References")] public Image goalIcon;
        public TextMeshProUGUI goalTargetCountTxt;
        private int _targetCount;

        [Header("Debug")] [SerializeField] private GoalData currentGoalData;
        public bool goalCompleted;
        private void Start()
        {
            CanvasManager.instance.BuildingPickedEvent += OnBuildingPicked;
            CanvasManager.instance.BuildingArrivedEvent += OnBuildingArrived;
        }

        private void OnBuildingPicked(BuildingType type)
        {
            if (type != currentGoalData.buildingType) return;

            AudioManager.instance.Play(SoundTag.Goal_Building_Picked);
            _targetCount--;
            goalTargetCountTxt.text = _targetCount.ToString();
        }
        
        private void OnBuildingArrived(BuildingType type)
        {
            if (type != currentGoalData.buildingType) return;
            
            if (_targetCount == 0)
            {
                goalCompleted = true;
                CanvasManager.instance.CheckIfGoalsMet();
            }
            
        }

        public void SetGoal(GoalData data)
        {
            goalIcon.gameObject.SetActive(true);
            currentGoalData = data;
            
            goalIcon.sprite = currentGoalData.icon;
            _targetCount = currentGoalData.targetCount;
            goalTargetCountTxt.text = _targetCount.ToString();
        }
    }
}