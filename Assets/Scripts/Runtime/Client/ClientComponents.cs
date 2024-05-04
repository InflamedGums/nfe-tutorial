using TMG.NFE_Tutorial.Common;
using Unity.Entities;

namespace TMG.NFE_Tutorial.Client
{
    public struct ClientTeamRequest : IComponentData
    {
        public TeamType Value;
    }
    
    
}
