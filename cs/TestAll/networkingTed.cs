using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OneSky.Services.Exceptions;

namespace OneSky.Services.Util
{
    /// <summary>
    /// Utilities to help create the service call.
    /// </summary>
    public static class Networking
    {
        private static HttpClient _client;
        public static string ApiKey { get; set; }
        public static Uri BaseUri { get; set; }

        static Networking()
        {
            Init();
        }

        /// <summary>
        /// Posts data of type T to a web service defined by address.  The web service returns data defined by R.
        /// </summary>
        /// <typeparam name="T">Type of data to post to the web service</typeparam>
        /// <typeparam name="R">Type of data returned by the web service</typeparam>
        /// <param name="address">The Uri of the web service, including all query parameters</param>
        /// <param name="postData">The post data to provide to the web service.</param>
        /// <exception cref="WebException">Thrown if response code is anything other than OK.</exception>
        /// <returns>Result from the Web service, as R.</returns>
        public static async Task<R> HttpPostCall<T, R>(Uri address, T postData) {

            var postDataS = JsonConvert.SerializeObject(postData, new Newtonsoft.Json.Converters.StringEnumConverter());
            HttpContent postContent = new StringContent(postDataS, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(address, postContent);
            if (!response.IsSuccessStatusCode)
            {
                GetErrorMessageandThrow(response.Content.ReadAsStringAsync().Result, response.StatusCode);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            R result;

            if (typeof(R) != typeof(string))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<R>(jsonResponse);
                }
                catch
                {
                    throw new ArgumentOutOfRangeException($"Unable to convert web response to type: {typeof(R)}");
                }
            }
            else
            {
                result = (R)Convert.ChangeType(jsonResponse, typeof(R));
            }

            return result;
        }

        public static async Task<R> HttpGetCall<R>(Uri address) {

            var response = await _client.GetAsync(address);
            if (!response.IsSuccessStatusCode)
                GetErrorMessageandThrow(response.Content.ReadAsStringAsync().Result, response.StatusCode);


            var jsonResponse = await response.Content.ReadAsStringAsync();

            R result;

            try {
                result = JsonConvert.DeserializeObject<R>(jsonResponse);
            } catch {
                throw new ArgumentOutOfRangeException($"Unable to convert web response to type: {typeof(R)}");
            }
            return result;
        }

        public static async Task<string> HttpGetCall(Uri address) {

            var response = await _client.GetAsync(address);
            if (!response.IsSuccessStatusCode)
                GetErrorMessageandThrow(response.Content.ReadAsStringAsync().Result, response.StatusCode);

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Initializes the apikey and base url from the config file.abstract  If there are issues with
        ///  the config file, a TypeInitializationException will be thrown, with the InnerException
        ///  describing the problem.
        /// </summary>
        public static void Init()
        {

            var efm = new ExeConfigurationFileMap { ExeConfigFilename = "OneSky.Services.config" };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(efm, ConfigurationUserLevel.None);
            var asc = (AppSettingsSection)configuration.GetSection("appSettings");
            if (asc.Settings.Count == 0)
            {
                throw new ConfigurationErrorsException("The configuration file is missing or empty.");
            }
            ApiKey = asc.Settings["ApiKey"]?.Value;
            if (string.IsNullOrEmpty(ApiKey))
            {
                throw new ConfigurationErrorsException("The ApiKey is not defined in the configuration file.");
            }
            var url = asc.Settings["BaseUrl"]?.Value;
            if (string.IsNullOrEmpty(url))
            {
                throw new ConfigurationErrorsException("The BaseUrl is not defined in the configuration file.");
            }
            BaseUri = new Uri(url);

            if (_client != null) return;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static Uri GetFullUri(string relativeUri) => new Uri(BaseUri, relativeUri + "?u=" + ApiKey);

        public static Uri AppendDateToUri(Uri existingUri, DateTime? date) {
            if (date.HasValue) {
                UriBuilder urib = new UriBuilder(existingUri);
                var dateQuery = $"&date={date.Value.Date:yyyy-MM-dd}";
                urib.Query = urib.Query.Substring(1) + dateQuery;
                return urib.Uri;
            }
            throw new ArgumentNullException(nameof(date), "The date must be supplied");
        }
        public static Uri AppendDateTimeAndPrnToUri(Uri existingUri,
                                                    DateTime? fromDate,
                                                    DateTime? toDate,
                                                    int? prn) {
            UriBuilder urib = new UriBuilder(existingUri);
            if (fromDate.HasValue && toDate.HasValue) {
                var fromQuery = $"&from={fromDate.Value:yyyy-MM-ddTHH:mm:ss}";
                var toQuery = $"&to={toDate.Value:yyyy-MM-ddTHH:mm:ss}";
                urib.Query = urib.Query.Substring(1) + fromQuery + toQuery;
            }
            if (prn.HasValue) {
                var prnQuery = $"&prn={prn.Value}";
                urib.Query = urib.Query.Substring(1) + prnQuery;
            }
            return urib.Uri;
        }

        internal static void GetErrorMessageandThrow(string errorResponse, HttpStatusCode status)
        {
            AnalyticalServicesException asEx = new AnalyticalServicesException(9999, $"Unknown service error response: {errorResponse}"); ;
            if (!string.IsNullOrEmpty(errorResponse))
            {
                try
                {
                    var errorResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorResponse);
                    asEx = new AnalyticalServicesException(
                        int.Parse(errorResult["ErrorId"]),
                        errorResult["Message"],
                        status)
                    {
                        HelpLink = errorResult["HelpUrl"]
                    };
                }
                catch
                {
                    throw asEx;
                }
            }
            throw asEx;
        }
    }


