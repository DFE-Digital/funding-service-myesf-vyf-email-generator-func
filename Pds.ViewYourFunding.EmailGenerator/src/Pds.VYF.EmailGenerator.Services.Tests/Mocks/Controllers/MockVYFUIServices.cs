using Moq;
using Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors;
using Pds.VYF.EmailGenerator.Services.Models.Requests;
using Pds.VYF.EmailGenerator.Services.Models.Responses;

namespace Pds.VYF.EmailGenerator.Services.Tests.Mocks.Controllers
{
    /// <summary>
    /// The mock class for IVYFUIServices.
    /// </summary>
    /// <seealso cref="Moq.Mock&lt;Pds.VYF.EmailGenerator.Services.Abstract.ApiConnectors.IVYFUIServices&gt;" />
    public class MockVYFUIServices(bool isSetupDefault = true) : MockBase<IVYFUIServices>(isSetupDefault)
    {
        /// <summary>
        /// Setups the default.
        /// </summary>
        public override void SetupDefault()
        {
            this.Setup(a => a.GetLatestFundingStreamPublishedDate(It.IsAny<ProcessRequest>())).ReturnsAsync(It.IsAny<DateTime>());
            this.Setup(a => a.GetEmailEnabledFundingStreamAndPeriodsAsync()).ReturnsAsync(It.IsAny<List<EmailEnabledFundingStreamAndPeriodsResponse>>());
        }
    }
}
