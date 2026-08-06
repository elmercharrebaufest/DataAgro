using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.ControlBoletosAgenteIA;

namespace Molinos.DataAgro.Interfaces.Agent
{
    public interface IControlBoletosIaAgent
    {
        ValidationResponse ValidateTicket(ValidateTicketRequest request);
        ContractLookupResponse GetContract(string contractId);
        ClauseComparisonResponse CompareClauses(ClauseCompareRequest request);
        Dictionary<string, object> Health();
    }
}
