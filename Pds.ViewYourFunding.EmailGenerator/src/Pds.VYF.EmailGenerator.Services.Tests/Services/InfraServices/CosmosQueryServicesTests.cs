// <copyright file="CosmosQueryServicesTests.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using FluentAssertions;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;
using Pds.VYF.EmailGenerator.Services.Models.Requests;
using Pds.VYF.EmailGenerator.Services.Services.InfraServices;

namespace Pds.VYF.EmailGenerator.Services.Tests.Services.InfraServices
{
    /// <summary>
    /// A class for CosmosQueryServices Tests.
    /// </summary>
    [TestClass]
    public class CosmosQueryServicesTests
    {
        private const string TestParentQueryExpectedResult = @"
    SELECT DISTINCT c.id Id
        , c.fundingStream.code as FundingStreamCode
        , c.fundingPeriod.id as FundingPeriodId
        , c.partitionKey as UKPRN
        , c.organisationGroup.name as OrganizationName
        , c.statusChangedDate as StatusChangedDate
    FROM c
    join (select value c.groupingReason = 'Indicative' ? true : false) as IsIndicative
    WHERE c.fundingStream.code = 'GAG'
            and c.fundingPeriod.id = 'AC-2425'
            and c.statusChangedDate > '2024-01-01T11:10:10.12345'
            and c.statusChangedDate >= '2024-06-25T00:00:00+00:00'
            and c.organisationGroup.groupTypeCode = 'AcademyTrust'
            and (c.groupingReason = 'Payment'
            or c.groupingReason = 'Indicative')
            and ARRAY_LENGTH(c.providerFundings) > 0
            and EXISTS (SELECT 1 FROM PF in c.providerFundings WHERE substring(PF, 12, 8) <> c.partitionKey)
    ORDER BY c.statusChangedDate";

