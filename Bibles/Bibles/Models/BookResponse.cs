namespace Bibles.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class BookResponse
    {
        #region Properties
        [JsonProperty("error_level")]
        public long ErrorLevel { get; set; }

        [JsonProperty("results")]
        public List<Book> Books { get; set; }
        #endregion
    }

}
