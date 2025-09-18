using System.Text;
using System.Collections.Generic;
using System;
using BepInEx;
using BepInEx.Bootstrap;
using GorillaLocomotion;
using UnityEngine;
using BananaOS;
using BananaOS.Pages;
using System.Linq;
using Photon.Pun;

namespace MonkeClick
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("dev.gorillainfowatch", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static bool useRightHand = true; 
        public static LineRenderer lineRenderer;
        public static bool active = false;
        public static int colorIndex = 0;
        public static List<Tuple<string, Color>> colors = new List<Tuple<string, Color>>

        {
            Tuple.Create("RGB", Color.white),
            Tuple.Create("white", Color.white),
            Tuple.Create("red", Color.red),
            Tuple.Create("green", Color.green),
            Tuple.Create("blue", Color.blue),
            Tuple.Create("yellow", Color.yellow),
            Tuple.Create("cyan", Color.cyan),
            Tuple.Create("magenta", Color.magenta),
            Tuple.Create("black", Color.black),
            Tuple.Create("purple", new Color(0.5f, 0f, 0.5f)),
            Tuple.Create("orange", new Color(1f, 0.5f, 0f)),
            Tuple.Create("pink", new Color(1f, 0.75f, 0.8f)),
            Tuple.Create("brown", new Color(0.6f, 0.3f, 0f)),
            Tuple.Create("gray", Color.gray),
        };

        void Start()
        {
            bool hasInfo = Chainloader.PluginInfos.ContainsKey("dev.gorillainfowatch");

            if (!hasInfo)
            {
                Logger.LogError($"MonkeClick requires GorillaInfoWatch.");
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

            // Hand config stuff
            Transform handTransform = Plugin.useRightHand ? GTPlayer.Instance.rightControllerTransform : GTPlayer.Instance.leftControllerTransform;
            bool grabPressed = Plugin.useRightHand ? ControllerInputPoller.instance.rightGrab : ControllerInputPoller.instance.leftGrab;
            float indexTrigger = Plugin.useRightHand ? ControllerInputPoller.instance.rightControllerIndexFloat : ControllerInputPoller.instance.leftControllerIndexFloat;
            Transform handTriggerCollider = Plugin.useRightHand ? GorillaTagger.Instance.rightHandTriggerCollider.transform : GorillaTagger.Instance.leftHandTriggerCollider.transform;

            lineRenderer.SetPosition(0, handTransform.position);

            if (grabPressed && Plugin.active)
            {
                if (colors[colorIndex].Item1 == "RGB")
                {
                    float hue = (Time.time * 0.2f) % 1f; 
                    Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);

                    RefreshColor(rainbowColor);
                }

                lineRenderer.enabled = true;

                float angleOffset = 30f;

                Vector3 rotatedDirection = Quaternion.AngleAxis(angleOffset, handTransform.right) * handTransform.forward;

                var hits = Physics.RaycastAll(
                    handTransform.position,
                    rotatedDirection,
                    Mathf.Infinity,
                    ~(1 << LayerMask.NameToLayer("Gorilla Boundary"))
                );


                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                foreach (var hit in hits)
                {
                    lineRenderer.SetPosition(1, hit.point);
                    if (indexTrigger > 0.5f && hit.transform.gameObject.layer == 18)
                    {
                        handTriggerCollider.position = hit.point;
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



    // GorillaInfoWatch Screen implementation
    public class Page : WatchPage
    {
        public override string Title => "Monke Click";
        public static bool active = false;
        public static int colorIndex = 0;
        public override bool DisplayOnMainMenu => true;

        public override void OnPostModSetup()
        {
            selectionHandler.maxIndex = 1;
        }

        public override string OnGetScreenContent()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"<color=yellow>==</color> Monke Click <color=yellow>==</color>");
            stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, $"activity: {(active ? "enabled" : "disabled")}"));
            stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, $"color: {Plugin.colors[colorIndex].Item1}"));
            return stringBuilder.ToString();
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
                    if (selectionHandler.currentIndex == 0) active = !active;
                    else
                    {
                        colorIndex += 1;

                        if (colorIndex > Plugin.colors.Count - 1)
                        {
                            colorIndex = 0;
                        }
                        Plugin.RefreshColor(Plugin.colors[colorIndex].Item2);
                    }

                    break;

                case WatchButtonType.Back:
                    ReturnToMainMenu();
                    break;
            }
        }
    }
}
