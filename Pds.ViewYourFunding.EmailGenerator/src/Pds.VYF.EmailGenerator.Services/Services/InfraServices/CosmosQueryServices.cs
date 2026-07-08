// <copyright file="CosmosQueryServices.cs" company="Department for Education - Skill Funding Services">
// Copyright (c) Department for Education - Skill Funding Services. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Enumerations;
using Pds.VYF.EmailGenerator.Services.Extensions;
using Pds.VYF.EmailGenerator.Services.Models.ConfigurationSettings;
using Pds.VYF.EmailGenerator.Services.Models.CosmosModels;
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Services.InfraServices
{
    /// <summary>
    /// The class for CosmosQueryServices.
    /// </summary>
    /// <seealso cref="Pds.VYF.EmailGenerator.Services.Abstract.InfraServices.ICosmosQueryServices" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="CosmosQueryServices" /> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="appSettings">The application settings.</param>
    public class CosmosQueryServices(AppSettings appSettings) : ICosmosQueryServices
    {
        private string? filterVariationReasons;

        /// <summary>
        /// Gets the filter variation reasons.
        /// </summary>
        /// <value>
        /// The filter variation reasons.
        /// </value>
        public string FilterVariationReasons
        {
            get
            {
                if (this.filterVariationReasons == null)
                {
                    this.filterVariationReasons = "[" + appSettings.FundingFilterVariationReasons.AddQuoteInEachValue(",", true) + "]";
                }

                return this.filterVariationReasons;
            }
        }

        /// <summary>
        /// Gets the last feed reader audit query.
        /// </summary>
        /// <returns>string.</returns>
        /// <exception cref="System.NotImplementedException">Please build logic and remove this.</exception>
        public string GetLastFeedReaderAuditQuery()
        {
            return """
                SELECT TOP 1 value c.status
                FROM c
                where c.action = 'Import'
                ORDER BY c._ts desc
                """;
        }

        /// <summary>
        /// Gets the individual provider query.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <returns>
        /// The Query to fetch all the children (Provider Funding) for the given request (funding stream, funding period) and delta data based on status change date.
        /// </returns>
        public string GetChildQuery(ProcessRequest processRequest, string statusChangeDate)
        {
            return $@"
    SELECT DISTINCT c.id {nameof(ChildCosmosModel.Id)}
        , c.fundingStreamCode as {nameof(ChildCosmosModel.FundingStreamCode)}
        , c.fundingPeriodId as {nameof(ChildCosmosModel.FundingPeriodId)}
        , c.partitionKey as {nameof(ChildCosmosModel.UKPRN)}
        , c.provider.name as {nameof(ChildCosmosModel.ProviderName)}
        , FirstStatusChangedDate as {nameof(ChildCosmosModel.StatusChangedDate)}
        , TypeOfFunding as {nameof(ChildCosmosModel.TypeOfFunding)}
    FROM c
    JOIN (SELECT value MIN(PI.statusChangedDate) FROM PI in c.parentInformation) FirstStatusChangedDate
    join (select value array_contains(c.parentInformation, {{ groupingReason : 'Indicative' }}, true)) as IsIndicative
    join (select value DateTimeFromParts(StringToNumber(substring(c.provider.providerDetails.dateOpened, 0, 4)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 5, 2)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 8, 2)))) as DateOpened
    join (select value 2000 + StringToNumber(substring(c.id, 7, 2))) as StartYear
    join (select value 2000 + StringToNumber(substring(c.id, 9, 2))) as EndYear
    join (select value C733['value'] from C733 in c.fundingValue.calculations where C733.templateCalculationId = 733) as C733_DaysInFullYear
    join (select value C567['value'] from C567 in c.fundingValue.calculations where C567.templateCalculationId = 567) as C567_DaysOpenInYear
    join (select value IsIndicative
                        ? is_defined(c.channelVersion) 
                            ? array_contains(c.channelVersion, {{'type' : 'Statement', 'value' : 1}}) 
                                ? '{nameof(TypeOfFundingEnum.IndicativeNew)}' 
                                : '{nameof(TypeOfFundingEnum.IndicativeUpdated)}'
                            : endswith(c.id, '-1_0') 
                                ? '{nameof(TypeOfFundingEnum.IndicativeNew)}' 
                                : '{nameof(TypeOfFundingEnum.IndicativeUpdated)}'
                        : is_defined(c.channelVersion) 
                            ? array_contains(c.channelVersion, {{'type' : 'Statement', 'value' : 1}}) 
                                ? '{nameof(TypeOfFundingEnum.New)}' 
                                : '{nameof(TypeOfFundingEnum.Updated)}'
                            : endswith(c.id, '-1_0') 
                                ? '{nameof(TypeOfFundingEnum.New)}' 
                                : '{nameof(TypeOfFundingEnum.Updated)}') as TypeOfFunding
    WHERE c.fundingStreamCode = '{processRequest.FundingStreamCode}'
            and c.fundingPeriodId = '{processRequest.FundingPeriodId}'
            and FirstStatusChangedDate > '{statusChangeDate}'
            and FirstStatusChangedDate >= '{processRequest.DigitalStatementsGoLiveDate}'
            and (TypeOfFunding = '{nameof(TypeOfFundingEnum.IndicativeNew)}'
                    or TypeOfFunding = '{nameof(TypeOfFundingEnum.IndicativeUpdated)}'
                    or TypeOfFunding = '{nameof(TypeOfFundingEnum.New)}'
                    or endswith(c.id, '-1_0')
                    or EXISTS(select 1 FROM vr in c.variationReasons WHERE ARRAY_CONTAINS({this.FilterVariationReasons}, vr)))";
        }

        /// <summary>
        /// Gets the parent query.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <returns>
        /// The Query to fetch all the parents (Funding) for the given request (funding stream, funding period) and delta data based on status change date.
        /// </returns>
        public string GetParentQuery(ProcessRequest processRequest, string statusChangeDate)
        {
            return $@"
    SELECT DISTINCT c.id {nameof(ParentCosmosModel.Id)}
        , c.fundingStream.code as {nameof(ParentCosmosModel.FundingStreamCode)}
        , c.fundingPeriod.id as {nameof(ParentCosmosModel.FundingPeriodId)}
        , c.partitionKey as {nameof(ParentCosmosModel.UKPRN)}
        , c.organisationGroup.name as {nameof(ParentCosmosModel.OrganizationName)}
        , c.statusChangedDate as {nameof(ParentCosmosModel.StatusChangedDate)}
    FROM c
    join (select value c.groupingReason = 'Indicative' ? true : false) as IsIndicative
    WHERE c.fundingStream.code = '{processRequest.FundingStreamCode}'
            and c.fundingPeriod.id = '{processRequest.FundingPeriodId}'
            and c.statusChangedDate > '{statusChangeDate}'
            and c.statusChangedDate >= '{processRequest.DigitalStatementsGoLiveDate}'
            and c.organisationGroup.groupTypeCode = 'AcademyTrust'
            and (c.groupingReason = 'Payment'
            or c.groupingReason = 'Indicative')
            and ARRAY_LENGTH(c.providerFundings) > 0
            and EXISTS (SELECT 1 FROM PF in c.providerFundings WHERE substring(PF, 12, 8) <> c.partitionKey)
    ORDER BY c.statusChangedDate";
        }

        /// <summary>
        /// Gets the child with parent identifier query.
        /// </summary>
        /// <param name="processRequest">The process request.</param>
        /// <param name="statusChangeDate">The status change date.</param>
        /// <param name="organizationIds">The organization ids.</param>
        /// <returns>
        /// The Query to fetch all the children (Provider Funding) along with its parent for the given request (funding stream, funding period) and delta data based on status change date.
        /// </returns>
        public string GetChildWithParentIdQuery(ProcessRequest processRequest, string statusChangeDate, IEnumerable<string> organizationIds)
        {
            var organizationIdsString = "'" + string.Join("' ,'", organizationIds) + "'";

            return $@"
    SELECT DISTINCT ParentId {nameof(ChildWithParentIdCosmosModel.ParentId)}
        , c.id {nameof(ChildWithParentIdCosmosModel.Id)}
        , c.fundingStreamCode as {nameof(ChildWithParentIdCosmosModel.FundingStreamCode)}
        , c.fundingPeriodId as {nameof(ChildWithParentIdCosmosModel.FundingPeriodId)}
        , c.partitionKey as {nameof(ChildWithParentIdCosmosModel.UKPRN)}
        , c.provider.name as {nameof(ChildWithParentIdCosmosModel.ProviderName)}
        , FirstStatusChangedDate as {nameof(ChildWithParentIdCosmosModel.StatusChangedDate)}
        , TypeOfFunding as {nameof(ChildWithParentIdCosmosModel.TypeOfFunding)}
    FROM c
    JOIN (SELECT value MIN(PI.statusChangedDate) FROM PI in c.parentInformation WHERE PI['group'].groupTypeCode = 'AcademyTrust' and (PI.groupingReason = 'Payment' or PI.groupingReason = 'Indicative')) FirstStatusChangedDate
    JOIN (select value PI.id FROM PI in c.parentInformation WHERE PI['group'].groupTypeCode = 'AcademyTrust' and (PI.groupingReason = 'Payment' or PI.groupingReason = 'Indicative') and PI.statusChangedDate = FirstStatusChangedDate) ParentId
    join (select value array_contains(c.parentInformation, {{ 'group' : {{ groupTypeCode : 'AcademyTrust'  }}, groupingReason : 'Payment' }}, true)) as IsChildOfMAT
    join (select value array_contains(c.parentInformation, {{ 'group' : {{ groupTypeCode : 'AcademyTrust'  }}, groupingReason : 'Indicative' }}, true)) as IsChildOfMATIndicative
    join (select value array_contains(c.parentInformation, {{ groupingReason : 'Indicative' }}, true)) as IsIndicative
    join (select value DateTimeFromParts(StringToNumber(substring(c.provider.providerDetails.dateOpened, 0, 4)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 5, 2)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 8, 2)))) as DateOpened
    join (select value 2000 + StringToNumber(substring(c.id, 7, 2))) as StartYear
    join (select value 2000 + StringToNumber(substring(c.id, 9, 2))) as EndYear
    join (select value C733['value'] from C733 in c.fundingValue.calculations where C733.templateCalculationId = 733) as C733_DaysInFullYear
    join (select value C567['value'] from C567 in c.fundingValue.calculations where C567.templateCalculationId = 567) as C567_DaysOpenInYear
    join (select value IsIndicative
                        ? is_defined(c.channelVersion) 
                            ? array_contains(c.channelVersion, {{'type' : 'Statement', 'value' : 1}}) 
                                ? '{nameof(TypeOfFundingEnum.IndicativeNew)}' 
                                : '{nameof(TypeOfFundingEnum.IndicativeUpdated)}'
                            : endswith(c.id, '-1_0') 
                                ? '{nameof(TypeOfFundingEnum.IndicativeNew)}' 
                                : '{nameof(TypeOfFundingEnum.IndicativeUpdated)}'
                        : is_defined(c.channelVersion) 
                            ? array_contains(c.channelVersion, {{'type' : 'Statement', 'value' : 1}}) 
                                ? '{nameof(TypeOfFundingEnum.New)}' 
                                : '{nameof(TypeOfFundingEnum.Updated)}'
                            : endswith(c.id, '-1_0') 
                                ? '{nameof(TypeOfFundingEnum.New)}' 
                                : '{nameof(TypeOfFundingEnum.Updated)}') as TypeOfFunding
    WHERE c.fundingStreamCode = '{processRequest.FundingStreamCode}'
            and c.fundingPeriodId = '{processRequest.FundingPeriodId}'
            and (IsChildOfMAT = true
            or IsChildOfMATIndicative = true)
            and FirstStatusChangedDate > '{statusChangeDate}'
            and FirstStatusChangedDate >= '{processRequest.DigitalStatementsGoLiveDate}'
            and ParentId in ({organizationIdsString})
            and (TypeOfFunding = '{nameof(TypeOfFundingEnum.IndicativeNew)}'
                    or TypeOfFunding = '{nameof(TypeOfFundingEnum.IndicativeUpdated)}'
                    or TypeOfFunding = '{nameof(TypeOfFundingEnum.New)}'
                    or endswith(c.id, '-1_0')
                    or EXISTS(select 1 FROM vr in c.variationReasons WHERE ARRAY_CONTAINS({this.FilterVariationReasons}, vr)))";
        }
    }
}
