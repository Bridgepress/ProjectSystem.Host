using Newtonsoft.Json;

namespace ProjectSystem.Domain.Responses
{
    public class CaptchaVerificationResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTimestamp { get; set; }
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
    }
}
