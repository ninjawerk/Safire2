namespace Kornea.Audio
{
    public enum OutputMode
    {
        DirectSound,
        WASAPI,
        ASIO
    }
    public enum StreamStatus
    {
        CanPlay,
        CanPause,
        Stopped
    }
}
