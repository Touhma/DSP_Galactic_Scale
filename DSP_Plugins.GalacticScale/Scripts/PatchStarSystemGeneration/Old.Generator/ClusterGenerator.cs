//using System;
//using System.Collections.Generic;
//using HarmonyLib;

//namespace GalacticScale.Scripts.PatchStarSystemGeneration.Generator {
//    public static class ClusterGenerator {
//        public static GeneratorCluster PreGenCluster(GameDesc gameDesc) {
//            GeneratorGlobalSettings globalSettings = Bootstrap.gen;
            
//            GeneratorCluster preGenCluster = new GeneratorCluster();

//            GeneratorClusterSettings clusterSettings = globalSettings.GeneratorClusterSettings;

//            preGenCluster.seed = gameDesc.galaxySeed;
//            preGenCluster.starCount = gameDesc.starCount;
//            preGenCluster.algo = gameDesc.galaxyAlgo;

//            Random randomGalaxySeed = new Random(gameDesc.galaxySeed);

//            var GenerateTempPoses = new Traverse(typeof(UniverseGen)).Method("GenerateTempPoses",
//                new Type[] {
//                    typeof(int),
//                    typeof(int),
//                    typeof(int),
//                    typeof(double),
//                    typeof(double),
//                    typeof(double),
//                    typeof(double)
//                },
//                new object[] {
//                    randomGalaxySeed.Next(),
//                    preGenCluster.starCount,
//                    clusterSettings.IterationCount,
//                    clusterSettings.MinStarDistances,
//                    clusterSettings.MinStepLen,
//                    clusterSettings.MaxStepLen,
//                    clusterSettings.Flatten
//                }
//            ).GetValue<int>();

//            List<GeneratorSystem> pregenSystems = new List<GeneratorSystem>();
            
//            int tempPoses = GenerateTempPoses;

//            for (var index = 0; index < tempPoses; index++) {
//                //Adding the new system
//                pregenSystems.Add(new GeneratorSystem());
                
//                double draw = randomGalaxySeed.NextDouble();
//                double draw2 = randomGalaxySeed.NextDouble();
//                EStarType starTypeSelected = EStarType.MainSeqStar;
//                ESpectrType starSpectrTypeSelected = ESpectrType.X;
         
                
//                foreach (var clusterSettingsStarTypeChance in clusterSettings.StarTypeChances) {
//                    if (draw <= clusterSettingsStarTypeChance.Value) {
//                        starTypeSelected = clusterSettingsStarTypeChance.Key;
//                        break;
//                    }
//                }

//                if (starTypeSelected == EStarType.MainSeqStar) {
//                    foreach (var spectralTypeChance in clusterSettings.SpectralTypeChances) {
//                        if (draw2 <= spectralTypeChance.Value) {
//                            starSpectrTypeSelected = spectralTypeChance.Key;
//                            break;
//                        }
//                    }
//                }
                
//                int seed = randomGalaxySeed.Next();

//                //PreGeneration of the star
//                GeneratorStar starToAdd ;
//                if (index != 0) {
//                    starToAdd = StarGenerator.PreGenStar(  seed ,globalSettings.BirthStarSpectr, globalSettings.BirthStarType);
//                }
//                else {
//                    if (globalSettings.UseCustomBirthStar) {
//                        starToAdd =  StarGenerator.PreGenBirthStar(seed, globalSettings.BirthStarSpectr, globalSettings.BirthStarType);
//                    }
//                    else {
//                        starToAdd =  StarGenerator.PreGenBirthStar(seed, starSpectrTypeSelected, starTypeSelected);
//                    }
//                }
                
//                pregenSystems[index].SystemStar = starToAdd;
//            }
            
            
//            // Pre-gen the actual stellar system
//            preGenCluster.ClusterSystems = pregenSystems;
            
//            return preGenCluster;
//        }
//    }
//}