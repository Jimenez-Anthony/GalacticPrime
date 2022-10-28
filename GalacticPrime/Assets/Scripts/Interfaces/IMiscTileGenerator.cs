using UnityEngine.Tilemaps;

public interface IMiscTileGenerator
{
    TileBase GetTile();
    int[,] Generate(int[,] map, System.Random rng);
}
