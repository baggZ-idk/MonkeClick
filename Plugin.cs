using System.Text;
using BepInEx;
using GorillaLocomotion;
using UnityEngine;
using BananaOS.Pages;
using BananaOS;
using System;
using System.Collections.Generic;

//Yes, I'm a Canadian, I spell it "colour" not "color", go cry about it.

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

        public static void RefreshColour(Color newColour, LineRenderer line, int newColourIndex)
        {
            line.startColor = newColour;
            line.endColor = newColour;
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
        public static int colourIndex = 0;
        public static List<Tuple<string, Color>> colours = new List<Tuple<string, Color>>
        {
            Tuple.Create("white", Color.white),
            Tuple.Create("red", Color.red),
            Tuple.Create("green", Color.green),
            Tuple.Create("blue", Color.blue),
            Tuple.Create("yellow", Color.yellow),
            Tuple.Create("cyan", Color.cyan),
            Tuple.Create("magenta", Color.magenta),
            Tuple.Create("black", Color.black)
            //if you want more colors make a pull request with the colors you want.
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
                stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, $"colour: {colors[colorIndex].Item1}"));
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
                        colourIndex += 1;

                        if (colourIndex > colours.Count - 1)
                        {
                            colourIndex = 0;
                        }
                        Plugin.RefreshColour(colours[colorIndex].Item2, Plugin.lineRenderer, colourIndex);
                    }

                        break;

                    case WatchButtonType.Back:
                        ReturnToMainMenu();
                        break;
                }
            }
        }
    }
