namespace PeterDoStuff.Games._2048
{
    public class VariantGame : Game
    {
        public VariantGame(int width, int height, int startBlocks, int spawnBlocks)
        {
            Clear();
            Width = width;
            Height = height;
            StartBlocks = startBlocks;
            SpawnBlocks = spawnBlocks;
            RandomSetup();
        }
    }
}
