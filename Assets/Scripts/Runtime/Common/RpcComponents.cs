using Unity.NetCode;

namespace TMG.NFE_Tutorial.Common
{
    public struct MobaTeamRequest : IRpcCommand
    {
        public TeamType Value;
    }        
}