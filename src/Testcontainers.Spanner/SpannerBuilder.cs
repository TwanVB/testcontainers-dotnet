namespace Testcontainers.Spanner;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SpannerBuilder : ContainerBuilder<SpannerBuilder, SpannerContainer, SpannerConfiguration>
{
  private const string DefaultProjectId = "my-project";
  private const string DefaultInstanceId = "my-instance";
  private const string DefaultDatabaseId = "my-database";
  private const string SpannerEmulatorImage = "gcr.io/cloud-spanner-emulator/emulator";


  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerBuilder" /> class.
  /// </summary>
  public SpannerBuilder()
        : this(new SpannerConfiguration())
  {

    DockerResourceConfiguration = Init().DockerResourceConfiguration;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerBuilder" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  private SpannerBuilder(SpannerConfiguration resourceConfiguration)
      : base(resourceConfiguration)
  {
    DockerResourceConfiguration = resourceConfiguration;
  }

  // /// <inheritdoc />
  protected override SpannerConfiguration DockerResourceConfiguration { get; }

  /// <summary>
  /// Sets the DatabaseId.
  /// </summary>
  /// <param name="databaseId">The DatabaseId.</param>
  /// <returns>A configured instance of <see cref="SpannerBuilder" />.</returns>
  public SpannerBuilder WithDatabaseId(string databaseId)
    => Merge(DockerResourceConfiguration, new SpannerConfiguration(databaseId: databaseId));

  /// <summary>
  /// Sets the InstanceId.
  /// </summary>
  /// <param name="instanceId">The InstanceId.</param>
  /// <returns>A configured instance of <see cref="SpannerBuilder" />.</returns>
  public SpannerBuilder WithInstanceId(string instanceId)
    => Merge(DockerResourceConfiguration, new SpannerConfiguration(instanceId: instanceId));

  /// <summary>
  /// Sets the ProjectId.
  /// </summary>
  /// <param name="projectId">The ProjectId.</param>
  /// <returns>A configured instance of <see cref="SpannerBuilder" />.</returns>
  public SpannerBuilder WithProjectId(string projectId)
    => Merge(DockerResourceConfiguration, new SpannerConfiguration(projectId: projectId));


  /// <inheritdoc />
  public override SpannerContainer Build()
  {
    Validate();
    return new SpannerContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
  }


  /// <inheritdoc />
  protected override SpannerBuilder Init()
  {
    return base.Init()
      .WithImage(SpannerEmulatorImage)
      .WithPortBinding(SpannerContainer.InternalGrpcPort, true)
      .WithPortBinding(SpannerContainer.InternalRestPort, true)
      .WithProjectId(DefaultProjectId)
      .WithInstanceId(DefaultInstanceId)
      .WithDatabaseId(DefaultDatabaseId)
      .WithWaitStrategy(
        Wait
          .ForUnixContainer()
          // The default wait for port implementation keeps waiting untill the test times out, therefor now using this custom flavor of the same concept
          .UntilMessageIsLogged($".+REST server listening at 0.0.0.0:{SpannerContainer.InternalRestPort}")
          .UntilMessageIsLogged($".+gRPC server listening at 0.0.0.0:{SpannerContainer.InternalGrpcPort}")
        );

  }


  /// <inheritdoc />
  protected override void Validate()
  {
    base.Validate();

    _ = Guard.Argument(DockerResourceConfiguration.ProjectId, nameof(DockerResourceConfiguration.ProjectId))
      .NotNull()
      .NotEmpty();

    _ = Guard.Argument(DockerResourceConfiguration.InstanceId, nameof(DockerResourceConfiguration.InstanceId))
      .NotNull()
      .NotEmpty();

    _ = Guard.Argument(DockerResourceConfiguration.DatabaseId, nameof(DockerResourceConfiguration.DatabaseId))
      .NotNull()
      .NotEmpty();

  }

  /// <inheritdoc />
  protected override SpannerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new SpannerConfiguration(resourceConfiguration));
  }

  /// <inheritdoc />
  protected override SpannerBuilder Clone(IContainerConfiguration resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new SpannerConfiguration(resourceConfiguration));
  }

  /// <inheritdoc />
  protected override SpannerBuilder Merge(SpannerConfiguration oldValue, SpannerConfiguration newValue)
  {
    return new SpannerBuilder(new SpannerConfiguration(oldValue, newValue));
  }
}
