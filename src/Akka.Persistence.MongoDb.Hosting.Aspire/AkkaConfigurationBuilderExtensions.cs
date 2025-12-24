using Akka.Actor;
using Akka.Hosting;
using Akka.Persistence.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Akka.Persistence.MongoDb.Hosting.Aspire;

public static class AkkaConfigurationBuilderExtensions
{
    private const string DefaultConfigSectionName = "Aspire:MongoDB:Driver";

    /// <summary>
    ///     Adds Akka.Persistence.MongoDb support to this <see cref="ActorSystem"/> with optional support
    ///     for health checks on both journal and snapshot store.
    /// </summary>
    /// <param name="builder">
    ///     The builder instance being configured.
    /// </param>
    /// <param name="provider">
    /// </param>
    /// <param name="connectionName">
    /// </param>
    /// <param name="mode">
    ///     <para>
    ///         Determines which settings should be added by this method call.
    ///     </para>
    ///     <i>Default</i>: <see cref="PersistenceMode.Both"/>
    /// </param>
    /// <param name="autoInitialize">
    ///     <para>
    ///         Should the SQL store table be initialized automatically.
    ///     </para>
    ///     <i>Default</i>: <c>false</c>
    /// </param>
    /// <param name="journalBuilder">
    ///     <para>
    ///         An <see cref="Action{T}"/> used to configure an <see cref="AkkaPersistenceJournalBuilder"/> instance.
    ///     </para>
    ///     <i>Default</i>: <c>null</c>
    /// </param>
    /// <param name="snapshotBuilder">
    ///     <para>
    ///         An <see cref="Action{T}"/> used to configure an <see cref="AkkaPersistenceSnapshotBuilder"/> instance.
    ///     </para>
    ///     <i>Default</i>: <c>null</c>
    /// </param>
    /// <param name="pluginIdentifier">
    ///     <para>
    ///         The configuration identifier for the plugins
    ///     </para>
    ///     <i>Default</i>: <c>"sql-server"</c>
    /// </param>
    /// <param name="isDefaultPlugin">
    ///     <para>
    ///         A <c>bool</c> flag to set the plugin as the default persistence plugin for the <see cref="ActorSystem"/>
    ///     </para>
    ///     <b>Default</b>: <c>true</c>
    /// </param>
    /// <returns>
    ///     The same <see cref="AkkaConfigurationBuilder"/> instance originally passed in.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when <see cref="journalBuilder"/> is set and <see cref="mode"/> is set to
    ///     <see cref="PersistenceMode.SnapshotStore"/>
    /// </exception>
    /// <example>
    /// <code>
    /// builder.WithMongoDbPersistence(
    ///     connectionString: "...",
    ///     journalBuilder: journal => journal
    ///         .AddEventAdapter&lt;MyAdapter&gt;("adapter", new[] { typeof(MyEvent) })
    ///         .WithHealthCheck(HealthStatus.Degraded),
    ///     snapshotBuilder: snapshot => snapshot
    ///         .WithHealthCheck(HealthStatus.Degraded));
    /// </code>
    /// </example>
    public static AkkaConfigurationBuilder WithMongoDbPersistence(
        this AkkaConfigurationBuilder builder,
        IServiceProvider provider,
        string connectionName,
        PersistenceMode mode = PersistenceMode.Both,
        Action<AkkaPersistenceJournalBuilder>? journalBuilder = null,
        Action<AkkaPersistenceSnapshotBuilder>? snapshotBuilder = null,
        string pluginIdentifier = "mongodb",
        bool isDefaultPlugin = true)
    {
        var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString(connectionName);
        if (mode == PersistenceMode.SnapshotStore && journalBuilder is not null)
        {
            throw new Exception(
                $"{nameof(journalBuilder)} can only be set when {nameof(mode)} is set to either {PersistenceMode.Both} or {PersistenceMode.Journal}");
        }

        if (mode == PersistenceMode.Journal && snapshotBuilder is not null)
        {
            throw new Exception(
                $"{nameof(snapshotBuilder)} can only be set when {nameof(mode)} is set to either {PersistenceMode.Both} or {PersistenceMode.SnapshotStore}");
        }

        var journalOpt = new MongoDbJournalOptions(isDefaultPlugin, pluginIdentifier)
        {
            ConnectionString = connectionString
        };

        var snapshotOpt = new MongoDbSnapshotOptions(isDefaultPlugin, pluginIdentifier)
        {
            ConnectionString = connectionString
        };

        return mode switch
        {
            PersistenceMode.Journal =>
                builder.WithMongoDbPersistence(journalOpt, null, journalBuilder, snapshotBuilder),
            PersistenceMode.SnapshotStore => builder.WithMongoDbPersistence(null, snapshotOpt, journalBuilder,
                snapshotBuilder),
            PersistenceMode.Both => builder.WithMongoDbPersistence(journalOpt, snapshotOpt, journalBuilder,
                snapshotBuilder),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Invalid PersistenceMode defined.")
        };
    }
}