using System;
using System.Collections.Generic;
using System.Linq;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Messages.Processing;
using CluedIn.ExternalSearch;
using CluedIn.ExternalSearch.Providers.GoogleMaps;
using CluedIn.ExternalSearch.Providers.GoogleMaps.Models;
using CluedIn.Testing.Base.Context;
using CluedIn.Testing.Base.ExternalSearch;
using CluedIn.Testing.Base.Processing.Actors;
using Moq;
using Xunit;
//using TestContext = CluedIn.Tests.Unit.TestContext;

namespace CluedIn.Tests.Integration.ExternalSearch
{
    public class GoogleMapsTests : BaseExternalSearchTest<GoogleMapsExternalSearchProvider>
    {
        [Theory]
        [InlineData("CluedIn APS", "Titangade 11", "Titangade 11, 2200 København, Denmark")]
        [InlineData("CluedIn APS", "11 Titangade", "Titangade 11, 2200 København, Denmark")]
        [InlineData("CluedIn APS", "Denmark, Titangade 11", "Titangade 11, 2200 København, Denmark")]
        // Fails since api call does not yield enough information, 
        // and no persons are returned.
        public void TestClueProduction(string name, string address, string formattedAdress)
        {
            var list = new List<string>(new string[] { "AIzaSyA8oZKYh7NT4bX_yZl8vKIMdecoQCHJC4I" });
            object[] parameters = { list };
            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, name);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Address, address);


            IEntityMetadata entityMetadata = new EntityMetadataPart() {
                EntityType = EntityType.Organization,
                Properties = properties.Properties
            };

            //var tokenProvider = new DummyTokenProvider("AIzaSyA8oZKYh7NT4bX_yZl8vKIMdecoQCHJC4I");

            this.Setup(parameters, entityMetadata);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.AtLeastOnce);
            Assert.True(clues.Count > 0);
            foreach(var clue in this.clues)
            {
                clue.Decompress().Data.EntityData.Properties
                    .TryGetValue("googleMaps.Organization.FormattedAddress", out var value);

                Assert.Equal(formattedAdress, value);
            }
        }

        [Theory]

        [InlineData(null)]
        public void TestNoClueProduction(string name)
        {
            IEntityMetadata entityMetadata = new EntityMetadataPart() {
                Name = name,
                EntityType = EntityType.Organization
            };

            this.Setup(null, entityMetadata);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.Never);
            Assert.True(clues.Count == 0);
        } 

        // TODO Issue 170 - Test Failures
        //[Theory]
        //[InlineData("null")]
        //[InlineData("empty")]
        //[InlineData("nonWorking")]
        //public void TestInvalidApiToken(string provider)
        //{
        //    var tokenProvider = GetProviderByName(provider);
        //    object[] parameters = { tokenProvider };

        //    IEntityMetadata entityMetadata = new EntityMetadataPart()
        //    {
        //        Name = "Sitecore",
        //        EntityType = EntityType.Organization
        //    };

        //    Setup(parameters, entityMetadata);
        //    // Assert
        //    this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.Never);
        //    Assert.True(clues.Count == 0);
        //}

        //[Fact]
        //public void NullTest()
        //{
        //    var dummyTokenProvider = new DummyTokenProvider("1234");
        //    var permIdExternalSearchProvider = new PermIdExternalSearchProvider(dummyTokenProvider);

        //    var query = new ExternalSearchQuery(permIdExternalSearchProvider, EntityType.FAQ, "asd", "gfh");
        //    var result = new ExternalSearchQueryResult<object>(query, null);

        //    var actual = permIdExternalSearchProvider.GetPrimaryEntityMetadata(null, result, null);

        //    Assert.Null(actual);
        //}

        //[Fact]
        //public void TestNoAdditionalInfo()
        //{
        //    var dummyTokenProvider = new DummyTokenProvider("1234");
        //    var permIdExternalSearchProvider = new PermIdExternalSearchProvider(dummyTokenProvider);

        //    var permIdSocialResponse = new PermIdSocialResponse {
        //        AdditionalInfo = null,
        //        DomiciledIn = new List<string>() { "Copenhagen" },
        //        PermId = new List<string>() { "123" },
        //        OrganizationName = new List<string>() { "Organization name" }
        //    };

        //    var query = new ExternalSearchQuery {
        //        ProviderId = new Guid("8bc514e4-3dcd-44ca-9e36-479c0940f646"),
        //        QueryKey = "abc",
        //        EntityType = EntityType.Organization
        //    };

        //    var result = new ExternalSearchQueryResult<PermIdSocialResponse>(query, permIdSocialResponse);
        //    var dummyContext = new TestContext().Context;
        //    var dummyRequest = new DummyRequest();

        //    foreach (var clue in permIdExternalSearchProvider.BuildClues(dummyContext, query, result, dummyRequest))
        //    {
        //        if (clue.Data.EntityData.EntityType != EntityType.Organization)
        //            throw new Exception("EntityType should be Organization, since only data about organization is provided");
        //    }
        //}

        //[Fact]
        //public void TestAdditionalInfo()
        //{
        //    var dummyTokenProvider = new DummyTokenProvider("1234");
        //    var permIdExternalSearchProvider = new PermIdExternalSearchProvider(dummyTokenProvider);

        //    var person = new AssociatedPerson() {
        //        FamilyName = new List<string>() { "FamilyName" },
        //        PersonUrl = new List<string>() { "abc" }
        //    };

        //    var permIdSocialResponse = new PermIdSocialResponse {
        //        AdditionalInfo = new List<AssociatedPerson>() { person },
        //        DomiciledIn = new List<string>() { "Copenhagen" },
        //        PermId = new List<string>() { "123" },
        //        OrganizationName = new List<string>() { "Organization name" }
        //    };

        //    var query = new ExternalSearchQuery {
        //        ProviderId = new Guid("8bc514e4-3dcd-44ca-9e36-479c0940f646"),
        //        QueryKey = "abc",
        //        EntityType = EntityType.Organization
        //    };

        //    var result = new ExternalSearchQueryResult<PermIdSocialResponse>(query, permIdSocialResponse);
        //    var dummyContext = new TestContext().Context;
        //    var dummyRequest = new DummyRequest();

        //    var hasYieldedPerson = false;
        //    foreach (var clue in permIdExternalSearchProvider.BuildClues(dummyContext, query, result, dummyRequest))
        //    {
        //        if (clue.Data.EntityData.EntityType == EntityType.Infrastructure.User)
        //            hasYieldedPerson = true;
        //    }

        //    if (!hasYieldedPerson)
        //        throw new Exception("Should yield at least one person, since data about one has been provided");
        //}

        //public class DummyRequest : IExternalSearchRequest
        //{
        //    public IEntityMetadata EntityMetaData
        //    { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //    public object CustomQueryInput
        //    { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //    public bool? NoRecursion
        //    { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //    public List<Guid> ProviderIds
        //    { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //    public IExternalSearchQueryParameters QueryParameters
        //    { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //    public List<IExternalSearchQuery> Queries
        //    { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //    public bool IsFinished
        //    { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //    public bool AllQueriesHasExecuted => throw new NotImplementedException();
        //}
    }
}