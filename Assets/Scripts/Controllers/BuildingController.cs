using Data;
using DG.Tweening;
using EssentialManagers.Scripts;
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

            AudioManager.instance.Play(SoundTag.Building_Picked);
            CanvasManager.instance.TriggerBuildingAPickedEvent(type);
            
            SetPoint(point, true);
        }

        private void SetPoint(PlacementPoint newPoint, bool performMoving)
        {
            if (currentPoint != null) currentPoint.SetFree();
            currentPoint = newPoint;
            transform.SetParent(currentPoint.transform);
            currentPoint.SetOccupied(this);


            if (!performMoving) return;

            MoveToPoint(currentPoint);
        }

        private void MoveToPoint(PlacementPoint targetPoint)
        {
            Sequence sq = DOTween.Sequence();

            sq.Append(transform.DOLocalMoveY(transform.localPosition.y + 5, 0.1f)
                .OnComplete(() => transform.SetParent(currentPoint.transform)));

            sq.Join(transform.DOBlendableRotateBy(Vector3.up * 360f, 0.25f, RotateMode.FastBeyond360));

            sq.Append(transform.DOLocalJump(targetPoint.GetCenterOffset(),
                1, 1, .35f));

            sq.Join(transform.DOScale(Vector3.one / 7f, 0.25f));

            sq.OnComplete(() =>
            {
                currentPoint.AnimateUpDown();
                AudioManager.instance.Play(SoundTag.Building_Arrived);
                CanvasManager.instance.TriggerBuildingArrivedEvent(type);
                CollectionManager.instance.OnNewBuildingArrived(this);
            });
        }


        public void Merge(BuildingController centerB, PlacementPoint centerPoint)
        {
            currentPoint.SetFree();

            float jumpDuration = 0.25f;
            Sequence sq = DOTween.Sequence();
            sq.Append(transform.DOLocalMoveY(transform.localPosition.y + 5, 0.25f));
            if (centerB != this)
            {
                transform.SetParent(centerPoint.transform);
                sq.Append(transform.DOLocalJump(Vector3.up * 3,
                    5, 1, jumpDuration));
            }
            else
            {
                Debug.Log("Center building plays the audio!");
                AudioManager.instance.Play(SoundTag.Building_Merged, jumpDuration / 1.5f);
            }

            sq.Append(transform.DOScale(Vector3.zero, 0.25f));
            sq.OnComplete(() => { Destroy(gameObject); });
        }

        public void RepositionSelf(PlacementPoint targetPoint)
        {
            //  transform.SetParent(targetPoint.transform);
            SetPoint(targetPoint, false);
            transform.DOLocalJump(Vector3.zero + targetPoint.GetCenterOffset(),
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