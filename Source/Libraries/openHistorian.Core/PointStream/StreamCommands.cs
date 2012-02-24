
namespace openHistorian.Core.PointStream
{
    public enum StreamCommands : byte
    {
        EndOfStream=0,
        NoCommand=1,
        PointDefinition=2,
        InitializePointState=3,
        SinglePointMessage=4,
        PointMessageString=5
    }
}
