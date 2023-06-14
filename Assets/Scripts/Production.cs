public class Production : BuildItem
{
    //public ProductionAction ProductionAction { get; private set; }

    public override void Initialize(ItemSettings settings, int size)
    {
        //ProductionAction = settings.productionAction;
        base.Initialize(settings, size);
    }
}