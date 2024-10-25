using System.Collections.Generic;
using Controllers;
using Data;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EssentialManagers.Scripts
{
    public class CanvasManager : MonoSingleton<CanvasManager>
    {
        public event System.Action<BuildingType> BuildingPickedEvent;
        public event System.Action<BuildingType> BuildingArrivedEvent;

        private enum PanelType
        {
            MainMenu,
            Game,
            Success,
            Fail
        }

        [Header("Canvas Groups")] public CanvasGroup mainMenuCanvasGroup;
        public CanvasGroup gameCanvasGroup;
        public CanvasGroup successCanvasGroup;
        public CanvasGroup failCanvasGroup;

        [Header("Standard Objects")] public Image screenFader;
        public TextMeshProUGUI levelText;
        private CanvasGroup[] _canvasArray;

        [Header("Config")] public List<GoalDataList> dataLists;

        [Header("References")] [SerializeField]
        private List<GoalDisplayController> goalDisplayControllers;
        private int _dataIndex;
        private int _targetCount;


        protected override void Awake()
        {
            base.Awake();

            _canvasArray = new CanvasGroup[System.Enum.GetNames(typeof(PanelType)).Length];

            _canvasArray[(int)PanelType.MainMenu] = mainMenuCanvasGroup;
            _canvasArray[(int)PanelType.Game] = gameCanvasGroup;
            _canvasArray[(int)PanelType.Success] = successCanvasGroup;
            _canvasArray[(int)PanelType.Fail] = failCanvasGroup;

            foreach (CanvasGroup canvas in _canvasArray)
            {
                canvas.gameObject.SetActive(true);
                canvas.alpha = 0;
            }

            FadeInScreen(1f);
            ShowPanel(PanelType.MainMenu);


            // HACK: Workaround for FBSDK
            // FBSDK spawns a persistent EventSystem object. Since Unity 2020.2 there must be only one EventSystem objects at a given time.
            // So we must dispose our own EventSystem object if it exists.
            UnityEngine.EventSystems.EventSystem[] eventSystems =
                FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystems.Length > 1)
            {
                Destroy(GetComponentInChildren<UnityEngine.EventSystems.EventSystem>().gameObject);
                Debug.LogWarning("There are multiple live EventSystem components. Destroying ours.");
            }
        }

        void Start()
        {
            levelText.text = "LEVEL " + GameManager.instance.GetTotalStagePlayed();

            GameManager.instance.LevelStartedEvent += (() => ShowPanel(PanelType.Game));
            GameManager.instance.LevelSuccessEvent += (() => ShowPanel(PanelType.Success));
            GameManager.instance.LevelFailedEvent += (() => ShowPanel(PanelType.Fail));

            SetGoal();
        }

        private void ShowPanel(PanelType panelId)
        {
            int panelIndex = (int)panelId;

            for (int i = 0; i < _canvasArray.Length; i++)
            {
                if (i == panelIndex)
                {
                    FadePanelIn(_canvasArray[i]);
                }

                else
                {
                    FadePanelOut(_canvasArray[i]);
                }
            }
        }

        #region ButtonEvents

        public void OnTapRestart()
        {
            FadeOutScreen(GameManager.instance.RestartStage, 1);
            HapticManager.instance.TriggerHaptic(HapticTypes.HeavyImpact);
        }

        public void OnTapContinue()
        {
            FadeOutScreen(GameManager.instance.NextStage, 1);
            HapticManager.instance.TriggerHaptic(HapticTypes.HeavyImpact);
        }

        #endregion

        #region FadeInOut

        private void FadePanelOut(CanvasGroup panel)
        {
            panel.DOFade(0, 0.75f);
            panel.blocksRaycasts = false;
        }

        private void FadePanelIn(CanvasGroup panel)
        {
            panel.DOFade(1, 0.75f);
            panel.blocksRaycasts = true;
        }

        public void FadeOutScreen(TweenCallback callback, float duration)
        {
            screenFader.DOFade(1, duration).From(0).OnComplete(callback);
        }

        public void FadeOutScreen(float duration)
        {
            screenFader.DOFade(1, duration).From(0);
        }

        public void FadeInScreen(TweenCallback callback, float duration)
        {
            screenFader.DOFade(0, duration).From(1).OnComplete(callback);
        }

        public void FadeInScreen(float duration)
        {
            screenFader.DOFade(0, duration).From(1);
        }

        #endregion

        #region Level Goal Assign
        
        private void SetGoal()
        {
            _dataIndex = SceneManager.GetActiveScene().buildIndex;

            Debug.Log("Goal index: " + _dataIndex);
            for (int i = 0; i < dataLists.Count; i++)
            {
                for (int p = 0; p < dataLists[_dataIndex].goalDataList.Count; p++)
                {
                    goalDisplayControllers[p].SetGoal(dataLists[_dataIndex].goalDataList[p]);
                }
            }
        }

        public void TriggerBuildingArrivedEvent(BuildingType type)
        {
            BuildingArrivedEvent?.Invoke(type);
            HapticManager.instance.TriggerHaptic(HapticTypes.MediumImpact);
        }

        public void TriggerBuildingAPickedEvent(BuildingType type)
        {
            BuildingPickedEvent?.Invoke(type);
            HapticManager.instance.TriggerHaptic(HapticTypes.MediumImpact);
        }

        public void CheckIfGoalsMet()
        {
            bool gameIsEnded = true;

            for (int p = 0; p < dataLists[_dataIndex].goalDataList.Count; p++)
            {
                if (!goalDisplayControllers[p].goalCompleted)
                {
                    gameIsEnded = false;
                    break;
                }
            }

            if (!gameIsEnded) return;

            GameManager.instance.EndGame(true);
        }

        #endregion
    }
}

[System.Serializable]
public class GoalDataList
{
    public List<GoalData> goalDataList;
}