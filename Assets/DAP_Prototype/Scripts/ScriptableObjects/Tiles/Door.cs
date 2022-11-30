using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPG.ScriptableObjects.Tiles
{
    [CreateAssetMenu(fileName = "New Door", menuName = "Tile/Door")]
    public class Door : Tile
    {
        #region interop

        [System.NonSerialized]
        private Managers.PersistentManagers.ClientSequences.CampaignSequence campaignSequence = null;

        private Managers.PersistentManagers.ClientSequences.CampaignSequence CampaignSequence()
        {
#if UNITY_EDITOR
            if (! Application.isPlaying) return null;
#endif
            if (campaignSequence == null)
                campaignSequence = Managers.PersistentManagers.ClientSequenceManager.Instance?.CampaignSequence;
            return campaignSequence;
        }

        public void OnEnable()
        {
#if UNITY_EDITOR
            if (! Application.isPlaying) return;
#endif
            campaignSequence = Managers.PersistentManagers.ClientSequenceManager.Instance?.CampaignSequence;
        }

        public void OnDisable()
        {
            campaignSequence = null;
        }

        #endregion

        private class DoorState : Core.Shared.Campaign.AreaStateData.ITileState
        {
            public bool open = false;

            public void Open()
            {
                open = true;
            }
        }

        public Sprite spriteOpen;

        public bool startsOpen = false;

        public override void GetTileData(
            Vector3Int location,
            ITilemap tileMap,
            ref TileData tileData
        )
        {
            base.GetTileData(location, tileMap, ref tileData);
            tileData.sprite = (
                (
                    (DoorState)(CampaignSequence()?.GetTileState(tileMap, location))
                )?.open ?? startsOpen
            ) ? spriteOpen : sprite;
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            tilemap.RefreshTile(position);
        }

        public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
        {
            CampaignSequence()?.RegisterTileState(
                tilemap,
                location,
                new DoorState { open = startsOpen }
            );
            return true;
        }

        public void Open(Vector3Int position, Tilemap tilemap)
        {
            ((DoorState)(CampaignSequence()?.GetTileState(tilemap, position)))?.Open();
            tilemap.RefreshTile(position);
        }
    }
}