        private const string TestChildQueryExpectedResult = @"
    SELECT DISTINCT c.id Id
        , c.fundingStreamCode as FundingStreamCode
        , c.fundingPeriodId as FundingPeriodId
        , c.partitionKey as UKPRN
        , c.provider.name as ProviderName
        , FirstStatusChangedDate as StatusChangedDate
        , TypeOfFunding as TypeOfFunding
    FROM c
    JOIN (SELECT value MIN(PI.statusChangedDate) FROM PI in c.parentInformation) FirstStatusChangedDate
    join (select value array_contains(c.parentInformation, { groupingReason : 'Indicative' }, true)) as IsIndicative
    join (select value DateTimeFromParts(StringToNumber(substring(c.provider.providerDetails.dateOpened, 0, 4)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 5, 2)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 8, 2)))) as DateOpened
    join (select value 2000 + StringToNumber(substring(c.id, 7, 2))) as StartYear
    join (select value 2000 + StringToNumber(substring(c.id, 9, 2))) as EndYear
    join (select value C733['value'] from C733 in c.fundingValue.calculations where C733.templateCalculationId = 733) as C733_DaysInFullYear
    join (select value C567['value'] from C567 in c.fundingValue.calculations where C567.templateCalculationId = 567) as C567_DaysOpenInYear
    join (select value IsIndicative
                        ? is_defined(c.channelVersion) 
                            ? array_contains(c.channelVersion, {'type' : 'Statement', 'value' : 1}) 
                                ? 'IndicativeNew' 
                                : 'IndicativeUpdated'
                            : endswith(c.id, '-1_0') 
                                ? 'IndicativeNew' 
                                : 'IndicativeUpdated'
                        : is_defined(c.channelVersion) 
                            ? array_contains(c.channelVersion, {'type' : 'Statement', 'value' : 1}) 
                                ? 'New' 
                                : 'Updated'
                            : endswith(c.id, '-1_0') 
                                ? 'New' 
                                : 'Updated') as TypeOfFunding
    WHERE c.fundingStreamCode = 'GAG'
            and c.fundingPeriodId = 'AC-2425'
            and FirstStatusChangedDate > '2024-01-01T11:10:10.12345'
            and FirstStatusChangedDate >= '2024-06-25T00:00:00+00:00'
            and (TypeOfFunding = 'IndicativeNew'
                    or TypeOfFunding = 'IndicativeUpdated'
                    or TypeOfFunding = 'New'
                    or endswith(c.id, '-1_0')
                    or EXISTS(select 1 FROM vr in c.variationReasons WHERE ARRAY_CONTAINS(['LegalNameFieldUpdated','TrustCodeFieldUpdated','FundingUpdated','ProfilingUpdated','CalculationValuesUpdated','DateOpenedFieldUpdated','DateClosedFieldUpdated','TrustStatusFieldUpdated'], vr)))";

        private const string TestChildWithParentIdQueryExpectedResult = @"
    SELECT DISTINCT ParentId ParentId
        , c.id Id
        , c.fundingStreamCode as FundingStreamCode
        , c.fundingPeriodId as FundingPeriodId
        , c.partitionKey as UKPRN
        , c.provider.name as ProviderName
        , FirstStatusChangedDate as StatusChangedDate
        , TypeOfFunding as TypeOfFunding
    FROM c
    JOIN (SELECT value MIN(PI.statusChangedDate) FROM PI in c.parentInformation WHERE PI['group'].groupTypeCode = 'AcademyTrust' and (PI.groupingReason = 'Payment' or PI.groupingReason = 'Indicative')) FirstStatusChangedDate
    JOIN (select value PI.id FROM PI in c.parentInformation WHERE PI['group'].groupTypeCode = 'AcademyTrust' and (PI.groupingReason = 'Payment' or PI.groupingReason = 'Indicative') and PI.statusChangedDate = FirstStatusChangedDate) ParentId
    join (select value array_contains(c.parentInformation, { 'group' : { groupTypeCode : 'AcademyTrust'  }, groupingReason : 'Payment' }, true)) as IsChildOfMAT
    join (select value array_contains(c.parentInformation, { 'group' : { groupTypeCode : 'AcademyTrust'  }, groupingReason : 'Indicative' }, true)) as IsChildOfMATIndicative
    join (select value array_contains(c.parentInformation, { groupingReason : 'Indicative' }, true)) as IsIndicative
    join (select value DateTimeFromParts(StringToNumber(substring(c.provider.providerDetails.dateOpened, 0, 4)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 5, 2)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 8, 2)))) as DateOpened
    join (select value 2000 + StringToNumber(substring(c.id, 7, 2))) as StartYear
    join (select value 2000 + StringToNumber(substring(c.id, 9, 2))) as EndYear
    join (select value C733['value'] from C733 in c.fundingValue.calculations where C733.templateCalculationId = 733) as C733_DaysInFullYear
    join (select value C567['value'] from C567 in c.fundingValue.calculations where C567.templateCalculationId = 567) as C567_DaysOpenInYear
    join (select value IsIndicative
                        ? is_defined(c.channelVersion) 
                            ? array_contains(c.channelVersion, {'type' : 'Statement', 'value' : 1}) 
                                ? 'IndicativeNew' 
                                : 'IndicativeUpdated'
                            : endswith(c.id, '-1_0') 
                                ? 'IndicativeNew' 
                                : 'IndicativeUpdated'
                        : is_defined(c.channelVersion) 
                            ? array_contains(c.channelVersion, {'type' : 'Statement', 'value' : 1}) 
                                ? 'New' 
                                : 'Updated'
                            : endswith(c.id, '-1_0') 
                                ? 'New' 
                                : 'Updated') as TypeOfFunding
    WHERE c.fundingStreamCode = 'GAG'
            and c.fundingPeriodId = 'AC-2425'
            and (IsChildOfMAT = true
            or IsChildOfMATIndicative = true)
            and FirstStatusChangedDate > '2024-01-01T11:10:10.12345'
            and FirstStatusChangedDate >= '2024-06-25T00:00:00+00:00'
            and ParentId in ('GAG-AC-2425-Payment-AcademyTrust-10093825-1_0' ,'GAG-AC-2425-Payment-AcademyTrust-10093825-2_0')
            and (TypeOfFunding = 'IndicativeNew'
                    or TypeOfFunding = 'IndicativeUpdated'
                    or TypeOfFunding = 'New'
                    or endswith(c.id, '-1_0')
                    or EXISTS(select 1 FROM vr in c.variationReasons WHERE ARRAY_CONTAINS(['LegalNameFieldUpdated','TrustCodeFieldUpdated','FundingUpdated','ProfilingUpdated','CalculationValuesUpdated','DateOpenedFieldUpdated','DateClosedFieldUpdated','TrustStatusFieldUpdated'], vr)))";

        private readonly AppSettings appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosQueryServicesTests"/> class.
        /// </summary>
        public CosmosQueryServicesTests()
        {
            this.appSettings = new AppSettings()
            {
                FundingFilterVariationReasons = "LegalNameFieldUpdated,TrustCodeFieldUpdated,FundingUpdated,ProfilingUpdated,CalculationValuesUpdated,DateOpenedFieldUpdated,DateClosedFieldUpdated,TrustStatusFieldUpdated",
            };
        }

        /// <summary>
        /// Gets the individual provider query success.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="expectedSql">The expected SQL.</param>
        [TestMethod]
        [DataRow("GAG", "AC-2425", "2024-01-01T11:10:10.12345", TestChildQueryExpectedResult)]
        public void GetChildQuery_Success(string fundingStreamCode, string fundingPeriodId, string statusChangeDate, string expectedSql)
        {
            ProcessRequest processRequest = new()
            {
                FundingStreamCode = fundingStreamCode,
                FundingPeriodId = fundingPeriodId,
                DigitalStatementsGoLiveDate = "2024-06-25T00:00:00+00:00",
            };

            var cosmosQueryServices = new CosmosQueryServices(this.appSettings);

            // Act
            var result = cosmosQueryServices.GetChildQuery(processRequest, statusChangeDate);

            // Assert
            result.Should().BeEquivalentTo(expectedSql);
        }

        /// <summary>
        /// Gets the parent query success.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="expectedSql">The expected SQL.</param>
        [TestMethod]
        [DataRow("GAG", "AC-2425", "2024-01-01T11:10:10.12345", TestParentQueryExpectedResult)]
        public void GetParentQuery_Success(string fundingStreamCode, string fundingPeriodId, string statusChangeDate, string expectedSql)
        {
            ProcessRequest processRequest = new()
            {
                FundingStreamCode = fundingStreamCode,
                FundingPeriodId = fundingPeriodId,
                DigitalStatementsGoLiveDate = "2024-06-25T00:00:00+00:00",
            };

            var cosmosQueryServices = new CosmosQueryServices(this.appSettings);

            // Act
            var result = cosmosQueryServices.GetParentQuery(processRequest, statusChangeDate);

            // Assert
            result.Should().BeEquivalentTo(expectedSql);
        }

        /// <summary>
        /// Gets the child with parent identifier query success.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="fundingPeriodId">The funding period identifier.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="organizationId">The organization identifier.</param>
        /// <param name="expectedSql">The expected SQL.</param>
        [TestMethod]
        [DataRow("GAG", "AC-2425", "2024-01-01T11:10:10.12345", "GAG-AC-2425-Payment-AcademyTrust-10093825-1_0,GAG-AC-2425-Payment-AcademyTrust-10093825-2_0", TestChildWithParentIdQueryExpectedResult)]
        public void GetChildWithParentIdQuery_Success(string fundingStreamCode, string fundingPeriodId, string statusChangeDate, string organizationId, string expectedSql)
        {
            ProcessRequest processRequest = new()
            {
                FundingStreamCode = fundingStreamCode,
                FundingPeriodId = fundingPeriodId,
                DigitalStatementsGoLiveDate = "2024-06-25T00:00:00+00:00",
            };

            var cosmosQueryServices = new CosmosQueryServices(this.appSettings);

            // Act
            var result = cosmosQueryServices.GetChildWithParentIdQuery(processRequest, statusChangeDate, organizationId.Split(","));

            // Assert
            result.Should().BeEquivalentTo(expectedSql);
        }
    }
}
