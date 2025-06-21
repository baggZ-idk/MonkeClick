using System.Text;
using System.Collections.Generic;
using System;
using BepInEx;
using BepInEx.Bootstrap;
using GorillaLocomotion;
using UnityEngine;
using BananaOS.Pages;
using BananaOS;
using GorillaInfoWatch.Attributes;
using GorillaInfoWatch.Models;
using GorillaInfoWatch.Models.Widgets;

namespace MonkeClick
{
    [BepInPlugin("baggz.monkeclick", "MonkeClick", "1.0.0")]
    [BepInDependency("Husky.BananaOS", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dev.gorillainfowatch", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static LineRenderer lineRenderer;
        public static bool active = false;
        public static int colorIndex = 0;
        public static List<Tuple<string, Color>> colors = new List<Tuple<string, Color>>
        {
            Tuple.Create("white", Color.white),
            Tuple.Create("red", Color.red),
            Tuple.Create("green", Color.green),
            Tuple.Create("blue", Color.blue),
            Tuple.Create("yellow", Color.yellow),
            Tuple.Create("cyan", Color.cyan),
            Tuple.Create("magenta", Color.magenta),
            Tuple.Create("black", Color.black)
        };

        void Start()
        {
            // Verify BananaOS or GorillaInfoWatch presence
            bool hasBanana = Chainloader.PluginInfos.ContainsKey("Husky.BananaOS");
            bool hasInfo = Chainloader.PluginInfos.ContainsKey("dev.gorillainfowatch");

            if (!hasBanana && !hasInfo)
            {
                Logger.LogError("MonkeClick requires BananaOS or GorillaInfoWatch. Disabling plugin.");
                this.enabled = false;
                return;
            }

            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void OnGameInitialized()
        {
            GameObject lineObj = new GameObject("RaycastLine");
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.enabled = false;
        }

        void FixedUpdate()
        {
            if (lineRenderer == null || GTPlayer.Instance == null) return;

            lineRenderer.SetPosition(0, GTPlayer.Instance.rightControllerTransform.position);

            if (ControllerInputPoller.instance.rightGrab && active)
            {
                lineRenderer.enabled = true;

                var hits = Physics.RaycastAll(
                    GTPlayer.Instance.rightControllerTransform.position,
                    GTPlayer.Instance.rightControllerTransform.forward,
                    Mathf.Infinity,
                    ~(1 << LayerMask.NameToLayer("Gorilla Boundary"))
                );
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                foreach (var hit in hits)
                {
                    lineRenderer.SetPosition(1, hit.point);
                    if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f &&
                        hit.transform.gameObject.layer == 18)
                    {
                        GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                        break;
                    }
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

        public static void RefreshColor(Color newColor)
        {
            if (lineRenderer != null)
            {
                lineRenderer.startColor = newColor;
                lineRenderer.endColor = newColor;
            }
        }
    }

    // BananaOS WatchPage implementation
    
    public class Page : WatchPage
    {
        public override string Title => "Monke Click";
        public override bool DisplayOnMainMenu => true;

        public override void OnPostModSetup()
        {
            selectionHandler.maxIndex = 1;
        }

        public override string OnGetScreenContent()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<color=yellow>==</color> Monke Click <color=yellow>==</color>");
            sb.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0,
                $"Status: {(Plugin.active ? "Enabled" : "Disabled")}"));
            sb.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1,
                $"Colour: {Plugin.colors[Plugin.colorIndex].Item1}"));
            return sb.ToString();
        }

        public override void OnButtonPressed(WatchButtonType buttonType)
        {
            switch (buttonType)
            {
                case WatchButtonType.Up:
                    selectionHandler.MoveSelectionUp();
                    break;
                case WatchButtonType.Down:
                    selectionHandler.MoveSelectionDown();
                    break;
                case WatchButtonType.Enter:
                    if (selectionHandler.currentIndex == 0)
                        Plugin.active = !Plugin.active;
                    else
                    {
                        Plugin.colorIndex = (Plugin.colorIndex + 1) % Plugin.colors.Count;
                        Plugin.RefreshColor(Plugin.colors[Plugin.colorIndex].Item2);
                    }
                    break;
                case WatchButtonType.Back:
                    ReturnToMainMenu();
                    break;
            }
        }
    }

    // GorillaInfoWatch Screen implementation
    [ShowOnHomeScreen]
    internal class InfoWatchPage : GorillaInfoWatch.Models.InfoWatchScreen
    {
        public override string Title => "Monke Click";

        public override ScreenContent GetContent()
        {
            var lines = new LineBuilder();
            lines.Add($"Status: {(Plugin.active ? "Enabled" : "Disabled")}",
    new List<Widget_Base> { new Widget_PushButton(OnToggleActive) });

            lines.Add($"Colour: {Plugin.colors[Plugin.colorIndex].Item1}",
                new List<Widget_Base> { new Widget_PushButton(OnCycleColor) });

            return lines;
        }

        private void OnToggleActive(object[] args)
        {
            Plugin.active = !Plugin.active;
            SetContent();
        }

        private void OnCycleColor(object[] args)
        {
            Plugin.colorIndex = (Plugin.colorIndex + 1) % Plugin.colors.Count;
            Plugin.RefreshColor(Plugin.colors[Plugin.colorIndex].Item2);
            SetContent();
        }
    }
}
