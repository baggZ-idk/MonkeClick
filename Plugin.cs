using System.Text;
using BepInEx;
using GorillaLocomotion;
using UnityEngine;
using BananaOS.Pages;
using BananaOS;
using System;
using System.Collections.Generic;

//Yes, I'm an American I spell it "color" not "colour" go cry about it.

namespace MonkeClick
{
        [BepInPlugin("baggz.monkeclick", "MonkeClick", "1.0.0")]
        public class Plugin : BaseUnityPlugin
        {
            bool inRoom;
            public static LineRenderer lineRenderer;


            void Start()
            {
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

        public static void RefreshColor(Color newColor, LineRenderer line, int newColorIndex)
        {
            line.startColor = newColor;
            line.endColor = newColor;
        }

            void FixedUpdate()
            {
                lineRenderer.SetPosition(0, GTPlayer.Instance.rightControllerTransform.position);

                if (ControllerInputPoller.instance.rightGrab && Page.active)
                {
                    lineRenderer.enabled = true;
                    
                    var hits = Physics.RaycastAll(GTPlayer.Instance.rightControllerTransform.position, GTPlayer.Instance.rightControllerTransform.forward, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Gorilla Boundary")));

                    System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                    foreach (var hit in hits)
                    {
                            lineRenderer.enabled = true;
                            lineRenderer.SetPosition(1, hit.point);
                            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f && hit.transform.gameObject.layer == 18)
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
        }

    public class Page : WatchPage
    {
        public override string Title => "Monke Click";
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
        
            Tuple.Create("orange", new Color(1f, 0.5f, 0f)),
            Tuple.Create("purple", new Color(0.5f, 0f, 0.5f)),
            Tuple.Create("pink", new Color(1f, 0.75f, 0.8f)),
            Tuple.Create("brown", new Color(0.6f, 0.3f, 0f)),
            Tuple.Create("gray", Color.gray),
            Tuple.Create("light gray", new Color(0.8f, 0.8f, 0.8f)),
            Tuple.Create("dark gray", new Color(0.3f, 0.3f, 0.3f)),
            Tuple.Create("lime", new Color(0.75f, 1f, 0f)),
            Tuple.Create("olive", new Color(0.5f, 0.5f, 0f)),
            Tuple.Create("maroon", new Color(0.5f, 0f, 0f)),
            Tuple.Create("navy", new Color(0f, 0f, 0.5f)),
            Tuple.Create("teal", new Color(0f, 0.5f, 0.5f)),
            Tuple.Create("aqua", new Color(0f, 1f, 1f)),
            Tuple.Create("gold", new Color(1f, 0.84f, 0f)),
            Tuple.Create("silver", new Color(0.75f, 0.75f, 0.75f)),
            Tuple.Create("beige", new Color(0.96f, 0.96f, 0.86f)),
            Tuple.Create("coral", new Color(1f, 0.5f, 0.31f)),
            Tuple.Create("salmon", new Color(0.98f, 0.5f, 0.45f)),
            Tuple.Create("chocolate", new Color(0.82f, 0.41f, 0.12f)),
            Tuple.Create("crimson", new Color(0.86f, 0.08f, 0.24f)),
            Tuple.Create("indigo", new Color(0.29f, 0f, 0.51f)),
            Tuple.Create("ivory", new Color(1f, 1f, 0.94f)),
            Tuple.Create("khaki", new Color(0.94f, 0.9f, 0.55f)),
            Tuple.Create("lavender", new Color(0.9f, 0.9f, 0.98f)),
            Tuple.Create("mint", new Color(0.74f, 0.99f, 0.79f)),
            Tuple.Create("orchid", new Color(0.85f, 0.44f, 0.84f)),
            Tuple.Create("plum", new Color(0.87f, 0.63f, 0.87f)),
            Tuple.Create("rose", new Color(1f, 0f, 0.5f)),
            Tuple.Create("scarlet", new Color(1f, 0.14f, 0f)),
            Tuple.Create("tan", new Color(0.82f, 0.71f, 0.55f)),
            Tuple.Create("turquoise", new Color(0.25f, 0.88f, 0.82f)),
            Tuple.Create("violet", new Color(0.93f, 0.51f, 0.93f)),
            Tuple.Create("amber", new Color(1f, 0.75f, 0f)),
            Tuple.Create("azure", new Color(0f, 0.5f, 1f)),
            Tuple.Create("baby blue", new Color(0.54f, 0.81f, 0.94f)),
            Tuple.Create("burgundy", new Color(0.5f, 0f, 0.13f)),
            Tuple.Create("charcoal", new Color(0.21f, 0.27f, 0.31f)),
            Tuple.Create("cream", new Color(1f, 0.99f, 0.82f)),
            Tuple.Create("emerald", new Color(0.31f, 0.78f, 0.47f)),
            Tuple.Create("jade", new Color(0f, 0.66f, 0.42f)),
            Tuple.Create("lavender blush", new Color(1f, 0.94f, 0.96f)),
            Tuple.Create("lemon", new Color(1f, 1f, 0.6f)),
            Tuple.Create("light coral", new Color(0.94f, 0.5f, 0.5f)),
            Tuple.Create("light salmon", new Color(1f, 0.63f, 0.48f)),
            Tuple.Create("light sea green", new Color(0.13f, 0.7f, 0.67f)),
            Tuple.Create("light sky blue", new Color(0.53f, 0.81f, 0.98f)),
            Tuple.Create("light steel blue", new Color(0.69f, 0.77f, 0.87f)),
            Tuple.Create("lime green", new Color(0.2f, 0.8f, 0.2f)),
            Tuple.Create("medium aquamarine", new Color(0.4f, 0.8f, 0.67f)),
            Tuple.Create("medium orchid", new Color(0.73f, 0.33f, 0.83f)),
            Tuple.Create("medium purple", new Color(0.58f, 0.44f, 0.86f)),
            Tuple.Create("medium sea green", new Color(0.24f, 0.7f, 0.44f)),
            Tuple.Create("medium slate blue", new Color(0.48f, 0.41f, 0.93f)),
            Tuple.Create("medium spring green", new Color(0f, 0.98f, 0.6f)),
            Tuple.Create("medium turquoise", new Color(0.28f, 0.82f, 0.8f)),
            Tuple.Create("midnight blue", new Color(0.1f, 0.1f, 0.44f)),
            Tuple.Create("moccasin", new Color(1f, 0.89f, 0.71f)),
            Tuple.Create("navajo white", new Color(1f, 0.87f, 0.68f)),
            Tuple.Create("old lace", new Color(0.99f, 0.96f, 0.9f)),
            Tuple.Create("pale goldenrod", new Color(0.93f, 0.91f, 0.67f)),
            Tuple.Create("pale green", new Color(0.6f, 0.98f, 0.6f)),
            Tuple.Create("pale turquoise", new Color(0.69f, 0.93f, 0.93f)),
            Tuple.Create("pale violet red", new Color(0.86f, 0.44f, 0.58f)),
            Tuple.Create("papaya whip", new Color(1f, 0.94f, 0.84f)),
            Tuple.Create("peach puff", new Color(1f, 0.85f, 0.73f)),
            Tuple.Create("powder blue", new Color(0.69f, 0.88f, 0.9f)),
            Tuple.Create("rosy brown", new Color(0.74f, 0.56f, 0.56f)),
            Tuple.Create("royal blue", new Color(0.25f, 0.41f, 0.88f)),
            Tuple.Create("saddle brown", new Color(0.55f, 0.27f, 0.07f)),
            Tuple.Create("sandy brown", new Color(0.96f, 0.64f, 0.38f)),
            Tuple.Create("sea green", new Color(0.18f, 0.55f, 0.34f)),
            Tuple.Create("seashell", new Color(1f, 0.96f, 0.93f)),
            Tuple.Create("sienna", new Color(0.63f, 0.32f, 0.18f)),
            Tuple.Create("sky blue", new Color(0.53f, 0.81f, 0.92f)),
            Tuple.Create("slate blue", new Color(0.42f, 0.35f, 0.8f)),
            Tuple.Create("slate gray", new Color(0.44f, 0.5f, 0.56f)),
            Tuple.Create("snow", new Color(1f, 0.98f, 0.98f)),
            Tuple.Create("spring green", new Color(0f, 1f, 0.5f)),
            Tuple.Create("steel blue", new Color(0.27f, 0.51f, 0.71f)),
            Tuple.Create("thistle", new Color(0.85f, 0.75f, 0.85f)),
            Tuple.Create("tomato", new Color(1f, 0.39f, 0.28f)),
            Tuple.Create("wheat", new Color(0.96f, 0.87f, 0.7f)),
            Tuple.Create("yellow green", new Color(0.6f, 0.8f, 0.2f)),
        };

            public override bool DisplayOnMainMenu => true;

            public override void OnPostModSetup()
            {
                selectionHandler.maxIndex = 1;
            }

            public override string OnGetScreenContent()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"<color=yellow>==</color> Monke Click <color=yellow>==</color>");
                stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, $"activity: {(active?"enabled":"disabled")}"));
                stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, $"color: {colors[colorIndex].Item1}"));
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

                        if (colorIndex > colors.Count - 1)
                        {
                            colorIndex = 0;
                        }
                        Plugin.RefreshColor(colors[colorIndex].Item2, Plugin.lineRenderer, colorIndex);
                    }

                        break;

                    case WatchButtonType.Back:
                        ReturnToMainMenu();
                        break;
                }
            }
        }
    }
