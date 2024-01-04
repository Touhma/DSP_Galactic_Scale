using System.Linq;
using NGPT;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public static class SystemDisplay
    {
        private const float mouseTolerance = 1.7f;
        public static bool inSystemDisplay;
        public static StarData viewStar;
        public static Button randomButton;
        public static Button startButton;
        public static Button backButton;
        private static bool deBounce;
        private static readonly int CustomBirthStar = -1;
        public static bool inGalaxySelect;
        private static GameObject galaxySelectRightGroup;
        private static GameObject galaxySelectTopTitle;
        private static readonly int TintColor = Shader.PropertyToID("_TintColor");
        private static readonly int LineColorA = Shader.PropertyToID("_LineColorA");
        private static readonly int LineColorB = Shader.PropertyToID("_LineColorB");
        private static UIGalaxySelect GalaxySelect => UIRoot.instance.galaxySelect;
        private static UIGame uiGame => UIRoot.instance.uiGame;
        private static UIPlanetDetail uiPlanetDetail => uiGame.planetDetail;
        private static UIStarDetail uiStarDetail => uiGame.starDetail;


        public static void AbortRender(UIVirtualStarmap starmap)
        {
            // Modeler.Reset();
            ShowStarMap(starmap);
        }

        public static void InitializeButtons(UIGalaxySelect instance)
        {
            Debug.Log("1");
            galaxySelectTopTitle = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/top-title/");
            Debug.Log("2");
            galaxySelectRightGroup = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/right-group/");
            Debug.Log("3");
            // backButton.onClick.RemoveAllListeners();
            randomButton.onClick.RemoveAllListeners();
            randomButton.onClick.m_PersistentCalls.Clear();
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.m_PersistentCalls.Clear();
            // backButton.onClick.AddListener(() => OnBackClick(instance));
            startButton.onClick.AddListener(() => OnStartClick(instance));
            randomButton.onClick.AddListener(() => OnRandomClick(instance));
        }

        private static void OnStartClick(UIGalaxySelect instance)
        {
            Debug.Log("Start Clicked");
            Modeler.Reset();
            inGalaxySelect = false;
            Debug.Log($"Waiting for modeler to reset...{Modeler.processing.Count}");
            if (instance.uiCombat.active)//0.10...
            {
                instance.uiCombat.ApplySettings();
                return;
            }
            if (!instance.gameDesc.isCombatMode)
            {
                instance.gameDesc.combatSettings.SetDefault();
            }//...0.10
            Bootstrap.WaitUntil(() => Modeler.Idle, () => Patches.PatchOnUIGalaxySelect.EnterGame(ref instance.gameDesc, ref instance));
        }

        public static void ResetView()
        {
            GameCamera.instance.transform.localPosition = Vector3.zero;
            UIRoot.instance.galaxySelect.cameraPoser.distRatio = 1f;
        }

        public static void OnUpdate(UIVirtualStarmap starmap)
        {
            inGalaxySelect = true;
            if (inSystemDisplay)
            {
                if (uiPlanetDetail.gameObject.activeSelf) uiPlanetDetail._OnUpdate();
                if (uiStarDetail.gameObject.activeSelf) uiStarDetail._OnUpdate();
            }

            if (Input.mouseScrollDelta.y < 0) GalaxySelect.cameraPoser.distRatio += VFInput.shift ? 1f : .1f;
            if (Input.mouseScrollDelta.y > 0) GalaxySelect.cameraPoser.distRatio -= VFInput.shift ? 1f : 0.1f;
            if (VFInput._moveRight) GameCamera.instance.transform.localPosition += GameCamera.instance.galaxySelectPoser.transform.localRotation * ((VFInput.shift ? 1f : 0.1f) * Vector3.right);
            if (VFInput._moveLeft) GameCamera.instance.transform.localPosition += GameCamera.instance.galaxySelectPoser.transform.localRotation * ((VFInput.shift ? 1f : 0.1f) * Vector3.left);
            if (VFInput._moveForward) GameCamera.instance.transform.localPosition += GameCamera.instance.galaxySelectPoser.transform.localRotation * ((VFInput.shift ? 1f : 0.1f) * Vector3.up);
            if (VFInput._moveBackward) GameCamera.instance.transform.localPosition += GameCamera.instance.galaxySelectPoser.transform.localRotation * ((VFInput.shift ? 1f : 0.1f) * Vector3.down);
            if (VFInput._jump) ResetView();

            var targetIndex = -1;
            starmap.starPointBirth.gameObject.SetActive(false);

            for (var i = 0; i < starmap.starPool.Count; ++i)
                // GS3.Warn($"#{i} {GSSettings.BirthPlanet.planetData.star.index}");
                if (starmap.starPool[i].active) //&& i == GSSettings.BirthPlanet.planetData.star.index
                {
                    var starData = starmap.starPool[i].starData;
                    var gsStar = GS3.GetGSStar(starData);
                    var decorative = false;
                    if (gsStar != null) decorative = gsStar.Decorative;
                    UIRoot.ScreenPointIntoRect(Camera.main.WorldToScreenPoint(starData.position), starmap.textGroup, out var zero);
                    zero.x += 18f;
                    zero.y += 6f;
                    starmap.starPool[i].nameText.rectTransform.anchoredPosition = zero;
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var num3 = Kit.ClosestPoint2Straight(ray.origin, ray.GetPoint(300f), starData.position);
                    var num4 = Vector3.Distance(ray.GetPoint(300f * num3), starData.position);
                    if (num4 < mouseTolerance)
                    {
                        if (starData.age >= 0 && decorative) continue;
                        // GS3.Warn($"Mouse Close to {starData.name}");
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
                        var value = starmap.starColors.Evaluate(starData.color);
                        starmap.starPointBirth.gameObject.SetActive(!inSystemDisplay);
                        starmap.starPointBirth.material.SetColor(TintColor, value);
                        starmap.starPointBirth.transform.localPosition = starData.position;
                    }

                    if (starData.id == GSSettings.BirthPlanet?.planetData?.id && viewStar?.id == GSSettings.BirthPlanet?.planetData?.star?.id)
                    {
                        var value = starmap.starColors.Evaluate(starData.color);
                        starmap.starPointBirth.gameObject.SetActive(inSystemDisplay);
                        starmap.starPointBirth.material.SetColor(TintColor, value);
                        starmap.starPointBirth.transform.localPosition = starData.position;
                    }
                    starmap.starPool[i].nameText.gameObject.SetActive(targetIndex == i || VFInput.alt || (i == GSSettings.BirthPlanet.planetData.star.index && !inSystemDisplay));

                    starmap.starPool[i].nameText.rectTransform.sizeDelta = new Vector2(starmap.starPool[i].nameText.preferredWidth, starmap.starPool[i].nameText.preferredHeight);

                    starmap.starPool[i].nameText.text = starmap.starPool[i].textContent;
                    starmap.starPool[i].nameText.rectTransform.sizeDelta = new Vector2(starmap.starPool[i].nameText.preferredWidth, starmap.starPool[i].nameText.preferredHeight);
                }
            if (!(VFInput.rtsConfirm.pressing || VFInput.rtsCancel.pressing))
            {
                deBounce = false;
                return;
            }

            if (deBounce) return;
            deBounce = true;
            var starIndex = -1;
            var clickTolerance = GS3.Config.VirtualStarmapClickTolerance;
            for (var i = 0; i < starmap.starPool.Count; ++i)
                if (starmap.starPool[i].active)
                {
                    var starData = starmap.starPool[i].starData;
                    var gsStar = GS3.GetGSStar(starData);
                    var decorative = false;
                    if (gsStar != null) decorative = gsStar.Decorative;
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var num2 = Kit.ClosestPoint2Straight(ray.origin, ray.GetPoint(300f), starData.position);
                    var distanceFromClickToStar = Vector3.Distance(ray.GetPoint(300f * num2), starData.position);
                    if (distanceFromClickToStar < (double)clickTolerance)
                    {
                        if (starData.age >= 0 && decorative) continue;
                        clickTolerance = distanceFromClickToStar >= starmap.starPool[i].pointRenderer.transform.localScale.x * 0.25 ? distanceFromClickToStar : 0.0f;
                        starIndex = i;
                    }
                }

            if (VFInput.rtsConfirm.pressing) OnStarMapClick(starmap, starIndex);
            if (VFInput.rtsCancel.pressing) OnStarMapRightClick(starmap, starIndex);
        }

        public static void OnBackClick(UIGalaxySelect instance)
        {
            if (!inSystemDisplay)
            {
                ResetView();
                inGalaxySelect = false;
                instance._Close();
                UIRoot.instance.OpenMainMenuUI();
            }
            else
            {
                ShowStarMap(instance.starmap);
            }
        }

        private static void OnRandomClick(UIGalaxySelect instance)
        {
            Modeler.Reset();
            Bootstrap.WaitUntil(() => Modeler.Idle, () =>
            {
                instance.Rerand();
                ShowStarMap(instance.starmap);
            });
        }


        private static void OnStarMapClick(UIVirtualStarmap starmap, int starIndex)
        {
            switch (starIndex)
            {
                case -1 when uiPlanetDetail.gameObject.activeSelf:
                    // GS3.Warn("Hiding Planet Details");
                    HidePlanetDetail();
                    break;
                case -1 when !uiPlanetDetail.gameObject.activeSelf:
                    // GS3.Warn("Hiding SolarSystem");
                    ShowStarMap(starmap);
                    break;
                case 0 when inSystemDisplay:
                    // GS3.Warn("System Star Click");
                    OnSolarSystemStarClick();
                    break;
                case >= 0 when inSystemDisplay:
                    // GS3.Warn($"PlanetClick {starIndex}");
                    OnSolarSystemPlanetClick(starIndex);
                    break;
                case >= 0 when !inSystemDisplay:

                    // GS3.Warn($"- {starIndex} OnStarMapClick");
                    OnStarClick(starmap, starIndex);
                    break;
                default:
                    GS3.Warn($"Clicked Starmap with Erroneous starIndex of {starIndex}");
                    break;
            }
        }

        private static void OnStarMapRightClick(UIVirtualStarmap starmap, int starIndex)
        {
            // GS3.Warn($"StarmapRightClick { starIndex}");
            switch (starIndex)
            {
                case -1 when uiPlanetDetail.gameObject.activeSelf:
                    // GS3.Warn("Rightclick Hiding Details");
                    HidePlanetDetail();
                    ShowStarCount();
                    break;
                case -1 when !uiPlanetDetail.gameObject.activeSelf:
                    // GS3.Warn("RightClick Hiding SolarSystem");
                    ShowStarMap(starmap);
                    break;
                case 0 when inSystemDisplay:
                    // GS3.Warn("System Star RightClick");
                    OnSolarSystemStarClick();
                    break;
                case >= 0 when inSystemDisplay:
                    // GS3.Warn($"Planet RightClick {starIndex}");
                    OnSolarSystemPlanetRightClick(starmap, starIndex);
                    break;
                case >= 0 when !inSystemDisplay:

                    // GS3.Warn($"- {starIndex} OnStarMapRightClick");
                    // SetBirthStar(starmap.starPool[starIndex].starData);
                    OnStarRightClick(starmap, starIndex);
                    break;

                default:
                    GS3.Warn($"RightClicked Starmap with Erroneous starIndex of {starIndex}");
                    break;
            }
        }

        private static void OnSolarSystemStarClick()
        {
            HidePlanetDetail();
            HideStarCount();
            if (!viewStar.calculated)
            {
                Debug.Log("Star not Calculated. Planet Status:");
                foreach (var p in viewStar.planets) Debug.Log($"{p.name}: {p.calculated}");
                viewStar.RunCalculateThread();
            }

            ShowStarDetail(viewStar);
        }

        private static void HideStarCount()
        {
            if (galaxySelectRightGroup != null) galaxySelectRightGroup.SetActive(false);
            if (galaxySelectTopTitle != null) galaxySelectTopTitle.SetActive(false);
            // GS3.Warn($"{UIRoot.instance.galaxySelect.sandboxToggle.transform.parent.galaxySelectRightGroup.name}");
            // GS3.Warn($"{UIRoot.instance.galaxySelect.propertyMultiplierText.transform.parent.galaxySelectRightGroup.name}");
            // GS3.Warn($"{UIRoot.instance.galaxySelect.addrText.transform.parent.galaxySelectRightGroup.name}");
            GalaxySelect.sandboxToggle.transform.parent.gameObject.SetActive(false);
            GalaxySelect.propertyMultiplierText.gameObject.SetActive(false);
            GalaxySelect.addrText.transform.parent.gameObject.SetActive(false);
            GalaxySelect.seedInput.transform.parent.gameObject.SetActive(false);
            GalaxySelect.starCountSlider.transform.parent.gameObject.SetActive(false);
            GalaxySelect.resourceMultiplierSlider.transform.parent.gameObject.SetActive(false);
            GalaxySelect.darkFogToggle.transform.parent.gameObject.SetActive(false);
        }

        private static void HidePlanetDetail()
        {
            uiPlanetDetail.gameObject.SetActive(false);
        }

        private static void HideStarDetail()
        {
            uiStarDetail.gameObject.SetActive(false);
        }

        private static void ShowPlanetDetail(PlanetData pData)
        {
            uiGame.SetPlanetDetail(pData);
            uiPlanetDetail.gameObject.SetActive(true);
            uiPlanetDetail.gameObject.GetComponent<RectTransform>().parent.gameObject.SetActive(true);
            uiPlanetDetail.gameObject.GetComponent<RectTransform>().parent.gameObject.GetComponent<RectTransform>().parent.gameObject.SetActive(true);
            uiPlanetDetail._OnUpdate();
        }

        private static void ShowStarDetail(StarData starData)
        {
            uiGame.SetStarDetail(starData);
            uiStarDetail.gameObject.SetActive(true);
            uiStarDetail.gameObject.GetComponent<RectTransform>().parent.gameObject.SetActive(true);
            uiStarDetail.gameObject.GetComponent<RectTransform>().parent.gameObject.GetComponent<RectTransform>().parent.gameObject.SetActive(true);
            uiStarDetail._OnUpdate();
            uiStarDetail.RefreshDynamicProperties();
        }

        private static void OnStarClick(UIVirtualStarmap starmap, int starIndex)
        {
            viewStar = starmap.starPool[starIndex].starData;
            var star = GS3.GetGSStar(viewStar);
            // GS3.Warn("Clicked Star");
            if (star.Decorative)
            {
                var primaryStar = GS3.GetBinaryStarHost(star);
                if (primaryStar != null)
                {
                    var primaryIndex = primaryStar.assignedIndex;
                    viewStar = starmap.starPool[primaryIndex].starData;
                    starIndex = primaryIndex;
                }
            }

            // UIRoot.instance.galaxySelect.resourceMultiplierSlider.galaxySelectRightGroup.SetActive(false);
            ClearStarmap(starmap);
            // GS3.Warn($"OnStarClick {viewStar.name}");
            HideStarCount();
            ShowSolarSystem(starmap, starIndex);
        }

        private static void OnStarRightClick(UIVirtualStarmap starmap, int starIndex)
        {
            if (!GS3.GetGSStar(starmap.starPool[starIndex].starData).Decorative)
                if (GS3.ActiveGenerator.Config.enableStarSelector)
                    RegenerateGalaxyWithNewBirthStar(starmap, starmap.starPool[starIndex].starData);
        }

        private static void RegenerateGalaxyWithNewBirthStar(UIVirtualStarmap starmap, StarData starData)
        {
            GS3.Log($"Regenerating galaxy with new birthstar:{starData.name} id:{starData.id} index:{starData.index}");
            GS3.ActiveGenerator.Generate(GSSettings.StarCount, starData);
            starmap.galaxyData = GS3.ProcessGalaxy(GS3.gameDesc, true);
            starmap.OnGalaxyDataReset();
        }

        private static void OnSolarSystemPlanetClick(int clickIndex)
        {
            var planetIndex = clickIndex - 1;
            GS3.Log($"System:{viewStar.name} PlanetCount:{viewStar.planetCount}");
            var pData = viewStar?.planets[planetIndex];
            // GS3.Warn("C");
            if (pData == null) return;
            GS3.Log($"{pData.name}");
            HideStarDetail();
            HideStarCount();
            if (!pData.calculated) pData.RunCalculateThread();
            ShowPlanetDetail(pData);
            GS3.Log($"calculated:{pData.calculated} calculating:{pData.calculating}");
        }

        private static void OnSolarSystemPlanetRightClick(UIVirtualStarmap starmap, int clickIndex)
        {
            var planetIndex = clickIndex - 1;
            // GS3.Warn($"System:{viewStar.name} PlanetCount:{viewStar.planetCount}");
            var pData = viewStar?.planets[planetIndex];
            // GS3.Warn("C");
            if (pData == null) return;
            GS3.Log($"Setting new Star as BirthStar and Planet as {pData.name}");
            if (pData.star.id != starmap.galaxyData.birthStarId || pData.id != starmap.galaxyData.birthPlanetId)
            {
                GS3.Log($"---- Original birthPlanetID:{starmap.galaxyData.birthPlanetId} {starmap.galaxyData.PlanetById(starmap.galaxyData.birthPlanetId)}");
                starmap.galaxyData.birthStarId = viewStar.id;
                starmap.galaxyData.birthPlanetId = pData.id;
                GSSettings.BirthPlanetName = pData.name;
                // GS3.Log($" GSSettings.BirthPlanetId:{GSSettings.BirthPlanetId} {GSSettings.BirthPlanet.Name} should be {pData.id} {pData.name}");
                GSSettings.BirthPlanetId = pData.id;
                // GS3.Warn($" GSSettings.BirthPlanetId:{GSSettings.BirthPlanetId} {GSSettings.BirthPlanet.Name} should be {pData.id} {pData.name}");
                // GSSettings.BirthPlanet = GS3.GetGSPlanet(pData);
                GSSettings.Instance.imported = true;
                GS3.galaxy.birthStarId = viewStar.id;
                GS3.galaxy.birthPlanetId = pData.id;
                GameMain.galaxy.birthPlanetId = pData.id;
                GameMain.galaxy.birthStarId = viewStar.id;
                // GS3.Warn($"GameMain.galaxy.birthPlanetID:{GameMain.galaxy.birthPlanetId} should be {pData.id}");
                GameMain.data.galaxy.birthPlanetId = pData.id;
                GameMain.data.galaxy.birthStarId = viewStar.id;
                // GS3.Warn($" GSSettings.BirthPlanetId:{GSSettings.BirthPlanetId} should be {pData.id} {GSSettings.BirthPlanet.Name} should be {pData.name}");
                var text = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/start-button/start-text").GetComponent<Text>();
                text.text = $"Start Game at {pData.displayName}";
                text.horizontalOverflow = HorizontalWrapMode.Overflow;
            }

            HideStarDetail();
            HideStarCount();
            // pData.CalculateVeinGroups();
            // pData.NotifyCalculated();
            pData.RunCalculateThread();
            ShowPlanetDetail(pData);
        }

        public static void ShowStarCount()
        {
            galaxySelectRightGroup.SetActive(true);
            galaxySelectTopTitle.SetActive(true);
            UIRoot.instance.galaxySelect.propertyMultiplierText.gameObject.SetActive(true);
            UIRoot.instance.galaxySelect.addrText.transform.parent.gameObject.SetActive(true);
            UIRoot.instance.galaxySelect.seedInput.transform.parent.gameObject.SetActive(true);
            UIRoot.instance.galaxySelect.sandboxToggle.transform.parent.gameObject.SetActive(true);
            UIRoot.instance.galaxySelect.starCountSlider.transform.parent.gameObject.SetActive(true);
            UIRoot.instance.galaxySelect.resourceMultiplierSlider.transform.parent.gameObject.SetActive(true);
            // UIRoot.instance.galaxySelect.starCountText .galaxySelectRightGroup.SetActive(true);
            // UIRoot.instance.galaxySelect.resourceMultiplierText.galaxySelectRightGroup.SetActive(true);
            GalaxySelect.darkFogToggle.transform.parent.gameObject.SetActive(true);
        }

        private static void ShowStarMap(UIVirtualStarmap starmap)
        {
            // GS3.Warn("Reverting to galaxy view");
            inSystemDisplay = false;
            viewStar = null;
            starmap.clickText = "";
            HidePlanetDetail();
            HideStarDetail();
            // ClearStarmap(starmap);
            ShowStarCount();
            starmap.OnGalaxyDataReset();
            OnGalaxyDataResetPostFix(starmap);
        }

        private static string StripStarFromDisplayType(string input)
        {
            var exploded = input.Split(' ').ToList();
            exploded.Remove("Star");
            return string.Join(" ", exploded.ToArray());
        }

        private static void OnGalaxyDataResetPostFix(UIVirtualStarmap __instance)
        {
            foreach (var s in __instance.starPool)
            {
                var starData = s.starData;
                if (starData == null) continue;
                var gsStar = GS3.GetGSStar(starData);
                if (gsStar == null) continue;

                if (!string.IsNullOrEmpty(gsStar.BinaryCompanion))
                {
                    var bc = GS3.GetGSStar(gsStar.BinaryCompanion);
                    if (s.textContent != null) s.textContent += $" with {StripStarFromDisplayType(bc.displayType)} binary";
                }
            }
        }

        private static void ShowSolarSystem(UIVirtualStarmap starmap, int starIndex)
        {
            ClearStarmap(starmap);
            inSystemDisplay = true;
            // GS3.Warn("ShowSolarSystem");
            // start planet compute thread if not done already
            // Modeler.shouldAbort = false;
            //       PlanetModelingManager.StartPlanetComputeThread();
            // await Modeler.RestartPlanetCalcThread();
            // add star
            var starData = starmap._galaxyData.StarById(starIndex + 1); // because StarById() decrements by 1
            AddStarToStarmap(starmap, starData);
            starData.RunCalculateThread();
            var starScale = starmap.starPool[0].starData.radius / 40f * GS3.Config.VirtualStarmapStarScaleFactor; //This is RadiusAU
            // var starScale = starmap.starPool[0].starData.radius * GS3.Config.VirtualStarmapStarScaleFactor;

            // GS3.Warn($"Scale : {starScale} Radius:{starmap.starPool[0].starData.radius} OrigScale:{starmap.starPool[0].pointRenderer.transform.localScale}");
            starmap.starPool[0].pointRenderer.transform.localScale = new Vector3(starScale, starScale, starScale);
            //starmap.clickText = starData.id.ToString();
            //Debug.Log("Setting it to " + starmap.clickText + " " + starData.id);

            for (var i = 0; i < starData.planetCount; i++)
            {
                // add planets
                var pData = starData.planets[i];

                var isMoon = false;

                var pPos = GetRelativeRotatedPlanetPos(starData, pData, ref isMoon);
                // GS3.Warn("ShowSolarSystem2");
                // request generation of planet surface data to display its details when clicked and if not already loaded
                //if (!pData.loaded) PlanetModelingManager.RequestLoadPlanet(pData);
                // GS3.Warn("ShowSolarSystem3");
                // create fake StarData to pass _OnLateUpdate()
                var dummyStarData = new StarData
                {
                    age = -1,
                    position = pPos
                };
                var gsPlanet = GS3.GetGSPlanet(pData);
                var gsTheme = gsPlanet.GsTheme;
                var orbitColor = PlanetTemperatureToStarColor(gsTheme.Temperature);
                var planetColor = gsTheme.minimapMaterial.Colors.ContainsKey("_Color") ? gsTheme.minimapMaterial.Colors["_Color"] : Color.magenta;
                if (pData.type == EPlanetType.Gas) planetColor = new Color(planetColor.r, planetColor.g, planetColor.b, 0.2f);
                // GS3.Log($"Color of {starData.name} a {starData.typeString} star is {starData.color}");
                dummyStarData.id = pData.id;
                var realRadius = Mathf.Clamp(pData.realRadius, 100, 800);

                var pScale = GS3.Config.VirtualStarmapPlanetScaleFactor * (realRadius / 1000f); //1000
                var planetScale = Vector3.one * pScale;
                // if (scale.x > 3 || scale.y > 3 || scale.z > 3)
                // {
                //     scale = new Vector3(3, 3, 3);
                // }

                // GS3.Warn($"ShowSolarSystem4 {i}");
                // GS3.Warn($"Planet: {starData.planets[i].name}");
                // GS3.Warn($"Pool Length: {i + 1} / {starmap.starPool.Count}");
                while (starmap.starPool.Count <= i + 1)
                {
                    var starNode2 = new UIVirtualStarmap.StarNode
                    {
                        active = false,
                        starData = null,
                        pointRenderer = Object.Instantiate(starmap.starPointPrefab, starmap.starPointPrefab.transform.parent),
                        nameText = Object.Instantiate(starmap.nameTextPrefab, starmap.nameTextPrefab.transform.parent)
                    };
                    starmap.starPool.Add(starNode2);
                }

                starmap.starPool[i + 1].active = true;

                // GS3.Warn("ShowSolarSystem4a");

                starmap.starPool[i + 1].starData = dummyStarData;
                starmap.starPool[i + 1].pointRenderer.material.SetColor(TintColor, planetColor);
                starmap.starPool[i + 1].pointRenderer.transform.localPosition = pPos;
                starmap.starPool[i + 1].pointRenderer.transform.localScale = planetScale;
                starmap.starPool[i + 1].pointRenderer.gameObject.SetActive(true);
                starmap.starPool[i + 1].nameText.text = pData.displayName + " (" + pData.typeString + ")";
                starmap.starPool[i + 1].nameText.color = Color.Lerp(planetColor, Color.white, 0.5f);
                starmap.starPool[i + 1].nameText.rectTransform.sizeDelta = new Vector2(starmap.starPool[i + 1].nameText.preferredWidth, starmap.starPool[i + 1].nameText.preferredHeight);
                starmap.starPool[i + 1].nameText.rectTransform.anchoredPosition = new Vector2(-2000f, -2000f);
                starmap.starPool[i + 1].textContent = pData.displayName + " (" + pData.typeString + ")";

                starmap.starPool[i + 1].nameText.gameObject.SetActive(true);
                // GS3.Warn($"ShowSolarSystem5 {i} / {starmap.connPool.Count}");
                // add orbit renderer
                while (starmap.connPool.Count <= i)
                    starmap.connPool.Add(new UIVirtualStarmap.ConnNode
                    {
                        active = false,
                        starA = null,
                        starB = null,
                        lineRenderer = Object.Instantiate(starmap.connLinePrefab, starmap.connLinePrefab.transform.parent)
                    });
                //starmap.connPool[starmap.connPool.Count-1].lineRenderer.material.SetColor("_LineColorA", Color.Lerp(color, Color.white, 0.65f));
                //starmap.connPool[starmap.connPool.Count - 1].lineRenderer.material.SetColor("_LineColorB", Color.Lerp(color, Color.white, 0.65f));

                //if (starmap.connPool.Count -1 >= i) {
                starmap.connPool[i].active = true;
                starmap.connPool[i].lineRenderer.material.SetColor(LineColorA, Color.Lerp(starmap.starColors.Evaluate(orbitColor), Color.white, 0.65f));
                starmap.connPool[i].lineRenderer.material.SetColor(LineColorB, Color.Lerp(starmap.starColors.Evaluate(orbitColor), Color.white, 0.25f));
                if (starmap.connPool[i].lineRenderer.positionCount != 61) starmap.connPool[i].lineRenderer.positionCount = 61;

                //}
                // GS3.Warn("ShowSolarSystem6");
                for (var j = 0; j < 61; j++)
                {
                    // GS3.Warn("ShowSolarSystem7");
                    var f = j * 0.017453292f * 6f; // ty dsp devs :D
                    Vector3 cPos = GetCenterOfOrbit(starData, pData, ref isMoon);
                    var position = new Vector3(Mathf.Cos(f) * pData.orbitRadius * GS3.Config.VirtualStarmapOrbitScaleFactor + cPos.x, cPos.y, Mathf.Sin(f) * pData.orbitRadius * GS3.Config.VirtualStarmapOrbitScaleFactor + cPos.z);

                    // GS3.Warn("ShowSolarSystem7a");

                    // rotate position around center by orbit angle
                    var quaternion = Quaternion.Euler(pData.orbitInclination, pData.orbitInclination, pData.orbitInclination);
                    var dir = quaternion * (position - cPos);
                    position = dir + cPos;

                    starmap.connPool[i].lineRenderer.SetPosition(j, position);
                }

                // GS3.Warn("ShowSolarSystem7b");

                starmap.connPool[i].lineRenderer.gameObject.SetActive(true);
            }
        }

        private static VectorLF3 GetCenterOfOrbit(StarData starData, PlanetData pData, ref bool isMoon)
        {
            // GS3.Warn("GetCenterOfOrbit");
            if (pData.orbitAroundPlanet != null) return GetRelativeRotatedPlanetPos(starData, pData.orbitAroundPlanet, ref isMoon);

            isMoon = false;
            return starData.position;
        }

        private static VectorLF3 GetRelativeRotatedPlanetPos(StarData starData, PlanetData pData, ref bool isMoon)
        {
            // GS3.Warn("GetRelativeRotatedPlanetPos");
            VectorLF3 pos;
            VectorLF3 dir;
            Quaternion quaternion;
            if (pData.orbitAroundPlanet != null)
            {
                var centerPos = GetRelativeRotatedPlanetPos(starData, pData.orbitAroundPlanet, ref isMoon);
                isMoon = true;
                pos = new VectorLF3(Mathf.Cos(pData.orbitPhase) * pData.orbitRadius * GS3.Config.VirtualStarmapOrbitScaleFactor + centerPos.x, centerPos.y, Mathf.Sin(pData.orbitPhase) * pData.orbitRadius * GS3.Config.VirtualStarmapOrbitScaleFactor + centerPos.z);
                quaternion = Quaternion.Euler(pData.orbitInclination, pData.orbitInclination, pData.orbitInclination);
                dir = quaternion * (pos - centerPos);
                return dir + centerPos;
            }

            pos = new VectorLF3(Mathf.Cos(pData.orbitPhase) * pData.orbitRadius * GS3.Config.VirtualStarmapOrbitScaleFactor + starData.position.x, starData.position.y, Mathf.Sin(pData.orbitPhase) * pData.orbitRadius * GS3.Config.VirtualStarmapOrbitScaleFactor + starData.position.z);
            quaternion = Quaternion.Euler(pData.orbitInclination, pData.orbitInclination, pData.orbitInclination);
            dir = quaternion * (pos - starData.position);
            return dir + starData.position;
        }

        // probably reverse patch this if there is time
        private static void AddStarToStarmap(UIVirtualStarmap starmap, StarData starData)
        {
            // GS3.Warn("AddStarToStarMap");
            var color = starmap.starColors.Evaluate(starData.color);
            if (starData.type == EStarType.NeutronStar)
                color = starmap.neutronStarColor;
            else if (starData.type == EStarType.WhiteDwarf)
                color = starmap.whiteDwarfColor;
            else if (starData.type == EStarType.BlackHole) color = starmap.blackholeColor;

            var num2 = 1.2f;
            if (starData.type == EStarType.GiantStar)
                num2 = 3f;
            else if (starData.type == EStarType.WhiteDwarf)
                num2 = 0.6f;
            else if (starData.type == EStarType.NeutronStar)
                num2 = 0.6f;
            else if (starData.type == EStarType.BlackHole) num2 = 0.8f;

            var gsStar = GS3.GetGSStar(starData);
            var decorative = gsStar.Decorative;
            if (decorative) num2 /= 3;
            var text = starData.displayName + "  ";
            if (starData.type == EStarType.GiantStar)
            {
                if (starData.spectr <= ESpectrType.K)
                    text += "红巨星".Translate();
                else if (starData.spectr <= ESpectrType.F)
                    text += "黄巨星".Translate();
                else if (starData.spectr == ESpectrType.A)
                    text += "白巨星".Translate();
                else
                    text += "蓝巨星".Translate();
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
                text = text + starData.spectr + "型恒星".Translate();
            }


            if (starData.index == (CustomBirthStar != -1 ? CustomBirthStar - 1 : starmap._galaxyData.birthStarId - 1)) text = "即将登陆".Translate() + "\r\n" + text;
            if (!string.IsNullOrEmpty(gsStar.BinaryCompanion))
            {
                var bc = GS3.GetGSStar(gsStar.BinaryCompanion);
                text += $" with {bc.displayType} binary";
            }

            starmap.starPool[0].active = true;
            starmap.starPool[0].starData = starData;
            starmap.starPool[0].pointRenderer.material.SetColor(TintColor, color);
            starmap.starPool[0].pointRenderer.transform.localPosition = starData.position;
            if (decorative) starmap.starPool[0].pointRenderer.transform.localPosition = starData.position + VectorLF3.one;
            starmap.starPool[0].pointRenderer.transform.localScale = Vector3.one * num2 * 2;
            starmap.starPool[0].pointRenderer.gameObject.SetActive(true);
            starmap.starPool[0].nameText.text = text;
            starmap.starPool[0].nameText.color = Color.Lerp(color, Color.white, 0.5f);
            starmap.starPool[0].nameText.rectTransform.sizeDelta = new Vector2(starmap.starPool[0].nameText.preferredWidth, starmap.starPool[0].nameText.preferredHeight);
            starmap.starPool[0].nameText.rectTransform.anchoredPosition = new Vector2(-2000f, -2000f);
            starmap.starPool[0].textContent = text;

            starmap.starPool[0].nameText.gameObject.SetActive(true);
        }

        private static void ClearStarmap(UIVirtualStarmap starmap)
        {
            // GS3.Warn("ClearStarmap");


            foreach (var starNode in starmap.starPool)
            {
                starNode.active = false;
                starNode.starData = null;
                starNode.pointRenderer.gameObject.SetActive(false);
                starNode.nameText.gameObject.SetActive(false);
            }

            foreach (var connNode in starmap.connPool)
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

        public static void InitHelpText(UIGalaxySelect __instance)
        {
            // GS3.Log("1");
            var leftGroup = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/left-group/");
            leftGroup.SetActive(true);
            // GS3.Log("2");
            var helpTextObject = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/left-group/icarus-box/Text");
            
            // GS3.Log("3");
            helpTextObject.SetActive(true);
            helpTextObject.transform.parent.gameObject.SetActive(true);
            var helpText = helpTextObject.GetComponent<Text>();
            // GS3.Log("4");
            if (leftGroup.GetComponentInChildren<Localizer>() != null) Object.DestroyImmediate(leftGroup.GetComponentInChildren<Localizer>());
            helpText.text = "Click star/planet to view system/details\r\nMousewheel to zoom\r\nMovement keys to pan\r\nShift to increase zoom/pan speed\r\nAlt to view all star/planet names\r\nSpace to reset view\r\nRightclick star/planet to set spawn".Translate();
            helpText.alignment = TextAnchor.LowerLeft;
            leftGroup.SetActive(true);
            // GS3.Log("5");
            var leftGroupRect = leftGroup.GetComponent<RectTransform>();
            // GS3.Log("6");
            leftGroupRect.anchorMax = leftGroupRect.anchorMin = leftGroupRect.pivot = Vector2.zero;
            leftGroupRect.offsetMin = new Vector2(0, 1);
            leftGroupRect.offsetMax = new Vector2(300, 20);
            leftGroupRect.anchoredPosition = new Vector2(0, 80);
            leftGroupRect.sizeDelta = new Vector2(300, 300);
        }
    }
}