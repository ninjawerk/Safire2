namespace Kornea.Audio
{
	public enum OutputMode
	{
		DirectSound,
		WASAPI,
		ASIO,
		NetStream

	}
	public enum StreamStatus
	{
		CanPlay,
		CanPause,
		Stopped
	}
}
