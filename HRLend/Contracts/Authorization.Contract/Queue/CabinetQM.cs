namespace Contracts.Authorization.Queue
{
    public class CabinetQM
    {
        public int MessageType { get; set; }
        public int CabinetId { get; set; }
    }

    public enum CABINET_MESSAGE_TYPE
    {
        ADD = 1,
        DELETE = 2
    }
}
