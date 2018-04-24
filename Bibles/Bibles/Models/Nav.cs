namespace Bibles.Models
{
    using Newtonsoft.Json;

    public class Nav
    {
        [JsonProperty("prev_book")]
        public string PrevBook { get; set; }

        [JsonProperty("next_book")]
        public string NextBook { get; set; }

        [JsonProperty("next_chapter")]
        public string NextChapter { get; set; }

        [JsonProperty("prev_chapter")]
        public string PrevChapter { get; set; }

        [JsonProperty("ncb")]
        public int? NextChapterBookId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int? NextChapterBookId1 { set { NextChapterBookId = value; } }

        [JsonProperty("ncc")]
        public int? NextChapterId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int? NextChapterId1 { set { NextChapterId = value; } }

        [JsonProperty("pcb")]
        public int? PrevChapterBookId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int? PrevChapterBookId1 { set { PrevChapterBookId = value; } }

        [JsonProperty("pcc")]
        public int? PrevChapterId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int? PrevChapterId1 { set { PrevChapterId = value; } }
    }

}
