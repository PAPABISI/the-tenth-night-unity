using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemotePlayers2DPresenter : MonoBehaviour
{
    [Header("Refs")]
    public GameStateStore store;
    public Transform spawnRoot;
    public GameObject remotePlayerPrefab;

    [Header("Layout")]
    public Vector2 center = Vector2.zero;
    public float ringRadius = 3.2f;

    [Header("Style")]
    public string playerLayerName = "Player";
    public Color aliveColor = new Color(0.24f, 0.75f, 0.35f);
    public Color deadColor = new Color(0.4f, 0.4f, 0.4f);
    public float markerScale = 0.55f;

    [Header("Refresh")]
    public float refreshInterval = 0.25f;
    public float moveLerpSpeed = 12f;

    private readonly Dictionary<string, RemoteView> _views = new();
    private float _timer;

    private class RemoteView
    {
        public GameObject Root;
        public SpriteRenderer Sprite;
        public TextMeshPro Label;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < refreshInterval) return;
        _timer = 0f;

        RefreshFromState();
    }

    public void RefreshFromState()
    {
        var st = store?.LatestState;
        if (st?.publics == null) return;

        var remotePlayers = new List<PlayerPublicView>();
        foreach (var p in st.publics)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.playerId)) continue;
            if (p.playerId == store.LocalPlayerId) continue;
            remotePlayers.Add(p);
        }

        var activeIds = new HashSet<string>();
        for (int i = 0; i < remotePlayers.Count; i++)
        {
            var p = remotePlayers[i];
            activeIds.Add(p.playerId);

            if (!_views.TryGetValue(p.playerId, out var view))
            {
                view = CreateRemoteView(p.playerId);
                _views[p.playerId] = view;
            }

            ApplyRemoteView(view, p, i, remotePlayers.Count);
        }

        var toRemove = new List<string>();
        foreach (var kv in _views)
        {
            if (!activeIds.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }

        foreach (var id in toRemove)
        {
            if (_views[id].Root != null)
                Destroy(_views[id].Root);
            _views.Remove(id);
        }
    }

    private RemoteView CreateRemoteView(string playerId)
    {
        GameObject go;
        if (remotePlayerPrefab != null)
        {
            go = Instantiate(remotePlayerPrefab, spawnRoot != null ? spawnRoot : transform);
            go.name = $"RemotePlayer_{playerId[..Mathf.Min(6, playerId.Length)]}";
        }
        else
        {
            go = new GameObject($"RemotePlayer_{playerId[..Mathf.Min(6, playerId.Length)]}");
            go.transform.SetParent(spawnRoot != null ? spawnRoot : transform, false);

            var sprite = go.AddComponent<SpriteRenderer>();
            sprite.sprite = BuildDefaultSprite();
            go.transform.localScale = new Vector3(markerScale, markerScale, 1f);

            if (go.GetComponent<CircleCollider2D>() == null)
            {
                var col = go.AddComponent<CircleCollider2D>();
                col.radius = 0.6f;
                col.isTrigger = true;
            }

            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(go.transform, false);
            labelObj.transform.localPosition = new Vector3(0f, 0.8f, 0f);

            var label = labelObj.AddComponent<TextMeshPro>();
            label.fontSize = 2.8f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
        }

        var layer = LayerMask.NameToLayer(playerLayerName);
        if (layer >= 0) go.layer = layer;

        return new RemoteView
        {
            Root = go,
            Sprite = go.GetComponent<SpriteRenderer>(),
            Label = go.GetComponentInChildren<TextMeshPro>()
        };
    }

    private void ApplyRemoteView(RemoteView view, PlayerPublicView model, int index, int total)
    {
        if (view.Root == null) return;

        var hasNetworkPos = model.hasPosition;
        var target = hasNetworkPos
            ? new Vector3(model.x, model.y, 0f)
            : new Vector3(ResolveSlotPosition(index, total).x, ResolveSlotPosition(index, total).y, 0f);

        var t = Mathf.Clamp01(Time.deltaTime * moveLerpSpeed);
        view.Root.transform.position = Vector3.Lerp(view.Root.transform.position, target, t);

        if (view.Sprite != null)
            view.Sprite.color = model.isAlive ? aliveColor : deadColor;

        if (view.Label != null)
        {
            var life = model.isAlive ? "Alive" : "Dead";
            view.Label.text = $"{model.displayName}\n({life})";
        }
    }

    private Vector2 ResolveSlotPosition(int index, int total)
    {
        if (total <= 0) return center;

        var angleStep = Mathf.PI * 2f / total;
        var angle = angleStep * index;

        return new Vector2(
            center.x + Mathf.Cos(angle) * ringRadius,
            center.y + Mathf.Sin(angle) * ringRadius
        );
    }

    private static Sprite BuildDefaultSprite()
    {
        var tex = Texture2D.whiteTexture;
        return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}
