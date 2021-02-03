using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DSP_Plugin {
    public static class Utils {
        // this should return the cargo node from a belt id
        public static CargoPath GetPathWithBeltId(PlanetFactory factory, int beltId) {
            CargoPath cargoLine = null;
            foreach (CargoPath cargoPath in factory.cargoTraffic.pathPool) {
                if (cargoPath != null && cargoPath.belts.Contains(beltId)) {
                    // get the line of conveyor from the belt ID
                    cargoLine = cargoPath;
                }
            }

            return cargoLine;
        }

        //this should return the list of the conveyors belts entities from a specific node
        public static List<EntityData> GetBeltsEntitiesByCargoPathBuildRange(
            PlanetFactory factory,
            CargoPath path) {
            List<EntityData> beltEntitiesList = new List<EntityData>();

            foreach (EntityData entityData in factory.entityPool) {
                if (path.belts.Contains(entityData.beltId)) {
                    if (CheckIfInBuildDistance(entityData.pos)) {
                        beltEntitiesList.Add(entityData);
                    }
                }
            }

            return beltEntitiesList;
        }

        // return all entities from a proto in the mecha build range 
        public static List<EntityData> GetEntitiesByProtoBuildRange(
            PlanetFactory factory,
            ItemProto itemProto) {
            List<EntityData> entityDataList = new List<EntityData>();
            foreach (EntityData entityData in factory.entityPool) {
                if ((uint) entityData.id > 0U && entityData.protoId == itemProto.ID &&
                    CheckIfInBuildDistance(entityData.pos)) {
                    entityDataList.Add(entityData);
                }
            }

            return entityDataList;
        }

        // return all entities from a proto in the mecha build range 
        public static List<EntityData> GetEntitySortedByTypeAndRadius(
            ItemProto itemProto, List<EntityData> baseList, Vector3 startPoint, float radius) {
            List<EntityData> entityDataList = new List<EntityData>();
            foreach (EntityData entityData in baseList) {
                if ((uint) entityData.id > 0U && entityData.protoId == itemProto.ID &&
                    (entityData.pos - startPoint).magnitude <= radius) {
                    entityDataList.Add(entityData);
                }
            }

            return entityDataList;
        }

        public static List<EntityData> GetEntitiesByProto(
            PlanetFactory factory,
            ItemProto itemProto) {
            List<EntityData> entityDataList = new List<EntityData>();
            if (itemProto != null) {
                foreach (EntityData entityData in factory.entityPool) {
                    if ((uint) entityData.id > 0 && entityData.protoId == itemProto.ID) {
                        entityDataList.Add(entityData);
                    }
                }
            }

            return entityDataList;
        }

        public static bool CheckIfInBuildDistance(Vector3 position) {
            return ((position - GameMain.data.mainPlayer.position).sqrMagnitude <=
                    GameMain.data.mainPlayer.mecha.buildArea *
                    GameMain.data.mainPlayer.mecha.buildArea);
        }
    }
}