// -----------------------------------------------------------------------
// <copyright file="SubmitResult.cs" company="">
// Christian Woltering, https://github.com/wo80
// </copyright>
// -----------------------------------------------------------------------

namespace Kornea.Audio.AcousticID.Web
{
	/// <summary>
    /// Result of a submit request.
    /// </summary>
    public class SubmitResult
    {
        public int Id { get; private set; }
        public int Index { get; private set; }
        public string Status { get; private set; }
        public string Result { get; private set; }
    }
}
