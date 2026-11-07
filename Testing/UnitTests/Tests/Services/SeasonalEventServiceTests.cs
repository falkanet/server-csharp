using NUnit.Framework;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Services.Server;

namespace UnitTests.Tests.Services;

[TestFixture]
public class SeasonalEventServiceTests
{
    [TestCase("peacefullZryachiyEvent", "bossZryachiy")]
    [TestCase("sectactPriestEvent", "sectantPriest")]
    [TestCase("arenaFighterEvent", "arenaFighterEvent")]
    [TestCase("bossScourgeEventLab", "bossScourgeEventLab")]
    [TestCase("followerScourgeEventLab", "followerScourgeEventLab")]
    public void GetBaseRoleForEventBot_MappedRoleOrTheRoleItself(string eventRole, string expected)
    {
        var service = DI.GetInstance().GetService<SeasonalEventService>();

        Assert.That(service.GetBaseRoleForEventBot(eventRole), Is.EqualTo(expected));
    }

    /// <summary>
    ///     BotController treats every role containing "event" as an event bot and generates it from its base role:
    ///     each such bot type must lead to a bot type that exists (Scourge and his bikers have no mapping)
    /// </summary>
    [Test]
    public void GetBaseRoleForEventBot_EveryEventBotTypeResolvesToABotType()
    {
        var service = DI.GetInstance().GetService<SeasonalEventService>();
        var types = DI.GetInstance().GetService<BotTable>().Types;

        var eventRoles = types.Keys.Where(role => role.Contains("event", StringComparison.OrdinalIgnoreCase)).ToList();

        Assert.That(eventRoles, Is.Not.Empty);
        foreach (var role in eventRoles)
        {
            var baseRole = service.GetBaseRoleForEventBot(role);
            Assert.That(types.ContainsKey(baseRole.ToLowerInvariant()), Is.True, $"{role} -> {baseRole}");
        }
    }
}
