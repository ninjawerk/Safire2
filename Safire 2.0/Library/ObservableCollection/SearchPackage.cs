namespace Safire.Library.ObservableCollection
{
    /// <summary>
    ///     The class for holding data from a Search
    /// </summary>
    public sealed class SearchPackage
    {
        /// <summary>
        ///     Type of the search
        /// </summary>
        public SearchType SearchType { get; set; }

        /// <summary>
        ///     Data, as the query
        /// </summary>
        public string Data { get; set; }
    }
}