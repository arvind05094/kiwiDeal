using kiwiDeal.Tests.Integration.Users;

namespace kiwiDeal.Tests.Integration;

[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<KiwiDealWebApplicationFactory>
{
}
