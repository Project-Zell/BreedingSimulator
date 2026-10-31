public class BreedContainer
{
    public readonly Unit FatherUnit;
    public readonly Unit MotherUnit;

    public readonly SpeciesData FatherData;
    public readonly SpeciesData MotherData;

    public BreedContainer((Unit unit, SpeciesData data) father, (Unit unit, SpeciesData data) mother)
    {
        FatherUnit = father.unit;
        FatherData = father.data;

        MotherUnit = mother.unit;
        MotherData = mother.data;
    }
}