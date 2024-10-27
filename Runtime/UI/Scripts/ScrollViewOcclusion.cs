using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Nach.Tools.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollViewOcclusion : MonoBehaviour
    {
        private RectTransform mainParentBounds = null;
        private RectTransform contentTransform = null;
        
        private ScrollRect scrollRect = null;
        private LayoutGroup activeLayout = null;
        private ContentSizeFitter contentSizeFitter = null;

        private Vector3[] mainCorners = new Vector3[4];
        private Vector3[] auxCorners = new Vector3[4];
        private Bounds mainBounds = default;

        private int currentChildCount = 0;
        private bool occlusionStatus = false;

        private void Awake()
        {
            ConfigureScrollRect();
            ConfigureLayoutReferences();

            ToggleOcclusion(true);
            OccludeElements();
        }

        private void Update()
        {
            RefreshContentChildren();
        }

        private void OnEnable()
        {
            CalculateMainBounds();
        }

        private async void RefreshContentChildren()
        {
            int updatedChildCount = contentTransform.childCount;
            if (updatedChildCount != currentChildCount)
            {
                ToggleLayoutGridComponentsStatus(true);
                //LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
                for (int i = 0; i < contentTransform.childCount; i++)
                {
                    if (contentTransform.GetChild(i).TryGetComponent(out RectTransform item))
                    {
                        item.gameObject.SetActive(true);
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
                await ToggleOcclusion(true);
                OccludeElements();
                currentChildCount = updatedChildCount;
            }
        }

        private void ConfigureScrollRect()
        {
            scrollRect = GetComponent<ScrollRect>();
            mainParentBounds = scrollRect.transform as RectTransform;
            contentTransform = scrollRect.content;
            scrollRect.onValueChanged.AddListener(OnScroll);
        }

        private void ConfigureLayoutReferences()
        {
            contentTransform.TryGetComponent(out activeLayout);
            contentTransform.TryGetComponent(out contentSizeFitter);
        }

        public async Task ToggleOcclusion(bool status)
        {
            if (!status)
            {
                occlusionStatus = false;
                ToggleLayoutGridComponentsStatus(true);
            }
            else
            {
                await Task.Delay(10);
                occlusionStatus = true;
                ToggleLayoutGridComponentsStatus(true);
            }
        }

        private void ToggleLayoutGridComponentsStatus(bool status)
        {
            if (contentSizeFitter != null)
            {
                contentSizeFitter.enabled = status;
            }
            if (activeLayout != null)
            {
                activeLayout.enabled = status;
            }
        }

        private void CalculateMainBounds()
        {
            mainParentBounds.GetLocalCorners(mainCorners);
            mainBounds = GeometryUtility.CalculateBounds(mainCorners, mainParentBounds.localToWorldMatrix);
        }

        private void OnScroll(Vector2 position)
        {
            OccludeElements();
        }

        private void OccludeElements()
        {
            if (!occlusionStatus)
            {
                CalculateMainBounds();
                return;
            }

            ToggleLayoutGridComponentsStatus(false);

            for (int i = 0; i < contentTransform.childCount; i++)
            {
                if (!contentTransform.GetChild(i).TryGetComponent(out RectTransform item))
                {
                    continue;
                }

                item.GetLocalCorners(auxCorners);
                Bounds rectBounds = GeometryUtility.CalculateBounds(auxCorners, item.localToWorldMatrix);

                bool isWithinBounds = mainBounds.Intersects(rectBounds);
                item.gameObject.SetActive(isWithinBounds);
            }
        }
    }
}
