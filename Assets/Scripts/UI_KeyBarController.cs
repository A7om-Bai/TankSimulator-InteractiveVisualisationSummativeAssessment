using UnityEngine;

public class UI_KeyBarController : MonoBehaviour
{
    public UI_KeyHighlight[] keys = new UI_KeyHighlight[8];

    int currentKey = -1;

    void Start()
    {
        ClearAll();
    }

    public void ToggleKey(int key)
    {
        int index = key - 1;

        // 再次按同一个 → 清除高亮
        if (currentKey == index)
        {
            ClearAll();
            currentKey = -1;
            return;
        }

        ClearAll();
        currentKey = index;

        if (keys[index] != null)
            keys[index].PlayOnce();
    }


    void ClearAll()
    {
        foreach (var k in keys)
            if (k != null)
                k.ResetHighlight();
    }

}
