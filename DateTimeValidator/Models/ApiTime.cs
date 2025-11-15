namespace DateTime_Validator.Models
{
    public class ApiTime
    {
        public bool Ok { get; set; }
        public string Status { get; set; }
        public int Status_code { get; set; }
        public Result Result { get; set; }
    }
    public class Result
    {
        public string Timezone { get; set; }
        public string Action { get; set; }
        public string Zone { get; set; }
        public string Time { get; set; }
        public int Houre { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }
}
