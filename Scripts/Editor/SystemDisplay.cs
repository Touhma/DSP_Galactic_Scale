using System;
using System.Collections.Generic;
using NGPT;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GalacticScale
{
    public static class SystemDisplay
    {
        public static bool inSystemDisplay = false;
        private static StarData viewStar;
        public static Button randomButton;
        public static Button startButton;
        public static Button backButton;
        public static bool deBounce = false;
        public static int customBirthStar = -1;
        public static int customBirthPlanet = -1;
        public static float mouseTolerance = 1.7f;
        public static void AbortRender(UIVirtualStarmap starmap)
        {
            // Modeler.Reset();
            ShowStarMap(starmap);
        }

        public static void initializeButtons(UIGalaxySelect instance)
        {
            backButton.onClick.RemoveAllListeners();
            randomButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => OnBackClick(instance));
            randomButton.onClick.AddListener(() => OnRandomClick(instance));
        }
        public static void OnUpdate(UIVirtualStarmap starmap)
        {
            if (Input.mouseScrollDelta.y < 0) UIRoot.instance.galaxySelect.cameraPoser.distRatio += 0.1f;
            if (Input.mouseScrollDelta.y > 0) UIRoot.instance.galaxySelect.cameraPoser.distRatio -= 0.1f;
            if (VFInput._moveRight) GameCamera.instance.transform.localPosition += GameCamera.instance.galaxySelectPoser.transform.localRotation * (0.1f * Vector3.right);
            if (VFInput._moveLeft) GameCamera.instance.transform.localPosition += GameCamera.instance.galaxySelectPoser.transform.localRotation * (0.1f * Vector3.left);
            if (VFInput._moveForward) GameCamera.instance.transform.localPosition += GameCamera.instance.galaxySelectPoser.transform.localRotation * (0.1f * Vector3.up);
            if (VFInput._moveBackward) GameCamera.instance.transform.localPosition += GameCamera.instance.galaxySelectPoser.transform.localRotation * (0.1f * Vector3.down);
            int targetIndex = -1;
            for (var i = 0; i < starmap.starPool.Count; ++i)
            {
                // GS2.Warn($"#{i} {GSSettings.BirthPlanet.planetData.star.index}");
                if (starmap.starPool[i].active) //&& i == GSSettings.BirthPlanet.planetData.star.index
                {
                    StarData starData = starmap.starPool[i].starData;
                    Vector2 zero = Vector2.zero;
                    UIRoot.ScreenPointIntoRect(Camera.main.WorldToScreenPoint(starData.position), starmap.textGroup, out zero);
                    zero.x += 18f;
                    zero.y += 6f;
                    starmap.starPool[i].nameText.rectTransform.anchoredPosition = zero;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float num3 = Kit.ClosestPoint2Straight(ray.origin, ray.GetPoint(300f), starData.position);
                    float num4 = Vector3.Distance(ray.GetPoint(300f * num3), starData.position);
                    if (num4 < mouseTolerance)
                    {
                        // GS2.Warn($"Mouse Close to {starData.name}");
                        // if (num4 < starmap.starPool[i].pointRenderer.transform.localScale.x * 0.25f)
                        // {
                        //     mouseTolerance = 0f;
                        // }
                        // else
                        // {
                        //     mouseTolerance = num4;
                        // }
                        targetIndex = i;
                    }
                    if (i == GSSettings.BirthPlanet.planetData.star.index)
                    {
                        Color value = starmap.starColors.Evaluate(starData.color);
                        starmap.starPointBirth.gameObject.SetActive(true);
                        starmap.starPointBirth.material.SetColor("_TintColor", value);
                        starmap.starPointBirth.transform.localPosition = starData.position;
                    }

                    starmap.starPool[i].nameText.gameObject.SetActive(targetIndex == i || VFInput.alt || i == GSSettings.BirthPlanet.planetData.star.index);

                    starmap.starPool[i].nameText.rectTransform.sizeDelta = new Vector2(starmap.starPool[i].nameText.preferredWidth, starmap.starPool[i].nameText.preferredHeight);

                    starmap.starPool[i].nameText.text = starmap.starPool[i].textContent;
                    starmap.starPool[i].nameText.rectTransform.sizeDelta = new Vector2(starmap.starPool[i].nameText.preferredWidth, starmap.starPool[i].nameText.preferredHeight);

                    
                }
                // GS2.Warn($"Setting StarPointBirth to {__instance.starPool[i].starData.name}");
                // var starData = starmap.starPool[i].starData;
                // var color = starmap.starColors.Evaluate(starData.color);
                // starmap.starPointBirth.gameObject.SetActive(true);
                // starmap.starPointBirth.material.SetColor("_TintColor", color);
                // starmap.starPointBirth.transform.localPosition = starData.position;
            }
            


            if (!(VFInput.rtsConfirm.pressing || VFInput.rtsCancel.pressing))
            {
                deBounce = false;
                //GS2.Log($"Nope {VFInput.rtsConfirm.pressing}{VFInput.rtsCancel.pressing}{VFInput.rtsConfirm.onDown}{VFInput.rtsCancel.onDown}{VFInput.axis_button.down[0]}{VFInput.axis_button.down[1]}");
                return;
            }
            if (deBounce) return;
            deBounce = true;
            var starIndex = -1;
            var clickTolerance = 1.7f;
            for (var i = 0; i < starmap.starPool.Count; ++i)
                if (starmap.starPool[i].active)
                {
                    var starData = starmap.starPool[i].starData;
                    var rectPoint = Vector2.zero;
                    UIRoot.ScreenPointIntoRect(Camera.main.WorldToScreenPoint(starData.position), starmap.textGroup, out _);
                    rectPoint.x += 18f;
                    rectPoint.y += 6f;
                    starmap.starPool[i].nameText.rectTransform.anchoredPosition = rectPoint;
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var num2 = Kit.ClosestPoint2Straight(ray.origin, ray.GetPoint(300f), starData.position);
                    var distanceFromClickToStar = Vector3.Distance(ray.GetPoint(300f * num2), starData.position);
                    if (distanceFromClickToStar < (double)clickTolerance)
                    {
                        clickTolerance = distanceFromClickToStar >= starmap.starPool[i].pointRenderer.transform.localScale.x * 0.25 ? distanceFromClickToStar : 0.0f;
                        starIndex = i;
                    }

                    //GS2.Warn($"index2 = {index2} GSSettings.birthStarId:{GSSettings.birthStarId}");
                    // if (i == GSSettings.BirthPlanet.planetData.star.index)
                    // {
                    //     var color = __instance.starColors.Evaluate(starData.color);
                    //     __instance.starPointBirth.gameObject.SetActive(true);
                    //     __instance.starPointBirth.material.SetColor("_TintColor", color);
                    //     __instance.starPointBirth.transform.localPosition = starData.position;
                    // }
                }

            if (VFInput.rtsConfirm.pressing)
            {
                OnStarMapClick(starmap, starIndex);
            }
            if (VFInput.rtsCancel.pressing)
            {
                OnStarMapRightClick(starmap, starIndex);
            }

            // return;
            // if (GS2.Vanilla) return;
            // if (GS2.NebulaClient && NebulaModAPI.MultiplayerSession != null) return; // use new lobby feature in multiplayer but preserve existing functionality in single player
            // GS2.Warn("*");
            // var index1 = -1;
            // var num1 = 1.7f;
            // for (var index2 = 0; index2 < __instance.starPool.Count; ++index2)
            //     if (__instance.starPool[index2].active)
            //     {
            //         var starData = __instance.starPool[index2].starData;
            //         var rectPoint = Vector2.zero;
            //         UIRoot.ScreenPointIntoRect(Camera.main.WorldToScreenPoint(starData.position), __instance.textGroup, out rectPoint);
            //         rectPoint.x += 18f;
            //         rectPoint.y += 6f;
            //         __instance.starPool[index2].nameText.rectTransform.anchoredPosition = rectPoint;
            //         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //         var num2 = Kit.ClosestPoint2Straight(ray.origin, ray.GetPoint(300f), starData.position);
            //         var num3 = Vector3.Distance(ray.GetPoint(300f * num2), starData.position);
            //         if (num3 < (double)num1)
            //         {
            //             num1 = num3 >= __instance.starPool[index2].pointRenderer.transform.localScale.x * 0.25 ? num3 : 0.0f;
            //             index1 = index2;
            //         }
            //
            //         //GS2.Warn($"index2 = {index2} GSSettings.birthStarId:{GSSettings.birthStarId}");
            //         if (index2 == GSSettings.BirthPlanet.planetData.star.index)
            //         {
            //             var color = __instance.starColors.Evaluate(starData.color);
            //             __instance.starPointBirth.gameObject.SetActive(true);
            //             __instance.starPointBirth.material.SetColor("_TintColor", color);
            //             __instance.starPointBirth.transform.localPosition = starData.position;
            //         }
            //     }
            //
            // var pressing = VFInput.rtsConfirm.pressing;
            // var flag1 = !string.IsNullOrEmpty(__instance.clickText);
            // for (var index2 = 0; index2 < __instance.starPool.Count; ++index2)
            // {
            //     var flag2 = __instance.starPool[index2].active && index2 == index1;
            //     __instance.starPool[index2].nameText.gameObject.SetActive(flag2);
            //
            //     if (flag2)
            //     {
            //         // GS2.Log("0");
            //         if (pressing && !GS2.GetGSStar(__instance.starPool[index1].starData).Decorative)
            //         {
            //             if (GS2.ActiveGenerator.Config.enableStarSelector)
            //             {
            //                 GS2.ActiveGenerator.Generate(GSSettings.StarCount, __instance.starPool[index1].starData);
            //                 __instance.galaxyData = GS2.ProcessGalaxy(GS2.gameDesc, true);
            //                 __instance.OnGalaxyDataReset();
            //             }
            //             else
            //             {
            //                 __instance.starPool[index2].nameText.text = __instance.starPool[index2].textContent + "\r\n" + __instance.clickText.Translate();
            //             }
            //         }
            //
            //         // GS2.Log(__instance.starPool[index1].starData.name + " - " +
            //         //         __instance.starPool[index2].starData.name);
            //         var sd = __instance.starPool[index2]?.starData;
            //         // GS2.Log("1");
            //         if (__instance.starPool[index2]?.nameText?.text != null && !GS2.GetGSStar(__instance.starPool[index1].starData).Decorative) __instance.starPool[index2].nameText.text = $"{__instance.starPool[index2].textContent}\r\n{Utils.GetStarDetail(sd)}";
            //
            //         // $"{__instance.starPool[index2].textContent}\r\n{"Gas Giants".Translate()}:{Utils.GetStarDataGasCount(sd)}\r\n{"Planets".Translate()}:{Utils.GetStarDataTelluricCount(sd)}\r\n{"Moons".Translate()}:{Utils.GetStarDataMoonCount(sd)}";
            //         // GS2.Log("2");
            //         // GS2.Log($"{sd?.planetCount}");
            //         if (GS2.GetGSStar(__instance.starPool[index1].starData).Decorative) __instance.starPool[index2].nameText.rectTransform.gameObject.SetActive(false);
            //         else __instance.starPool[index2].nameText.rectTransform.sizeDelta = new Vector2(__instance.starPool[index2].nameText.preferredWidth, __instance.starPool[index2].nameText.preferredHeight);
            //     }
            //     else if (!flag2 & flag1)
            //     {
            //         __instance.starPool[index2].nameText.text = __instance.starPool[index2].textContent;
            //         __instance.starPool[index2].nameText.rectTransform.sizeDelta = new Vector2(__instance.starPool[index2].nameText.preferredWidth, __instance.starPool[index2].nameText.preferredHeight);
            //     }
            // }
            //
            // var flag3 = index1 >= 0 && __instance.starPool[index1].active;
            // __instance.starPointSelection.gameObject.SetActive(flag3);
            // __instance.starPool[GSSettings.BirthPlanet.planetData.star.index].nameText.gameObject.SetActive(true);
            // if (!flag3) return;
            //
            // var starData1 = __instance.starPool[index1].starData;
            // var color1 = __instance.starColors.Evaluate(starData1.color);
            // if (starData1.type == EStarType.NeutronStar)
            //     color1 = __instance.neutronStarColor;
            // else if (starData1.type == EStarType.WhiteDwarf)
            //     color1 = __instance.whiteDwarfColor;
            // else if (starData1.type == EStarType.BlackHole) color1 = __instance.blackholeColor;
            //
            // var num4 = 1.2f;
            // if (starData1.type == EStarType.GiantStar)
            //     num4 = 3f;
            // else if (starData1.type == EStarType.WhiteDwarf)
            //     num4 = 0.6f;
            // else if (starData1.type == EStarType.NeutronStar)
            //     num4 = 0.6f;
            // else if (starData1.type == EStarType.BlackHole) num4 = 0.8f;
            //
            // __instance.starPointSelection.material.SetColor("_TintColor", color1);
            // __instance.starPointSelection.transform.localPosition = starData1.position;
            // __instance.starPointSelection.transform.localScale = Vector3.one * (float)(num4 * 0.600000023841858 + 0.600000023841858);
        }
        
        public static void OnBackClick(UIGalaxySelect instance)
        {
            GS2.Warn("BackClick");
            if (!inSystemDisplay) instance.CancelSelect();
            else ShowStarMap(instance.starmap);
        }

        public static void OnRandomClick(UIGalaxySelect instance)
        {
            instance.Rerand();
            ShowStarMap(instance.starmap);
        }
        public static void OnStarMapClick(UIVirtualStarmap starmap, int starIndex)
        {
            GS2.Warn($"StarmapClick { starIndex}");
            switch (starIndex)
            {
                case -1 when (UIRoot.instance.uiGame.planetDetail.gameObject.activeSelf):
                    GS2.Warn("Hiding Planet Details");
                    HidePlanetDetail();
                    break;
                case -1 when (!UIRoot.instance.uiGame.planetDetail.gameObject.activeSelf):
                    GS2.Warn("Hiding SolarSystem");
                    ShowStarMap(starmap);
                    break;
                case 0 when inSystemDisplay:
                    GS2.Warn("System Star Click");
                    OnSolarSystemStarClick(starmap);
                    break;
                case int x when x >= 0 && inSystemDisplay:
                    GS2.Warn($"PlanetClick {starIndex}");
                    OnSolarSystemPlanetClick(starmap, starIndex);
                    break;
                case int x when x >= 0 && !inSystemDisplay:                   ;
                    GS2.Warn($"- {starIndex} OnStarMapClick");
                    OnStarClick(starmap, starIndex);
                    break;
                default:
                    GS2.Warn($"Clicked Starmap with Erroneous starIndex of {starIndex}");
                    break;
            }
        }
        public static void OnStarMapRightClick(UIVirtualStarmap starmap, int starIndex)
        {
            GS2.Warn($"StarmapRightClick { starIndex}");
            switch (starIndex)
            {
                case -1 when (UIRoot.instance.uiGame.planetDetail.gameObject.activeSelf):
                    GS2.Warn("Rightclick Hiding Details");
                    HidePlanetDetail();
                    ShowStarCount();
                    break;
                case -1 when (!UIRoot.instance.uiGame.planetDetail.gameObject.activeSelf):
                    GS2.Warn("RightClick Hiding SolarSystem");
                    ShowStarMap(starmap);
                    break;
                case 0 when inSystemDisplay:
                    GS2.Warn("System Star RightClick");
                    OnSolarSystemStarClick(starmap);
                    break;
                case int x when x >= 0 && inSystemDisplay:
                    GS2.Warn($"Planet RightClick {starIndex}");
                    OnSolarSystemPlanetRightClick(starmap, starIndex);
                    break;
                case int x when x >= 0 && !inSystemDisplay:
                    
                    GS2.Warn($"- {starIndex} OnStarMapRightClick");
                    // SetBirthStar(starmap.starPool[starIndex].starData);
                    OnStarRightClick(starmap,starIndex);
                    break;
                   
                default:
                    GS2.Warn($"RightClicked Starmap with Erroneous starIndex of {starIndex}");
                    break;
            }
        }
        public static void OnSolarSystemStarClick(UIVirtualStarmap starmap)
        {
            HidePlanetDetail();
            HideStarCount();
            ShowStarDetail(viewStar);
        }
        public static void HideStarCount()
        {
            GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/right-group")?.SetActive(false);
            GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/top-title")?.SetActive(false);
            UIRoot.instance.galaxySelect.seedInput.transform.parent.gameObject.SetActive(false);
            UIRoot.instance.galaxySelect.starCountSlider.transform.parent.gameObject.SetActive(false);
            UIRoot.instance.galaxySelect.resourceMultiplierSlider.transform.parent.gameObject.SetActive(false);
        }
        public static void HidePlanetDetail()
        {
            UIRoot.instance.uiGame.planetDetail.gameObject.SetActive(false);
        }
        public static void HideStarDetail()
        {
            UIRoot.instance.uiGame.starDetail.gameObject.SetActive(false);
        }
        public static void ShowPlanetDetail(PlanetData pData)
        {
            UIRoot.instance.uiGame.SetPlanetDetail(pData);
            UIRoot.instance.uiGame.planetDetail.gameObject.SetActive(true);
            UIRoot.instance.uiGame.planetDetail.gameObject.GetComponent<RectTransform>().parent.gameObject.SetActive(true);
            UIRoot.instance.uiGame.planetDetail.gameObject.GetComponent<RectTransform>().parent.gameObject.GetComponent<RectTransform>().parent.gameObject.SetActive(true);
            UIRoot.instance.uiGame.planetDetail._OnUpdate();
        }
        public static void ShowStarDetail(StarData starData)
        {
            UIRoot.instance.uiGame.SetStarDetail(starData);
            UIRoot.instance.uiGame.starDetail.gameObject.SetActive(true);
            UIRoot.instance.uiGame.starDetail.gameObject.GetComponent<RectTransform>().parent.gameObject.SetActive(true);
            UIRoot.instance.uiGame.starDetail.gameObject.GetComponent<RectTransform>().parent.gameObject.GetComponent<RectTransform>().parent.gameObject.SetActive(true);
            UIRoot.instance.uiGame.starDetail._OnUpdate();
        }
        public static void OnStarClick(UIVirtualStarmap starmap, int starIndex)
        {
            viewStar = starmap.starPool[starIndex].starData;

            // UIRoot.instance.galaxySelect.resourceMultiplierSlider.gameObject.SetActive(false);
            ClearStarmap(starmap);
            GS2.Warn($"OnStarClick {viewStar.name}");
            HideStarCount();
            ShowSolarSystem(starmap, starIndex);
        }

        public static void OnStarRightClick(UIVirtualStarmap starmap, int starIndex)
        {
            if (!GS2.GetGSStar(starmap.starPool[starIndex].starData).Decorative)
            {
                if (GS2.ActiveGenerator.Config.enableStarSelector)
                {
                    RegenerateGalaxyWithNewBirthStar(starmap, starmap.starPool[starIndex].starData);
                }
            }
        }

        public static void RegenerateGalaxyWithNewBirthStar(UIVirtualStarmap starmap, StarData starData)
        {
            GS2.ActiveGenerator.Generate(GSSettings.StarCount, starData);
            starmap.galaxyData = GS2.ProcessGalaxy(GS2.gameDesc, true);
            starmap.OnGalaxyDataReset();
        }
        public static void OnSolarSystemPlanetClick(UIVirtualStarmap starmap, int clickIndex)
        {
            int planetIndex = clickIndex -1;
            GS2.Warn($"System:{viewStar.name} PlanetCount:{viewStar.planetCount}");
            PlanetData pData = viewStar?.planets[planetIndex];
            GS2.Warn("C");
            if (pData == null) return;
            GS2.Warn($"{pData.name}");
            HideStarDetail();
            HideStarCount();
            ShowPlanetDetail(pData);
        }        
        public static void OnSolarSystemPlanetRightClick(UIVirtualStarmap starmap, int clickIndex)
        {
            int planetIndex = clickIndex -1;
            GS2.Warn($"System:{viewStar.name} PlanetCount:{viewStar.planetCount}");
            PlanetData pData = viewStar?.planets[planetIndex];
            GS2.Warn("C");
            if (pData == null) return;
            GS2.Warn($"Setting new Star as BirthStar and Planet as {pData.name}");
            if (pData.star.id != starmap.galaxyData.birthStarId || pData.id != starmap.galaxyData.birthPlanetId)
            {
                GS2.Warn($"---- Original birthPlanetID:{starmap.galaxyData.birthPlanetId} {starmap.galaxyData.PlanetById(starmap.galaxyData.birthPlanetId)}");
                starmap.galaxyData.birthStarId = viewStar.id;
                starmap.galaxyData.birthPlanetId = pData.id;
                GSSettings.BirthPlanetName = pData.name;
                GS2.Warn($" GSSettings.BirthPlanetId:{ GSSettings.BirthPlanetId} {GSSettings.BirthPlanet.Name} should be {pData.id} {pData.name}");
                GSSettings.BirthPlanetId = pData.id;
                GS2.Warn($" GSSettings.BirthPlanetId:{ GSSettings.BirthPlanetId} {GSSettings.BirthPlanet.Name} should be {pData.id} {pData.name}");
                // GSSettings.BirthPlanet = GS2.GetGSPlanet(pData);
                GSSettings.Instance.imported = true;
                GS2.galaxy.birthStarId = viewStar.id;
                GS2.galaxy.birthPlanetId = pData.id;
                GameMain.galaxy.birthPlanetId = pData.id;
                GameMain.galaxy.birthStarId = viewStar.id;
                GS2.Warn($"GameMain.galaxy.birthPlanetID:{GameMain.galaxy.birthPlanetId} should be {pData.id}");
                GameMain.data.galaxy.birthPlanetId = pData.id;
                GameMain.data.galaxy.birthStarId = viewStar.id;
                GS2.Warn($" GSSettings.BirthPlanetId:{ GSSettings.BirthPlanetId} should be {pData.id} {GSSettings.BirthPlanet.Name} should be {pData.name}");
            }
            HideStarDetail();
            HideStarCount();
            ShowPlanetDetail(pData);
        }
        public static void ShowStarCount()
        {
            GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/right-group").SetActive(true);
            GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/top-title").SetActive(true);
            UIRoot.instance.galaxySelect.seedInput.transform.parent.gameObject.SetActive(true);
            UIRoot.instance.galaxySelect.starCountSlider.transform.parent.gameObject.SetActive(true);
            UIRoot.instance.galaxySelect.resourceMultiplierSlider.transform.parent.gameObject.SetActive(true);
            // UIRoot.instance.galaxySelect.starCountText .gameObject.SetActive(true);
            // UIRoot.instance.galaxySelect.resourceMultiplierText.gameObject.SetActive(true);
            
        }
        public static void ShowStarMap(UIVirtualStarmap starmap)
        {
            GS2.Warn("Reverting to galaxy view");
            inSystemDisplay = false;
            viewStar = null;
            starmap.clickText = "";
            HidePlanetDetail();
            HideStarDetail();
            // ClearStarmap(starmap);
            ShowStarCount();
            starmap.OnGalaxyDataReset();
        }
        public static void ShowSolarSystem(UIVirtualStarmap starmap, int starIndex)
        {
            ClearStarmap(starmap);
            inSystemDisplay = true;
            GS2.Warn("ShowSolarSystem");
            // start planet compute thread if not done already
            // Modeler.aborted = false;
            PlanetModelingManager.StartPlanetComputeThread();

            // add star
            StarData starData = starmap._galaxyData.StarById(starIndex + 1); // because StarById() decrements by 1
            AddStarToStarmap(starmap, starData);
            
            var starScale = starmap.starPool[0].starData.radius /40f * GS2.Config.VirtualStarmapStarScaleFactor;
            GS2.Warn($"Scale : {starScale} Radius:{starmap.starPool[0].starData.radius} OrigScale:{starmap.starPool[0].pointRenderer.transform.localScale}");
            starmap.starPool[0].pointRenderer.transform.localScale = new Vector3(starScale,starScale,starScale);
            //starmap.clickText = starData.id.ToString();
            //Debug.Log("Setting it to " + starmap.clickText + " " + starData.id);

            for (int i = 0; i < starData.planetCount; i++)
            {
                // add planets
                PlanetData pData = starData.planets[i];
                Color color = starmap.neutronStarColor;
                
                bool isMoon = false;

                VectorLF3 pPos = GetRelativeRotatedPlanetPos(starData, pData, ref isMoon);
                // GS2.Warn("ShowSolarSystem2");
                // request generation of planet surface data to display its details when clicked and if not already loaded
                //if (!pData.loaded) PlanetModelingManager.RequestLoadPlanet(pData);
                // GS2.Warn("ShowSolarSystem3");
                // create fake StarData to pass _OnLateUpdate()
                StarData dummyStarData = new StarData();
                dummyStarData.position = pPos;
                var gsPlanet = GS2.GetGSPlanet(pData);
                var gsTheme = gsPlanet.GsTheme;
                var orbitColor = PlanetTemperatureToStarColor(gsTheme.Temperature);
                var planetColor = (gsTheme.minimapMaterial.Colors.ContainsKey("_Color"))? gsTheme.minimapMaterial.Colors["_Color"]:Color.magenta;
                // GS2.Log($"Color of {starData.name} a {starData.typeString} star is {starData.color}");
                dummyStarData.id = pData.id;

                Vector3 scale = Vector3.one * GS2.Config.VirtualStarmapScaleFactor * (pData.realRadius / 1000);
                // if (scale.x > 3 || scale.y > 3 || scale.z > 3)
                // {
                //     scale = new Vector3(3, 3, 3);
                // }

                // GS2.Warn($"ShowSolarSystem4 {i}");
                // GS2.Warn($"Planet: {starData.planets[i].name}");
                // GS2.Warn($"Pool Length: {i + 1} / {starmap.starPool.Count}");
                while (starmap.starPool.Count <= i + 1)
                {
                    UIVirtualStarmap.StarNode starNode2 = new UIVirtualStarmap.StarNode();
                    starNode2.active = false;
                    starNode2.starData = null;
                    starNode2.pointRenderer = Object.Instantiate<MeshRenderer>(starmap.starPointPrefab, starmap.starPointPrefab.transform.parent);
                    starNode2.nameText = Object.Instantiate<Text>(starmap.nameTextPrefab, starmap.nameTextPrefab.transform.parent);
                    starmap.starPool.Add(starNode2);
                }

                starmap.starPool[i + 1].active = true;

                // GS2.Warn("ShowSolarSystem4a");

                starmap.starPool[i + 1].starData = dummyStarData;
                starmap.starPool[i + 1].pointRenderer.material.SetColor("_TintColor", planetColor);
                starmap.starPool[i + 1].pointRenderer.transform.localPosition = pPos;
                starmap.starPool[i + 1].pointRenderer.transform.localScale = scale;
                starmap.starPool[i + 1].pointRenderer.gameObject.SetActive(true);
                starmap.starPool[i + 1].nameText.text = pData.displayName + " (" + pData.typeString + ")";
                starmap.starPool[i + 1].nameText.color = Color.Lerp(planetColor, Color.white, 0.5f);
                starmap.starPool[i + 1].nameText.rectTransform.sizeDelta = new Vector2(starmap.starPool[i + 1].nameText.preferredWidth, starmap.starPool[i + 1].nameText.preferredHeight);
                starmap.starPool[i + 1].nameText.rectTransform.anchoredPosition = new Vector2(-2000f, -2000f);
                starmap.starPool[i + 1].textContent = pData.displayName + " (" + pData.typeString + ")";

                starmap.starPool[i + 1].nameText.gameObject.SetActive(true);
                // GS2.Warn($"ShowSolarSystem5 {i} / {starmap.connPool.Count}");
                // add orbit renderer
                while (starmap.connPool.Count <= i)
                {
                    starmap.connPool.Add(new UIVirtualStarmap.ConnNode
                    {
                        active = false,
                        starA = null,
                        starB = null,
                        lineRenderer = Object.Instantiate(starmap.connLinePrefab, starmap.connLinePrefab.transform.parent)
                    });
                    //starmap.connPool[starmap.connPool.Count-1].lineRenderer.material.SetColor("_LineColorA", Color.Lerp(color, Color.white, 0.65f));
                    //starmap.connPool[starmap.connPool.Count - 1].lineRenderer.material.SetColor("_LineColorB", Color.Lerp(color, Color.white, 0.65f));
                }

                //if (starmap.connPool.Count -1 >= i) {
                starmap.connPool[i].active = true;
                starmap.connPool[i].lineRenderer.material.SetColor("_LineColorA", Color.Lerp(starmap.starColors.Evaluate(orbitColor), Color.white, 0.65f));
                starmap.connPool[i].lineRenderer.material.SetColor("_LineColorB", Color.Lerp(starmap.starColors.Evaluate(orbitColor), Color.white, 0.25f));
                if (starmap.connPool[i].lineRenderer.positionCount != 61)
                {
                    starmap.connPool[i].lineRenderer.positionCount = 61;
                }

                //}
                // GS2.Warn("ShowSolarSystem6");
                for (int j = 0; j < 61; j++)
                {
                    // GS2.Warn("ShowSolarSystem7");
                    float f = (float)j * 0.017453292f * 6f; // ty dsp devs :D
                    Vector3 cPos = GetCenterOfOrbit(starData, pData, ref isMoon);
                    Vector3 position;
                    if (isMoon)
                    {
                        position = new Vector3(Mathf.Cos(f) * pData.orbitRadius * GS2.Config.VirtualStarmapOrbitScaleFactor * 8 + (float)cPos.x, cPos.y, Mathf.Sin(f) * pData.orbitRadius * GS2.Config.VirtualStarmapOrbitScaleFactor * 8 + (float)cPos.z);
                    }
                    else
                    {
                        position = new Vector3(Mathf.Cos(f) * pData.orbitRadius * GS2.Config.VirtualStarmapOrbitScaleFactor + (float)cPos.x, cPos.y, Mathf.Sin(f) * pData.orbitRadius * GS2.Config.VirtualStarmapOrbitScaleFactor + (float)cPos.z);
                    }

                    // GS2.Warn("ShowSolarSystem7a");

                    // rotate position around center by orbit angle
                    Quaternion quaternion = Quaternion.Euler(pData.orbitInclination, pData.orbitInclination, pData.orbitInclination);
                    Vector3 dir = quaternion * (position - cPos);
                    position = dir + cPos;

                    starmap.connPool[i].lineRenderer.SetPosition(j, position);
                }

                // GS2.Warn("ShowSolarSystem7b");

                starmap.connPool[i].lineRenderer.gameObject.SetActive(true);
            }
        }
        private static VectorLF3 GetCenterOfOrbit(StarData starData, PlanetData pData, ref bool isMoon)
        {
            // GS2.Warn("GetCenterOfOrbit");
            if (pData.orbitAroundPlanet != null)
            {
                return GetRelativeRotatedPlanetPos(starData, pData.orbitAroundPlanet, ref isMoon);
            }

            isMoon = false;
            return starData.position;
        }

        private static VectorLF3 GetRelativeRotatedPlanetPos(StarData starData, PlanetData pData, ref bool isMoon)
        {
            // GS2.Warn("GetRelativeRotatedPlanetPos");
            VectorLF3 pos;
            VectorLF3 dir;
            Quaternion quaternion;
            if (pData.orbitAroundPlanet != null)
            {
                VectorLF3 centerPos = GetRelativeRotatedPlanetPos(starData, pData.orbitAroundPlanet, ref isMoon);
                isMoon = true;
                pos = new VectorLF3(Mathf.Cos(pData.orbitPhase) * pData.orbitRadius * GS2.Config.VirtualStarmapOrbitScaleFactor * 8 + centerPos.x, centerPos.y, Mathf.Sin(pData.orbitPhase) * pData.orbitRadius * GS2.Config.VirtualStarmapOrbitScaleFactor * 8 + centerPos.z);
                quaternion = Quaternion.Euler(pData.orbitInclination, pData.orbitInclination, pData.orbitInclination);
                dir = quaternion * (pos - centerPos);
                return dir + centerPos;
            }

            pos = new VectorLF3(Mathf.Cos(pData.orbitPhase) * pData.orbitRadius * GS2.Config.VirtualStarmapOrbitScaleFactor + starData.position.x, starData.position.y, Mathf.Sin(pData.orbitPhase) * pData.orbitRadius * GS2.Config.VirtualStarmapOrbitScaleFactor + starData.position.z);
            quaternion = Quaternion.Euler(pData.orbitInclination, pData.orbitInclination, pData.orbitInclination);
            dir = quaternion * (pos - starData.position);
            return dir + starData.position;
        }

        // probably reverse patch this if there is time
        private static void AddStarToStarmap(UIVirtualStarmap starmap, StarData starData)
        {
            // GS2.Warn("AddStarToStarMap");
            Color color = starmap.starColors.Evaluate(starData.color);
            if (starData.type == EStarType.NeutronStar)
            {
                color = starmap.neutronStarColor;
            }
            else if (starData.type == EStarType.WhiteDwarf)
            {
                color = starmap.whiteDwarfColor;
            }
            else if (starData.type == EStarType.BlackHole)
            {
                color = starmap.blackholeColor;
            }

            float num2 = 1.2f;
            if (starData.type == EStarType.GiantStar)
            {
                num2 = 3f;
            }
            else if (starData.type == EStarType.WhiteDwarf)
            {
                num2 = 0.6f;
            }
            else if (starData.type == EStarType.NeutronStar)
            {
                num2 = 0.6f;
            }
            else if (starData.type == EStarType.BlackHole)
            {
                num2 = 0.8f;
            }

            string text = starData.displayName + "  ";
            if (starData.type == EStarType.GiantStar)
            {
                if (starData.spectr <= ESpectrType.K)
                {
                    text += "红巨星".Translate();
                }
                else if (starData.spectr <= ESpectrType.F)
                {
                    text += "黄巨星".Translate();
                }
                else if (starData.spectr == ESpectrType.A)
                {
                    text += "白巨星".Translate();
                }
                else
                {
                    text += "蓝巨星".Translate();
                }
            }
            else if (starData.type == EStarType.WhiteDwarf)
            {
                text += "白矮星".Translate();
            }
            else if (starData.type == EStarType.NeutronStar)
            {
                text += "中子星".Translate();
            }
            else if (starData.type == EStarType.BlackHole)
            {
                text += "黑洞".Translate();
            }
            else if (starData.type == EStarType.MainSeqStar)
            {
                text = text + starData.spectr.ToString() + "型恒星".Translate();
            }

            if (starData.index == ((customBirthStar != -1) ? customBirthStar - 1 : starmap._galaxyData.birthStarId - 1))
            {
                text = "即将登陆".Translate() + "\r\n" + text;
            }

            starmap.starPool[0].active = true;
            starmap.starPool[0].starData = starData;
            starmap.starPool[0].pointRenderer.material.SetColor("_TintColor", color);
            starmap.starPool[0].pointRenderer.transform.localPosition = starData.position;
            starmap.starPool[0].pointRenderer.transform.localScale = Vector3.one * num2 * 2;
            starmap.starPool[0].pointRenderer.gameObject.SetActive(true);
            starmap.starPool[0].nameText.text = text;
            starmap.starPool[0].nameText.color = Color.Lerp(color, Color.white, 0.5f);
            starmap.starPool[0].nameText.rectTransform.sizeDelta = new Vector2(starmap.starPool[0].nameText.preferredWidth, starmap.starPool[0].nameText.preferredHeight);
            starmap.starPool[0].nameText.rectTransform.anchoredPosition = new Vector2(-2000f, -2000f);
            starmap.starPool[0].textContent = text;

            starmap.starPool[0].nameText.gameObject.SetActive(true);
        }
        public static void ClearStarmap(UIVirtualStarmap starmap)
        {
            GS2.Warn("ClearStarmap");

            
            
            foreach (UIVirtualStarmap.StarNode starNode in starmap.starPool)
            {
                starNode.active = false;
                starNode.starData = null;
                starNode.pointRenderer.gameObject.SetActive(false);
                starNode.nameText.gameObject.SetActive(false);
            }
            
            foreach (UIVirtualStarmap.ConnNode connNode in starmap.connPool)
            {
                connNode.active = false;
                connNode.starA = null;
                connNode.starB = null;
                connNode.lineRenderer.gameObject.SetActive(false);
            }
            // starmap = new UIVirtualStarmap();
            // starmap.starPool = new List<UIVirtualStarmap.StarNode>();
            // starmap.connPool = new List<UIVirtualStarmap.ConnNode>();
            
        }

        private static float PlanetTemperatureToStarColor(float temperature)
        {
            if (temperature > 3) return 0;
            if (temperature > 1) return 0.3f;
            if (temperature > -1) return 0.5f;
            if (temperature > -3.1f) return 0.8f;
            return 0.9f;
        }
    }
}