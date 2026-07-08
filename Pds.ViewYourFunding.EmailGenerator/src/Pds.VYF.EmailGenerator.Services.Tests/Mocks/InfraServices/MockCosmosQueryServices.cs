using Moq;
using Pds.VYF.EmailGenerator.Services.Abstract.InfraServices;
using Pds.VYF.EmailGenerator.Services.Models.Requests;

namespace Pds.VYF.EmailGenerator.Services.Tests.Mocks.InfraServices
{
    /// <summary>
    /// The mock class for ICosmosQueryServices.
    /// </summary>
    /// <seealso cref="Moq.Mock&lt;ICosmosQueryServices&gt;" />
    public class MockCosmosQueryServices(bool isSetupDefault = true) : MockBase<ICosmosQueryServices>(isSetupDefault)
    {
        /// <summary>
        /// Setups the default.
        /// </summary>
        public override void SetupDefault()
        {
            this
                .Setup(a => a.GetParentQuery(It.IsAny<ProcessRequest>(), It.IsAny<string>()))
                .Returns(It.IsAny<string>())
                .Verifiable();

            this
                .Setup(a => a.GetChildQuery(It.IsAny<ProcessRequest>(), It.IsAny<string>()))
                .Returns(It.IsAny<string>())
                .Verifiable();

            this
                .Setup(a => a.GetChildWithParentIdQuery(It.IsAny<ProcessRequest>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(It.IsAny<string>())
                .Verifiable();

            this
                .Setup(a => a.GetLastFeedReaderAuditQuery())
                .Returns(It.IsAny<string>())
                .Verifiable();
        }

        /// <summary>
        /// Setups the get child with parent identifier query.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="query">The query.</param>
        /// <returns>The same object for chaining.</returns>
        public MockCosmosQueryServices SetupGetChildWithParentIdQuery(IEnumerable<string> ids, string query)
        {
            this
                .Setup(a => a.GetChildWithParentIdQuery(It.IsAny<ProcessRequest>(), It.IsAny<string>(), ids))
                .Returns(query)
                .Verifiable();

            return this;
        }
    }
}
