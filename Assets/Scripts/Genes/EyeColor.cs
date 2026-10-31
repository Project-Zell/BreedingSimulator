public class EyeColor : Gene
{
    public static readonly EyeColor[] Genes;

    public void LoadGenes(string json)
    {
        //
    }
    
    private EyeColor(byte value, string key, string hexColor) : base(value, key, hexColor) { }
}