using System.Text;
using System.Collections.Generic;
using System;
using BepInEx;
using BepInEx.Bootstrap;
using GorillaLocomotion;
using UnityEngine;
using GorillaInfoWatch.Attributes;
using GorillaInfoWatch.Models;
using GorillaInfoWatch.Models.Widgets;
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
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
            {
                {
                    PluginInfo.HashKey, PluginInfo.Version
                }
            });
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
                lineRenderer.enabled = true;

                var hits = Physics.RaycastAll(
                    handTransform.position,
                    handTransform.forward,
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
    [ShowOnHomeScreen]
    internal class InfoWatchPage : GorillaInfoWatch.Models.InfoWatchScreen
    {
        public override string Title => "Monke Click";

        public override ScreenContent GetContent()
        {
            var lines = new LineBuilder();

            lines.Add($"Status: {(Plugin.active ? "Enabled" : "Disabled")}",
                new List<Widget_Base> { new Widget_PushButton(OnToggleActive) });

            lines.Add($"Color: {Plugin.colors[Plugin.colorIndex].Item1}",
                new List<Widget_Base>
                {
                new Widget_PushButton(OnPreviousColor),
                new Widget_PushButton(OnNextColor)
                });

            lines.Add($"Hand: {(Plugin.useRightHand ? "Right" : "Left")}",
                new List<Widget_Base> { new Widget_PushButton(OnToggleHand) });

            return lines;
        }

        private void OnToggleActive(object[] args)
        {
            Plugin.active = !Plugin.active;
            SetContent();
        }

        private void OnPreviousColor(object[] args)
        {
            Plugin.colorIndex = (Plugin.colorIndex - 1 + Plugin.colors.Count) % Plugin.colors.Count;
            Plugin.RefreshColor(Plugin.colors[Plugin.colorIndex].Item2);
            SetContent();
        }

        private void OnNextColor(object[] args)
        {
            Plugin.colorIndex = (Plugin.colorIndex + 1) % Plugin.colors.Count;
            Plugin.RefreshColor(Plugin.colors[Plugin.colorIndex].Item2);
            SetContent();
        }

        private void OnToggleHand(object[] args)
        {
            Plugin.useRightHand = !Plugin.useRightHand;
            SetContent();
        }
    }
}
