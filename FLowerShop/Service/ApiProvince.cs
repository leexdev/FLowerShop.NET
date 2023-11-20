using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FlowerShop.Service
{
    public class ApiProvince
    {
        private readonly HttpClient _httpClient;

        public ApiProvince()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<dynamic>> GetProvincesAsync()
        {
            var response = await _httpClient.GetAsync("https://provinces.open-api.vn/api/p");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<dynamic>>(content);
        }

        public async Task<List<dynamic>> GetDistrictsByProvinceAsync(int provinceCode)
        {
            var url = $"https://provinces.open-api.vn/api/p/{provinceCode}?depth=2";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var province = JsonConvert.DeserializeObject<dynamic>(content);

            return province.districts.ToObject<List<dynamic>>() ?? new List<dynamic>();
        }

    }
}