    private void gottliebnorm(mu, re, xin, c, s, nax, max, rnp)
    {

        Int32 n, m;

        for (n = 2; n <= nax + 1; n++) //RAE
        {
            norm1[n] = Math.Sqrt((2 * n + 1) / (2 * n - 1)); //eq 3-17 alpha 2 RAE
            norm2[n] = Math.Sqrt((2 * n + 1) / (2 * n - 3)); //eq 3-17 beta 3 RAE
            norm11[n] = Math.Sqrt((2 * n + 1) / (2 * n)) / (2 * n - 1); //eq 3-15 not reduced RAE
            normn10[n] = Math.Sqrt((n + 1) * n / 2); //zeta 5 RAE
            for (m = 1; m <= n; m++) //RAE
            {
                norm1m(n, m) = Math.Sqrt((n - m) * (2 * n + 1) / ((n + m) * (2 * n - 1))); //eq 3-18 xi  0 RAE
                norm2m(n, m) = Math.Sqrt((n - m) * (n - m - 1) * (2 * n + 1) / ((n + m) * (n + m - 1) * (2 * n - 3))); //eq 3-18 eta 1 RAE
                normn1(n, m) = Math.Sqrt((n + m + 1) * (n - m)); // zeta 6  RAE
            } //RAE
        } //RAE
               
        x = rnp * xin; //RAE
        magr = MathTimeLibr.mag(recef);
        ri = 1.0 / magr;
        xor = recef[1] * ri;
        yor = recef[2] * ri;
        zor = recef[3] * ri;
        ep = zor;
        reor = MathTimeLib.globals.re * ri;
        reorn = reor;
        muor2 = MathTimeLib.globals.mu * ri * ri;
        p[1, 1] = 1; //RAE
        p[1, 2] = 0; //RAE
        p[1, 3] = 0; //RAE
        p[2, 2] = Math.Sqrt(3); //RAE //norm
        p[2, 3] = 0; //RAE
        p[2, 4] = 0; //RAE
        for (n = 2; n <= nax; n++) //RAE
        {
            ni = n + 1; //RAE
            p[ni, ni] = norm11[n] * p[n, n] * (2 * n - 1); //RAE //norm
            p[ni, ni + 1] = 0; //RAE
            p(ni, ni + 2) = 0; //RAE
        }
        ctil[1] = 1; //RAE
        stil[1] = 0; //RAE
        ctil[2] = xor; //RAE
        stil[2] = yor; //RAE
        sumh = 0;
        sumgm = 1;
        sumj = 0;
        sumk = 0;
        p[2, 1] = Math.Sqrt(3) * ep; //RAE //norm
        for (n = 2; n <= nax; n++)
        {
            ni = n + 1; //RAE
            reorn = reorn * reor;
            n2m1 = n + n - 1;
            nm1 = n - 1;
            np1 = n + 1;
            p[ni, n] = normn1[n, n - 1] * ep * p[ni, ni]; //RAE //norm
            p[ni, 1] = (n2m1 * ep * norm1[n] * p[n, 1] - nm1 * norm2[n] * p[nm1, 1]) / n; //RAE //norm
            p[ni, 2] = (n2m1 * ep * norm1m[n, 1] * p[n, 2] - n * norm2m[n, 1] * p[nm1, 2]) / (nm1); //RAE //norm
            sumhn = normn10[n] * p[ni, 2] * gravData.cNor(ni, 1); //norm //RAE
            sumgmn = p[ni, 1] * gravData.cNor[ni, 1] * np1; //RAE
            if (max > 0)
            {
                for (m = 2; m <= n - 2; m++)
                {
                    mi = m + 1; //RAE
                    p[ni, mi] = (n2m1 * ep * norm1m[n, m] * p[n, mi] - (nm1 + m) * norm2m[n, m] * p[nm1, mi]) / (n - m); //RAE //norm
                }
                sumjn = 0;
                sumkn = 0;
                ctil[ni] = ctil[2] * ctil[ni - 1] - stil[2] * stil[ni - 1]; //RAE
                stil[ni] = stil[2] * ctil[ni - 1] + ctil[2] * stil[ni - 1]; //RAE
                if (n < max)
                    lim = n;
                else
                    lim = max;

                for (m = 1; m <= lim; m++)
                {
                    mi = m + 1; //RAE
                    mm1 = mi - 1; //RAE
                    mp1 = mi + 1; //RAE

                    mxpnm = m * p(ni, mi); //RAE
                    bnmtil = gravData.cNor[ni, mi] * ctil[mi] + gravData.sNor[ni, mi] * stil[mi]; //RAE
                    sumhn = sumhn + normn1[n, m] * p[ni, mp1] * bnmtil; //RAE //norm
                    sumgmn = sumgmn + (n + m + 1) * p[ni, mi] * bnmtil; //RAE
                    bnmtm1 = gravData.cNor[ni, mi] * ctil[mm1] + gravData.sNor[ni, mi] * stil[mm1]; //RAE
                    anmtm1 = gravData.cNor[ni, mi] * stil[mm1] - gravData.sNor[ni, mi] * ctil[mm1]; //RAE
                    sumjn = sumjn + mxpnm * bnmtm1;
                    sumkn = sumkn - mxpnm * anmtm1;
                }
                sumj = sumj + reorn * sumjn;
                sumk = sumk + reorn * sumkn;
            }
            sumh = sumh + reorn * sumhn;
            sumgm = sumgm + reorn * sumgmn;
        }
        lambda = sumgm + ep * sumh;
        g[1, 1] = -muor2 * (lambda * xor - sumj);
        g[2, 1] = -muor2 * (lambda * yor - sumk);
        g[3, 1] = -muor2 * (lambda * zor - sumh);
        accel = rnp * g; //RAE

    }


}