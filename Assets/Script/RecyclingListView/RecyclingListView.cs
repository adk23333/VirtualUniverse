using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ѭ�������б�
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class RecyclingListView : MonoBehaviour
{
    [Tooltip("�ӽڵ�����")]
    public RecyclingListViewItem ChildObj;
    [Tooltip("�м��")]
    public float RowPadding = 15f;
    [Tooltip("����Ԥ������С�б�߶�")]
    public float PreAllocHeight = 0;

    public enum ScrollPosType
    {
        Top,
        Center,
        Bottom,
    }


    public float VerticalNormalizedPosition
    {
        get => scrollRect.verticalNormalizedPosition;
        set => scrollRect.verticalNormalizedPosition = value;
    }


    /// <summary>
    /// �б�����
    /// </summary>
    protected int rowCount;

    /// <summary>
    /// �б���������ֵʱ����ִ���б����¼���
    /// </summary>
    public int RowCount
    {
        get => rowCount;
        set
        {
            if (rowCount != value)
            {
                rowCount = value;
                // �Ƚ��ù����仯
                ignoreScrollChange = true;
                // ���¸߶�
                UpdateContentHeight();
                // �������ù����仯
                ignoreScrollChange = false;
                // ���¼���item
                ReorganiseContent(true);
            }
        }
    }

    /// <summary>
    /// item���»ص�����ί��
    /// </summary>
    /// <param name="item">�ӽڵ����</param>
    /// <param name="rowIndex">����</param>
    public delegate void ItemDelegate(RecyclingListViewItem item, int rowIndex);

    /// <summary>
    /// item���»ص�����ί��
    /// </summary>
    public ItemDelegate ItemCallback;

    protected ScrollRect scrollRect;
    /// <summary>
    /// ���õ�item����
    /// </summary>
    protected RecyclingListViewItem[] childItems;

    /// <summary>
    /// ѭ���б��У���һ��item���������ʼÿ��item����һ��ԭʼ�����������item��ԭʼ��������childBufferStart
    /// �����б���ѭ�����õģ��������»���ʱ��childBufferStart���0��ʼ��n��Ȼ���ִ�0��ʼ���Դ�����
    /// ��������ϻ��������Ǵ�0��-n���ٴ�0��ʼ���Դ�����
    /// </summary>
    protected int childBufferStart = 0;
    /// <summary>
    /// �б��������item����ʵ����������������һ�������ݣ�����10��item����ǰ����ǵ�60�����ݣ���ôsourceDataRowStart����59��ע��������0��ʼ��
    /// </summary>
    protected int sourceDataRowStart;

    protected bool ignoreScrollChange = false;
    protected float previousBuildHeight = 0;
    protected const int rowsAboveBelow = 1;

    protected virtual void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        ChildObj.gameObject.SetActive(false);
    }


    protected virtual void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
        ignoreScrollChange = false;
    }

    protected virtual void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }


    /// <summary>
    /// ���ⲿ���ã�ǿ��ˢ�������б��������ݱ仯�ˣ�ˢ��һ���б�
    /// </summary>
    public virtual void Refresh()
    {
        ReorganiseContent(true);
    }

    /// <summary>
    /// ���ⲿ���ã�ǿ��ˢ�������б�ľֲ�item
    /// </summary>
    /// <param name="rowStart">��ʼ��</param>
    /// <param name="count">����</param>
    public virtual void Refresh(int rowStart, int count)
    {
        int sourceDataLimit = sourceDataRowStart + childItems.Length;
        for (int i = 0; i < count; ++i)
        {
            int row = rowStart + i;
            if (row < sourceDataRowStart || row >= sourceDataLimit)
                continue;

            int bufIdx = WrapChildIndex(childBufferStart + row - sourceDataRowStart);
            if (childItems[bufIdx] != null)
            {
                UpdateChild(childItems[bufIdx], row);
            }
        }
    }

    /// <summary>
    /// ���ⲿ���ã�ǿ��ˢ�������б��ĳһ��item
    /// </summary>
    public virtual void Refresh(RecyclingListViewItem item)
    {

        for (int i = 0; i < childItems.Length; ++i)
        {
            int idx = WrapChildIndex(childBufferStart + i);
            if (childItems[idx] != null && childItems[idx] == item)
            {
                UpdateChild(childItems[i], sourceDataRowStart + i);
                break;
            }
        }
    }

    /// <summary>
    /// ����б�
    /// </summary>
    public virtual void Clear()
    {
        RowCount = 0;
    }


    /// <summary>
    /// ���ⲿ���ã�ǿ�ƹ����б�ʹĳһ����ʾ���б���
    /// </summary>
    /// <param name="row">�к�</param>
    /// <param name="posType">Ŀ������ʾ���б��λ�ã����������ģ��ײ�</param>
    public virtual void ScrollToRow(int row, ScrollPosType posType)
    {
        scrollRect.verticalNormalizedPosition = GetRowScrollPosition(row, posType);
    }

    /// <summary>
    /// ��ù�һ���Ĺ���λ�ã���λ�ý�������������ͼ�о���
    /// </summary>
    /// <param name="row">�к�</param>
    /// <returns></returns>
    public float GetRowScrollPosition(int row, ScrollPosType posType)
    {
        // ��ͼ��
        float vpHeight = ViewportHeight();
        float rowHeight = RowHeight();
        // ��Ŀ���й������б�Ŀ��λ��ʱ���б�����λ��
        float vpTop = 0;
        switch (posType)
        {
            case ScrollPosType.Top:
                {
                    vpTop = row * rowHeight;
                }
                break;
            case ScrollPosType.Center:
                {
                    // Ŀ���е�����λ�����б����ľ���
                    float rowCentre = (row + 0.5f) * rowHeight;
                    // �ӿ�����λ��
                    float halfVpHeight = vpHeight * 0.5f;

                    vpTop = Mathf.Max(0, rowCentre - halfVpHeight);
                }
                break;
            case ScrollPosType.Bottom:
                {
                    vpTop = (row + 1) * rowHeight - vpHeight;
                }
                break;
        }


        // �������б�ײ���λ��
        float vpBottom = vpTop + vpHeight;
        // �б������ܸ߶�
        float contentHeight = scrollRect.content.sizeDelta.y;
        // ����������б�ײ���λ���Ѿ��������б��ܸ߶ȣ�������б�����λ��
        if (vpBottom > contentHeight)
            vpTop = Mathf.Max(0, vpTop - (vpBottom - contentHeight));

        // ����ֵ����������ֵ֮���Lerp������Ҳ����value��from��to֮��ı���ֵ
        return Mathf.InverseLerp(contentHeight - vpHeight, 0, vpTop);
    }

    /// <summary>
    /// �����кŻ�ȡ���õ�item����
    /// </summary>
    /// <param name="row">�к�</param>
    protected RecyclingListViewItem GetRowItem(int row)
    {
        if (childItems != null &&
            row >= sourceDataRowStart && row < sourceDataRowStart + childItems.Length &&
            row < rowCount)
        {
            // ע������Ҫ�����кż��㸴�õ�itemԭʼ����
            return childItems[WrapChildIndex(childBufferStart + row - sourceDataRowStart)];
        }

        return null;
    }

    protected virtual bool CheckChildItems()
    {
        // �б��ӿڸ߶�
        float vpHeight = ViewportHeight();
        float buildHeight = Mathf.Max(vpHeight, PreAllocHeight);
        bool rebuild = childItems == null || buildHeight > previousBuildHeight;
        if (rebuild)
        {

            int childCount = Mathf.RoundToInt(0.5f + buildHeight / RowHeight());
            childCount += rowsAboveBelow * 2;

            if (childItems == null)
                childItems = new RecyclingListViewItem[childCount];
            else if (childCount > childItems.Length)
                Array.Resize(ref childItems, childCount);

            // ����item
            for (int i = 0; i < childItems.Length; ++i)
            {
                if (childItems[i] == null)
                {
                    var item = Instantiate(ChildObj);
                    childItems[i] = item;
                }
                childItems[i].RectTransform.SetParent(scrollRect.content, false);
                childItems[i].gameObject.SetActive(false);
            }

            previousBuildHeight = buildHeight;
        }

        return rebuild;
    }


    /// <summary>
    /// �б����ʱ����ص��˺���
    /// </summary>
    /// <param name="normalisedPos">��һ����λ��</param>
    protected virtual void OnScrollChanged(Vector2 normalisedPos)
    {
        if (!ignoreScrollChange)
        {
            ReorganiseContent(false);
        }
    }

    /// <summary>
    /// ���¼����б�����
    /// </summary>
    /// <param name="clearContents">�Ƿ�Ҫ����б����¼���</param>
    protected virtual void ReorganiseContent(bool clearContents)
    {

        if (clearContents)
        {
            scrollRect.StopMovement();
            scrollRect.verticalNormalizedPosition = 1;
        }

        bool childrenChanged = CheckChildItems();
        // �Ƿ�Ҫ���������б�
        bool populateAll = childrenChanged || clearContents;


        float ymin = scrollRect.content.localPosition.y;

        // ��һ���ɼ�item������
        int firstVisibleIndex = (int)(ymin / RowHeight());


        int newRowStart = firstVisibleIndex - rowsAboveBelow;

        // �����仯��
        int diff = newRowStart - sourceDataRowStart;
        if (populateAll || Mathf.Abs(diff) >= childItems.Length)
        {

            sourceDataRowStart = newRowStart;
            childBufferStart = 0;
            int rowIdx = newRowStart;
            foreach (var item in childItems)
            {
                UpdateChild(item, rowIdx++);
            }

        }
        else if (diff != 0)
        {
            int newBufferStart = (childBufferStart + diff) % childItems.Length;

            if (diff < 0)
            {
                // ��ǰ����
                for (int i = 1; i <= -diff; ++i)
                {
                    // �õ�����item������
                    int wrapIndex = WrapChildIndex(childBufferStart - i);
                    int rowIdx = sourceDataRowStart - i;
                    UpdateChild(childItems[wrapIndex], rowIdx);
                }
            }
            else
            {
                // ��󻬶�
                int prevLastBufIdx = childBufferStart + childItems.Length - 1;
                int prevLastRowIdx = sourceDataRowStart + childItems.Length - 1;
                for (int i = 1; i <= diff; ++i)
                {
                    int wrapIndex = WrapChildIndex(prevLastBufIdx + i);
                    int rowIdx = prevLastRowIdx + i;
                    UpdateChild(childItems[wrapIndex], rowIdx);
                }
            }

            sourceDataRowStart = newRowStart;

            childBufferStart = newBufferStart;
        }
    }

    private int WrapChildIndex(int idx)
    {
        while (idx < 0)
            idx += childItems.Length;

        return idx % childItems.Length;
    }

    /// <summary>
    /// ��ȡһ�еĸ߶ȣ�ע��Ҫ����RowPadding
    /// </summary>
    private float RowHeight()
    {
        return RowPadding + ChildObj.RectTransform.rect.height;
    }

    /// <summary>
    /// ��ȡ�б��ӿڵĸ߶�
    /// </summary>
    private float ViewportHeight()
    {
        return scrollRect.viewport.rect.height;
    }

    protected virtual void UpdateChild(RecyclingListViewItem child, int rowIdx)
    {
        if (rowIdx < 0 || rowIdx >= rowCount)
        {
            child.gameObject.SetActive(false);
        }
        else
        {
            if (ItemCallback == null)
            {
                Debug.Log("RecyclingListView is missing an ItemCallback, cannot function", this);
                return;
            }

            // �ƶ�����ȷ��λ��
            var childRect = ChildObj.RectTransform.rect;
            Vector2 pivot = ChildObj.RectTransform.pivot;
            float ytoppos = RowHeight() * rowIdx;
            float ypos = ytoppos + (1f - pivot.y) * childRect.height;
            float xpos = 0 + pivot.x * childRect.width;
            child.RectTransform.anchoredPosition = new Vector2(xpos, -ypos);
            child.NotifyCurrentAssignment(this, rowIdx);

            // ��������
            ItemCallback(child, rowIdx);

            child.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// ����content�ĸ߶�
    /// </summary>
    protected virtual void UpdateContentHeight()
    {
        // �б�߶�
        float height = ChildObj.RectTransform.rect.height * rowCount + (rowCount - 1) * RowPadding;
        // ����content�ĸ߶�
        var sz = scrollRect.content.sizeDelta;
        scrollRect.content.sizeDelta = new Vector2(sz.x, height);
    }

    protected virtual void DisableAllChildren()
    {
        if (childItems != null)
        {
            for (int i = 0; i < childItems.Length; ++i)
            {
                childItems[i].gameObject.SetActive(false);
            }
        }
    }
}
