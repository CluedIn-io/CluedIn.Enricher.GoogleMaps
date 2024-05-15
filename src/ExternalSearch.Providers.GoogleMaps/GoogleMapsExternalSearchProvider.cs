﻿using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.ExternalSearch;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Providers.GoogleMaps.Models;
using CluedIn.ExternalSearch.Providers.GoogleMaps.Models.Companies;
using CluedIn.ExternalSearch.Providers.GoogleMaps.Models.Locations;
using CluedIn.ExternalSearch.Providers.GoogleMaps.Vocabularies;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;
using EntityType = CluedIn.Core.Data.EntityType;
using CluedIn.Core.Data.Vocabularies;
using Microsoft.Extensions.Logging;

namespace CluedIn.ExternalSearch.Providers.GoogleMaps
{
    /// <summary>The googlemaps graph external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class GoogleMapsExternalSearchProvider : ExternalSearchProviderBase, IExtendedEnricherMetadata, IConfigurableExternalSearchProvider
    {
        private static EntityType[] AcceptedEntityTypes = { EntityType.Organization };

        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        public GoogleMapsExternalSearchProvider()
            : base(Constants.ProviderId, AcceptedEntityTypes)
        {
            var nameBasedTokenProvider = new NameBasedTokenProvider("GoogleMaps");

            if (nameBasedTokenProvider.ApiToken != null)
                this.TokenProvider = new RoundRobinTokenProvider(nameBasedTokenProvider.ApiToken.Split(',', ';'));
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            foreach (var externalSearchQuery in InternalBuildQueries(context, request))
            {
                yield return externalSearchQuery;
            }
        }
        private IEnumerable<IExternalSearchQuery> InternalBuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config = null)
        {
            if (config.TryGetValue(Constants.KeyName.AcceptedEntityType, out var customType) && !string.IsNullOrWhiteSpace(customType?.ToString()))
            {
                if (!request.EntityMetaData.EntityType.Is(customType.ToString()))
                {
                    yield break;
                }
            }
            else if (!this.Accepts(request.EntityMetaData.EntityType))
                yield break;

            //var entityType = request.EntityMetaData.EntityType;

            //if (string.IsNullOrEmpty(this.TokenProvider.ApiToken))
            //	throw new InvalidOperationException("GoogleMaps ApiToken have not been configured");

            var existingResults = request.GetQueryResults<CompanyDetailsResponse>(this).ToList();

            bool NameFilter(string value)
            {
                return existingResults.Any(r => string.Equals(r.Data.Result.Name, value, StringComparison.InvariantCultureIgnoreCase));
            }

            bool AddressFilter(string value)
            {
                return existingResults.Any(r => string.Equals(r.Data.Result.FormattedAddress, value, StringComparison.InvariantCultureIgnoreCase));
            }

            var entityType = request.EntityMetaData.EntityType;

            var organizationName = GetValue(request, config, Constants.KeyName.OrgNameKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName);
            var organizationAddress = GetValue(request, config, Constants.KeyName.OrgAddressKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Address);
            var organizationZip = GetValue(request, config, Constants.KeyName.OrgZipCodeKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressZipCode);
            var organizationState = GetValue(request, config, Constants.KeyName.OrgStateKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryName);
            var organizationCity = GetValue(request, config, Constants.KeyName.OrgCityKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCity);
            var organizationCountry = GetValue(request, config, Constants.KeyName.OrgCountryKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryName);
            var locationAddress = GetValue(request, config, Constants.KeyName.LocationAddressKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryName);
            var userAddress = GetValue(request, config, Constants.KeyName.UserAddressKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryName);
            var personAddress = GetValue(request, config, Constants.KeyName.PersonAddressKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryName);
            var personAddressCity = GetValue(request, config, Constants.KeyName.PersonAddressCityKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryName);
            var latitude = GetValue(request, config, Constants.KeyName.LatitudeKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryName);
            var longitude = GetValue(request, config, Constants.KeyName.LongitudeKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryName);


            if (organizationName != null && organizationName.Count > 0
                && organizationAddress != null && organizationAddress.Count > 0
                && organizationZip != null && organizationZip.Count > 0
                && organizationState != null && organizationState.Count > 0
                   && organizationCity != null && organizationCity.Count > 0
                && organizationCountry != null && organizationCountry.Count > 0)
            {
                foreach (var nameValue in organizationName.Where(v => !NameFilter(v)))
                {
                    foreach (var addressValue in organizationAddress.Where(v => !AddressFilter(v)))
                    {
                        foreach (var cityValue in organizationCity.Where(v => !AddressFilter(v)))
                        {
                            foreach (var zipValue in organizationZip.Where(v => !AddressFilter(v)))
                            {
                                foreach (var stateValue in organizationState.Where(v => !AddressFilter(v)))
                                {
                                    foreach (var countryValue in organizationCountry.Where(v => !AddressFilter(v)))
                                    {
                                        var companyDict = new Dictionary<string, string>
                                    {
                                        {"companyName", nameValue },
                                        {"companyAddress", $"{addressValue}, {cityValue}, {zipValue}, {stateValue}, {countryValue}" }
                                    };
                                        yield return new ExternalSearchQuery(this, entityType, companyDict);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (organizationName != null && organizationName.Count > 0 && organizationAddress != null && organizationAddress.Count > 0)
            {
                foreach (var nameValue in organizationName.Where(v => !NameFilter(v)))
                {
                    foreach (var addressValue in organizationAddress.Where(v => !AddressFilter(v)))
                    {
                        var companyDict = new Dictionary<string, string>
                        {
                            {"companyName", nameValue },
                            {"companyAddress", addressValue }
                        };
                        yield return new ExternalSearchQuery(this, entityType, companyDict);
                    }
                }
            }

            if (organizationName != null && organizationName.Count > 0)
            {
                foreach (var value in organizationName.Where(v => !NameFilter(v)))
                    yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            }

            if (organizationAddress != null && organizationAddress.Count > 0)
            {
                foreach (var value in organizationAddress.Where(v => !AddressFilter(v)))
                    yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            }

            if (locationAddress != null && locationAddress.Count > 0)
            {
                foreach (var locationNameValue in locationAddress.Where(v => !AddressFilter(v)))
                {
                    var locationDict = new Dictionary<string, string>
                            {
                                {"locationName", locationNameValue }
                            };

                    yield return new ExternalSearchQuery(this, entityType, locationDict);
                }
            }


            if (personAddress != null && personAddress.Count > 0 && personAddressCity != null && personAddressCity.Count > 0)
            {
                foreach (var locationNameValue in personAddress.Where(v => !AddressFilter(v)))
                {
                    foreach (var locationCityValue in personAddressCity.Where(v => !AddressFilter(v)))
                    {

                        var locationDict = new Dictionary<string, string>
                        {
                            {"locationName", $"{locationNameValue}, {locationCityValue}" }
                        };

                        yield return new ExternalSearchQuery(this, entityType, locationDict);
                    }
                }
            }
            else if (personAddress != null && personAddress.Count > 0)
            {
                foreach (var locationNameValue in personAddress.Where(v => !AddressFilter(v)))
                {
                    var locationDict = new Dictionary<string, string>
                        {
                            {"locationName", locationNameValue }
                        };

                    yield return new ExternalSearchQuery(this, entityType, locationDict);
                }
            }
            if (userAddress != null && userAddress.Count > 0)
            {
                foreach (var locationNameValue in userAddress.Where(v => !AddressFilter(v)))
                {
                    var locationDict = new Dictionary<string, string>
                    {
                        {"locationName", locationNameValue }
                    };

                    yield return new ExternalSearchQuery(this, entityType, locationDict);
                }
            }
        }

        private static HashSet<string> GetValue(IExternalSearchRequest request, IDictionary<string, object> config, string keyName, VocabularyKey defaultKey)
        {
            HashSet<string> value;
            if (config.TryGetValue(keyName, out var customVocabKey) && !string.IsNullOrWhiteSpace(customVocabKey?.ToString()))
            {
                value = request.QueryParameters.GetValue<string, HashSet<string>>(customVocabKey.ToString(), new HashSet<string>());
            }
            else
            {
                value = request.QueryParameters.GetValue(defaultKey, new HashSet<string>());
            }

            return value;
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var apiKey = this.TokenProvider.ApiToken;

            foreach (var externalSearchQueryResult in InternalExecuteSearch(context, query, apiKey)) yield return externalSearchQueryResult;
        }

        private static IEnumerable<IExternalSearchQueryResult> InternalExecuteSearch(ExecutionContext context, IExternalSearchQuery query, string apiKey)
        {
            bool isCompany = false;

            var client = new RestClient("https://maps.googleapis.com/maps/api");
            var output = "json";
            var placeDetailsEndpoint = $"place/details/{output}?";
            var placeIdEndpoint = $"place/textsearch/{output}?";

            var placeIdRequest = new RestRequest(placeIdEndpoint, Method.GET);
            placeIdRequest.AddQueryParameter("key", apiKey);

            if (query.QueryParameters.ContainsKey("companyName") && query.QueryParameters.ContainsKey("companyAddress"))
            {
                var input = new
                {
                    name = query.QueryParameters["companyName"].FirstOrDefault(),
                    address = query.QueryParameters["companyAddress"].FirstOrDefault()
                };
                var encodedInput = input.name + " " + input.address;
                placeIdRequest.AddQueryParameter("query", encodedInput);
                //placeIdRequest.AddParameter("inputtype", "textquery");
                isCompany = true;
            }
            else
            {
                if (query.QueryParameters.ContainsKey("locationName"))
                {
                    placeIdRequest.AddParameter("input", query.QueryParameters["locationName"].First());
                }

                if (query.QueryParameters.ContainsKey("coordinates"))
                {
                    var transformedCoordinates = string.Join("", query.QueryParameters["coordinates"]);
                    var splitCoordinates = transformedCoordinates.Split(',');
                    placeIdRequest.AddParameter("locationbias", $"point:{splitCoordinates[0]}, {splitCoordinates[1]}");
                }

                placeIdRequest.AddParameter("inputtype", "textquery");
            }

            IRestResponse<PlaceIdResponse> placeIdResponse = null;

            try
            {
                context.Log.LogInformation("Making Google Maps call. Request: ", JsonUtility.Serialize(placeIdRequest.Parameters));
                placeIdResponse = client.ExecuteTaskAsync<PlaceIdResponse>(placeIdRequest).Result;
            }
            catch(Exception exception)
            {
                context.Log.LogError("Could not return PlaceIdResponse from Google Maps", exception);
            }

            if (placeIdResponse == null)
            {
                yield break;
            }

            if (placeIdResponse.Data.status.Equals("REQUEST_DENIED"))
            {
                context.Log.LogError("REQUEST DENIED by Google Maps");
                yield break;
            }

            if (placeIdResponse.StatusCode == HttpStatusCode.OK)
            {
                if (placeIdResponse.Data != null && isCompany == false)
                {
                    var request = new RestRequest(placeDetailsEndpoint, Method.GET);
                    foreach (var placeId in placeIdResponse.Data.results)
                    {
                        request.AddParameter("placeid", placeId.place_id);
                        request.AddParameter("key", apiKey);
                        //request.AddParameter("fields", "name,formatted_address,address_component,adr_address,geometry");//Removed becasue I want all propertues. 
                    }

                    var response = client.ExecuteTaskAsync<LocationDetailsResponse>(request).Result;
                    if (response.Data.Status.Equals("REQUEST_DENIED"))
                    {
                        yield break;
                    }

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (response.Data != null)
                            yield return new ExternalSearchQueryResult<LocationDetailsResponse>(query, response.Data);
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                        yield break;
                    else if (response.ErrorException != null)
                        throw new AggregateException(response.ErrorException.Message, response.ErrorException);
                    else
                        throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);
                }
                else
                {
                    var request = new RestRequest(placeDetailsEndpoint, Method.GET);
                    foreach (var placeId in placeIdResponse.Data.results)
                    {
                        request.AddParameter("placeid", placeId.place_id);
                        request.AddParameter("key", apiKey);
                    }

                    IRestResponse<CompanyDetailsResponse> response = null;

                    try
                    {
                        context.Log.LogInformation("Making Google Maps call. Request: ", JsonUtility.Serialize(request.Parameters));
                        response = client.ExecuteTaskAsync<CompanyDetailsResponse>(request).Result;
                    }
                    catch(Exception exception)
                    {
                        context.Log.LogError("Could not return CompanyDetailsResponse from Google Maps", exception);
                    }

                    if (response == null)
                    {
                        yield break;

                    }

                    if (response.Data.Status.Equals("REQUEST_DENIED"))
                    {
                        context.Log.LogError("REQUEST DENIED by Google Maps");
                        yield break;
                    }

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (response.Data != null)
                            yield return new ExternalSearchQueryResult<CompanyDetailsResponse>(query, response.Data);
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                        yield break;
                    else if (response.ErrorException != null)
                        throw new AggregateException(response.ErrorException.Message, response.ErrorException);
                    else
                        throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);
                }
            }
        }

        /// <summary>Builds the clues.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {

            if (result is IExternalSearchQueryResult<LocationDetailsResponse> locationDetailsResult)
            {

                var code = this.GetLocationOriginEntityCode(locationDetailsResult, request);

                var clue = new Clue(code, context.Organization);
                clue.Data.EntityData.Codes.Add(request.EntityMetaData.Codes.First());

                this.PopulateLocationMetadata(clue.Data.EntityData, locationDetailsResult, request);

                // TODO: If necessary, you can create multiple clues and return them.

                return new[] { clue };
            }
            else if (result is IExternalSearchQueryResult<CompanyDetailsResponse> companyResult)
            {

                var code = this.GetOrganizationOriginEntityCode(companyResult, request);

                var clue = new Clue(code, context.Organization);

                this.PopulateCompanyMetadata(clue.Data.EntityData, companyResult, request);

                // TODO: If necessary, you can create multiple clues and return them.

                return new[] { clue };
            }

            return null;


        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {

            if (result is IExternalSearchQueryResult<LocationDetailsResponse> locationResult)
            {
                if (locationResult.Data.Result != null)
                {
                    return this.CreateLocationMetadata(locationResult, request);
                }
            }
            else if (result is IExternalSearchQueryResult<CompanyDetailsResponse> companyResult)
            {
                if (companyResult.Data.Result != null)
                {
                    return this.CreateCompanyMetadata(companyResult, request);
                }
            }
            return null;
        }

        /// <summary>Gets the preview image.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The preview image.</returns>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return null;
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateLocationMetadata(IExternalSearchQueryResult<LocationDetailsResponse> resultItem, IExternalSearchRequest request)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateLocationMetadata(metadata, resultItem, request);

            return metadata;
        }

        private IEntityMetadata CreateCompanyMetadata(IExternalSearchQueryResult<CompanyDetailsResponse> resultItem, IExternalSearchRequest request)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateCompanyMetadata(metadata, resultItem, request);

            return metadata;
        }


        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetLocationOriginEntityCode(IExternalSearchQueryResult<LocationDetailsResponse> resultItem, IExternalSearchRequest request)
        {
            return new EntityCode(request.EntityMetaData.EntityType, this.GetCodeOrigin(), request.EntityMetaData.OriginEntityCode.Value);
        }

        private EntityCode GetOrganizationOriginEntityCode(IExternalSearchQueryResult<CompanyDetailsResponse> resultItem, IExternalSearchRequest request)
        {

            return new EntityCode(request.EntityMetaData.EntityType, this.GetCodeOrigin(), request.EntityMetaData.OriginEntityCode.Value);
        }

        /// <summary>Gets the code origin.</summary>
        /// <returns>The code origin</returns>
        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("googlemaps");
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultItem">The result item.</param>
        private void PopulateLocationMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<LocationDetailsResponse> resultItem, IExternalSearchRequest request)
        {
            var code = this.GetLocationOriginEntityCode(resultItem, request);

            metadata.EntityType = request.EntityMetaData.EntityType;
            metadata.Name = request.EntityMetaData.Name;
            metadata.OriginEntityCode = code;
            metadata.Codes.Add(code);
            metadata.Codes.Add(request.EntityMetaData.OriginEntityCode);

            //metadata.Properties[GoogleMapsVocabulary.Location.ComponentsAddress] = JsonUtility.Serialize(resultItem.Data.Result.AddressComponents);
            foreach (var component in resultItem.Data.Result.AddressComponents)
            {
                switch (component.Types.First())
                {
                    case "route":
                        metadata.Properties[GoogleMapsVocabulary.Location.NameStreet] = component.ShortName;
                        break;
                    case "colloquial_area":
                        metadata.Properties[GoogleMapsVocabulary.Location.NameCity] = component.ShortName;
                        break;
                    case "locality":
                        metadata.Properties[GoogleMapsVocabulary.Location.NameCity] = component.ShortName;
                        break;
                    case "administrative_area_level_1":
                        metadata.Properties[GoogleMapsVocabulary.Location.AdministrativeArea] = component.ShortName;
                        break;
                    case "country":
                        metadata.Properties[GoogleMapsVocabulary.Location.CodeCountry] = component.ShortName;
                        break;
                    case "postal_code":
                        metadata.Properties[GoogleMapsVocabulary.Location.CodePostal] = component.ShortName;
                        break;
                }

            }

            metadata.Properties[GoogleMapsVocabulary.Location.Name] = resultItem.Data.Result.Name.PrintIfAvailable();
            //metadata.Properties[GoogleMapsVocabulary.Location.Geometry] = JsonUtility.Serialize(resultItem.Data.Result.Geometry);
            metadata.Properties[GoogleMapsVocabulary.Location.Latitude] = resultItem.Data.Result.Geometry.Location.Lat;
            metadata.Properties[GoogleMapsVocabulary.Location.Longitude] = resultItem.Data.Result.Geometry.Location.Lng;
            metadata.Properties[GoogleMapsVocabulary.Location.FormattedAddress] = resultItem.Data.Result.FormattedAddress.PrintIfAvailable();
        }

        private void PopulateCompanyMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<CompanyDetailsResponse> resultItem, IExternalSearchRequest request)
        {
            var code = this.GetOrganizationOriginEntityCode(resultItem, request);

            metadata.EntityType = request.EntityMetaData.EntityType;
            metadata.Name = request.EntityMetaData.Name;
            metadata.OriginEntityCode = code;
            metadata.Codes.Add(code);
            metadata.Codes.Add(request.EntityMetaData.OriginEntityCode);

            //metadata.Properties[GoogleMapsVocabulary.Organization.AddressComponents] = JsonUtility.Serialize(resultItem.Data.Result.AddressComponents);
            foreach (var component in resultItem.Data.Result.AddressComponents)
            {
                switch (component.Types.First())
                {
                    case "street_number":
                        metadata.Properties[GoogleMapsVocabulary.Organization.StreetNumber] = component.ShortName;
                        break;
                    case "route":
                        metadata.Properties[GoogleMapsVocabulary.Organization.StreetName] = component.ShortName;
                        break;
                    case "locality":
                        metadata.Properties[GoogleMapsVocabulary.Organization.CityName] = component.ShortName;
                        break;
                    case "country":
                        metadata.Properties[GoogleMapsVocabulary.Organization.CountryCode] = component.ShortName;
                        break;
                    case "postal_code":
                        metadata.Properties[GoogleMapsVocabulary.Organization.PostalCode] = component.ShortName;
                        break;
                    case "subpremise":
                        metadata.Properties[GoogleMapsVocabulary.Organization.SubPremise] = component.ShortName;
                        break;
                    case "administrative_area_level_1":
                        metadata.Properties[GoogleMapsVocabulary.Organization.AdministrativeAreaLevel1] = component.ShortName;
                        break;
                    case "administrative_area_level_2":
                        metadata.Properties[GoogleMapsVocabulary.Organization.AdministrativeAreaLevel2] = component.ShortName;
                        break;
                    case "neighborhood":
                        metadata.Properties[GoogleMapsVocabulary.Organization.Neighborhood] = component.ShortName;
                        break;
                }

            }

            metadata.Properties[GoogleMapsVocabulary.Organization.AdrAddress] = resultItem.Data.Result.AdrAddress.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.FormattedAddress] = resultItem.Data.Result.FormattedAddress.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.FormattedPhoneNumber] = resultItem.Data.Result.FormattedPhoneNumber.PrintIfAvailable();
            //metadata.Properties[GoogleMapsVocabulary.Organization.Geometry] = resultItem.Data.Result.Geometry.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Longitude] = resultItem.Data.Result.Geometry.Location.Lng.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Latitude] = resultItem.Data.Result.Geometry.Location.Lat.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Icon] = resultItem.Data.Result.Icon.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Id] = resultItem.Data.Result.PlaceId.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.InternationalPhoneNumber] = resultItem.Data.Result.InternationalPhoneNumber.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Name] = resultItem.Data.Result.Name.PrintIfAvailable();
            //metadata.Properties[GoogleMapsVocabulary.Organization.OpeningHours] = resultItem.Data.Result.OpeningHours.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.PlaceId] = resultItem.Data.Result.PlaceId.PrintIfAvailable();
            //metadata.Properties[GoogleMapsVocabulary.Organization.PlusCode] = JsonUtility.Serialize(resultItem.Data.Result.PlusCode);
            metadata.Properties[GoogleMapsVocabulary.Organization.Rating] = resultItem.Data.Result.Rating.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Reference] = resultItem.Data.Result.Reference.PrintIfAvailable();
            //metadata.Properties[GoogleMapsVocabulary.Organization.Reviews] = JsonUtility.Serialize(resultItem.Data.Result.Reviews);
            //metadata.Properties[GoogleMapsVocabulary.Organization.Scope] = resultItem.Data.Result..PrintIfAvailable();
            //metadata.Properties[GoogleMapsVocabulary.Organization.Types] = resultItem.Data.Result.Types.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Url] = resultItem.Data.Result.Url.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.UserRatingsTotal] = resultItem.Data.Result.UserRatingsTotal.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.UtcOffset] = resultItem.Data.Result.UtcOffset.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Vicinity] = resultItem.Data.Result.Vicinity.PrintIfAvailable();
            metadata.Properties[GoogleMapsVocabulary.Organization.Website] = resultItem.Data.Result.Website.PrintIfAvailable();

            metadata.Properties[GoogleMapsVocabulary.Organization.BusinessStatus] = resultItem.Data.Result.BusinessStatus.PrintIfAvailable();
        }

        public IEnumerable<EntityType> Accepts(IDictionary<string, object> config, IProvider provider)
        {
            var customTypes = config[Constants.KeyName.AcceptedEntityType].ToString();
            if (string.IsNullOrWhiteSpace(customTypes))
            {
                AcceptedEntityTypes = new EntityType[] { config[Constants.KeyName.AcceptedEntityType].ToString() };
            };

            return AcceptedEntityTypes;
        }

        public IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return InternalBuildQueries(context, request, config);
        }

        public IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query, IDictionary<string, object> config, IProvider provider)
        {
            var jobData = new GoogleMapsExternalSearchJobData(config);

            foreach (var externalSearchQueryResult in InternalExecuteSearch(context, query, jobData.ApiToken)) yield return externalSearchQueryResult;
        }

        public IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return BuildClues(context, query, result, request);
        }

        public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return GetPrimaryEntityMetadata(context, result, request);
        }

        public IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return GetPrimaryEntityPreviewImage(context, result, request);
        }

        public string Icon { get; } = Constants.Icon;
        public string Domain { get; } = Constants.Domain;
        public string About { get; } = Constants.About;

        public AuthMethods AuthMethods { get; } = Constants.AuthMethods;
        public IEnumerable<Control> Properties { get; } = Constants.Properties;
        public Guide Guide { get; } = Constants.Guide;
        public IntegrationType Type { get; } = Constants.IntegrationType;

        
    }
}