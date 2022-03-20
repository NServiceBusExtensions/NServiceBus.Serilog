class BehaviorThatThrowsFeature :
    Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var pipeline = context.Pipeline;
        pipeline.Register(new BehaviorThatThrows.Registration());
    }
}