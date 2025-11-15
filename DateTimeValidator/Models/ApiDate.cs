namespace DateTime_Validator.Models
{
    public class ApiDate
    {
        public bool Ok { get; set; }
        public string Status { get; set; }
        public int Status_code { get; set; }
        public ApiDateResult Result { get; set; }
    }

    public class ApiDateResult
    {
        public string Timezone { get; set; }
        public string Action { get; set; }
        public string Zone { get; set; }
        public string Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string Year_name { get; set; }
        public string Month_name { get; set; }
        public string Day_name { get; set; }
        public string Season_name { get; set; }
    }
}
