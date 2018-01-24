// -----------------------------------------------------------------------
// <copyright file="Artist.cs" company="">
// Christian Woltering, https://github.com/wo80
// </copyright>
// -----------------------------------------------------------------------

namespace Kornea.Audio.AcousticID.Web
{
	/// <summary>
    /// Represents an artist.
    /// </summary>
    public class Artist
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public Artist(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
