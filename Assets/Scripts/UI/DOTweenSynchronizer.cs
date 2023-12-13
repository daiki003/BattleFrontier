using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// DOTweenの再生タイミングを同期
/// </summary>
public static class DOTweenSynchronizer
{
    private static Dictionary<string, HashSet<Tween>> _tweensDict;

    static DOTweenSynchronizer()
    {
        _tweensDict = new Dictionary<string, HashSet<Tween>>();
    }

    /// <summary>
    /// 登録
    /// 同期キーを指定しない場合トゥイーンのdurationが同じものが同期対象になります
    /// トゥイーンターゲットのGameObjectがアクティブなものが同期対象になるのでSetTarget()でのターゲット指定を推奨
    /// </summary>
    /// <param name="tween">対象トゥイーン</param>
    /// <param name="key">同期キー</param>
    /// <param name="autoUnregister">Killしたときに自動的に同期対象から登録解除</param>
    /// <returns>Tween</returns>
    public static Tween Register(Tween tween, string key = null, bool autoUnregister = true)
    {
        if (tween == null || !tween.active) return tween;

        if (string.IsNullOrEmpty(key))
        {
            key = GetDefaultSyncKey(tween);
        }

        HashSet<Tween> tweens;
        if (!_tweensDict.TryGetValue(key, out tweens))
        {
            tweens = new HashSet<Tween>();
            _tweensDict.Add(key, tweens);
        }
        tweens.Add(tween);

        // Killで同期対象から解除
        if (autoUnregister)
        {
            tween.OnKill(() => Unregister(tween));
        }

        return tween;
    }

    /// <summary>
    /// 解除
    /// </summary>
    /// <param name="tween">対象トゥイーン</param>
    /// <param name="key">同期キー</param>
    public static void Unregister(Tween tween, string key = null)
    {
        if (tween == null) return;

        if (tween.active)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = GetDefaultSyncKey(tween);
            }

            if (_tweensDict.TryGetValue(key, out var tweens))
            {
                tweens.Remove(tween);

                // 0件になったHashSetは削除
                if (tweens.Count == 0)
                {
                    _tweensDict.Remove(key);
                }
            }
        }
        else
        {
            // tweenがKillされている場合はデフォルトキーが作れないので全件走査で削除
            foreach (var set in _tweensDict.Values)
            {
                set.Remove(tween);
            }

            // 0件になったHashSet削除
            var emptyTweens = _tweensDict.Where(kvp => kvp.Value.Count == 0);
            foreach (var kvp in emptyTweens.Reverse())
            {
                _tweensDict.Remove(kvp.Key);
            }
        }
    }

    /// <summary>
    /// デフォルトの同期キーを取得
    /// トゥイーン時間が同じものを同期対象にする
    /// </summary>
    /// <param name="tween"></param>
    /// <returns>同期キー</returns>
    private static string GetDefaultSyncKey(Tween tween)
    {
        if (tween == null || !tween.active) return null;

        float duration = tween.Duration(false);
        if (Mathf.Approximately(duration, 0f))
        {
            Debug.LogWarning("Durationが0のTweenが指定されました。");
        }
        return $"duration_{duration}";
    }

    /// <summary>
    /// 第一トゥイーンと同期
    /// </summary>
    /// <param name="tween">対象トゥイーン</param>
    /// <param name="key">同期キー</param>
    /// <returns></returns>
    public static Tween SyncWithPrimary(Tween tween, string key = null)
    {
        // 同期トゥイーンがないときはスキップ
        if (_tweensDict.Count == 0) return tween;

        // 同期キーが指定されないときはデフォルトキー (トゥイーン時間ベース)
        if (string.IsNullOrEmpty(key))
        {
            key = GetDefaultSyncKey(tween);
        }

        // 同期キーが該当なしならスキップ
        if (!_tweensDict.ContainsKey(key)) return tween;

        // 他の再生中のトゥイーン一覧
        _tweensDict.TryGetValue(key, out var tweens);

        // 第一トゥイーンを検索
        Tween first = tweens.FirstOrDefault(x =>
            // 同期対象は除く
            x != tween
            // アクティブ (Killされていない)
            && x.active
            // 再生中
            && x.IsPlaying()
            // トゥイーン対象コンポーネントのGameObjectがHierarchyでアクティブ
            && ((x.target as Component) != null && (x.target as Component).gameObject.activeInHierarchy)
        );

        if (first == null) return tween;

        // 再生開始位置
        float position = 0f;

        // 再生中トゥイーンの位置を開始位置に指定
        position = first.position;

        // Yoyoループの逆再生中?
        if (tween.hasLoops && first.IsYoyoBackwards())
        {
            // Yoyoの逆再生中のpositionは逆再生の開始点からの値なので順再生分の秒数を加算
            position += tween.Duration(false);
        }
        // 再生位置設定
        tween.Goto(position);

        return tween;
    }

    /// <summary>
    /// クリア
    /// </summary>
    public static void Clear()
    {
        if (_tweensDict.Count == 0) return;

        foreach (var v in _tweensDict.Values)
        {
            v.Clear();
        }
        _tweensDict.Clear();
    }
}

/// <summary>
/// DOTweenSynchronizer用拡張メソッド
/// </summary>
public static class DOTweenSynchronizerExtensions
{
    /// <summary>
    /// 同期のために登録
    /// </summary>
    /// <param name="tween"></param>
    /// <param name="syncKey">同期キー</param>
    /// <param name="autoUnregister">Killしたときに自動的に同期対象から登録解除</param>
    /// <returns></returns>
    public static Tween RegisterForSync(this Tween tween, string syncKey = null, bool autoUnregister = true)
        => DOTweenSynchronizer.Register(tween, syncKey);

    /// <summary>
    /// 同期するための登録解除
    /// TweenをKillする前に実行してください
    /// </summary>
    /// <param name="tween"></param>
    /// <param name="syncKey">同期キー</param>
    /// <returns></returns>
    public static void UnregisterForSync(this Tween tween, string syncKey = null)
        => DOTweenSynchronizer.Unregister(tween, syncKey);

    /// <summary>
    /// 第一トゥイーンと同期する
    /// </summary>
    /// <param name="tween"></param>
    /// <param name="syncKey">同期キー</param>
    /// <returns></returns>
    public static Tween SyncWithPrimary(this Tween tween, string syncKey = null)
        => DOTweenSynchronizer.SyncWithPrimary(tween, syncKey);

    /// <summary>
    /// Yoyoの逆再生中？
    /// </summary>
    /// <param name="tween"></param>
    /// <returns></returns>
    public static bool IsYoyoBackwards(this Tween tween)
    {
        if (!tween.hasLoops) return false;
        return !Mathf.Approximately(tween.ElapsedPercentage(false), tween.ElapsedDirectionalPercentage());
    }
}