using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class RectTransformUtility
{
    // 幅を返します
    public static float GetWidth(this RectTransform self)
    {
        return self.sizeDelta.x;
    }
    
    // 高さを返します
    public static float GetHeight(this RectTransform self)
    {
        return self.sizeDelta.y;
    }

    // 幅を設定します
    public static void SetWidth(this RectTransform self, float width)
    {
        var size = self.sizeDelta;
        size.x = width;
        self.sizeDelta = size;
    }
    
    // 高さを設定します
    public static void SetHeight(this RectTransform self, float height)
    {
        var size = self.sizeDelta;
        size.y = height;
        self.sizeDelta = size;
    }
    
    // サイズを設定します
    public static void SetSize(this RectTransform self, float width, float height)
    {
        self.sizeDelta = new Vector2(width, height);
    }
}
