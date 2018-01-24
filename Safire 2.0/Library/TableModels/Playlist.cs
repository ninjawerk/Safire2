using Safire.SQLite;

namespace Safire.Library.TableModels
{
    internal class Playlist 
    {
        [PrimaryKey]
        public string ID { get; set; }
		public string Name { get; set; }
        public string Path { get; set; }
    }
}
