using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Opener2.Utils
{
    /// <summary>
    /// Данные для диаграммы.
    /// </summary>
    public class ChartJson
    {
        private readonly string _webConnection = ConfigurationManager.ConnectionStrings["ESP_WEB"].ConnectionString;

        /// <summary>
        /// Координаты разметки диаграммы.
        /// </summary>
        public class Coordinates
        {
            /// <summary>
            /// Дата договора.
            /// </summary>
            public DateTime x { get; set; }
            /// <summary>
            /// Количество договоров.
            /// </summary>
            public string y { get; set; }
        }

        /// <summary>
        /// Представляет точку на диаграмме.
        /// </summary>
        private class AgreementForCharts
        {
            /// <summary>
            /// Даты и количество договоров.
            /// </summary>
            public Coordinates Data { get; set; }    //{ x:date, y:ctx }
            /// <summary>
            /// Отдел продаж.
            /// </summary>
            public string Unit { get; set; }
            /// <summary>
            /// Фонд.
            /// </summary>
            public string Npf { get; set; }
        }

        public string Label { get; set; }
        public string BorderColor { get; set; }
        public Coordinates[] Data { get; set; }

        private List<string> _colors = new List<string>()
            {
                "red",
                "orange",
                "yellow",
                "green",
                "blue",
                "purple",
                "grey"
            };

        /// <summary>
        /// Получить договора для диаграмм с координатами.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="npf"></param>
        /// <returns></returns>
        private List<AgreementForCharts> GetAgreementsForCharts(DateTime d1, DateTime d2, string npf)
        {
            const string sqlExpression = @"WITH a AS(
                                            SELECT CONVERT(date, a.CreatedOn) AS created, bu.Name bu, n.Name npf
                                            FROM Agreements a
                                            JOIN Employees e ON e.id = a.CreatorId
                                            JOIN BusinessUnits bu ON bu.id = e.BusinessUnitId
                                            JOIN Npfs n on n.id = a.NpfId
                                            WHERE a.CreatedOn BETWEEN (@date1) AND (@date2)
                                            AND n.Name LIKE '%' + @npf + '%'                  
                                            )
                                            SELECT created, count(*) ctx, bu, npf FROM a
                                            GROUP BY created, bu, npf;";

            var agrs = new List<AgreementForCharts>();
            using (var connection = new SqlConnection(_webConnection))
            {
                connection.Open();
                var cmd = new SqlCommand(sqlExpression, connection);
                var date1Param = new SqlParameter("@date1", d1);
                var date2Param = new SqlParameter("@date2", d2);
                var npfParam = new SqlParameter("@npf", npf);
                cmd.Parameters.Add(date1Param);
                cmd.Parameters.Add(date2Param);
                cmd.Parameters.Add(npfParam);
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var agr = new AgreementForCharts();
                        agr.Data = new Coordinates
                        {
                            x = DateTime.Parse(reader.GetValue(0).ToString()),
                            y = reader.GetValue(1).ToString()
                        };
                        agr.Unit = reader.GetValue(2).ToString();
                        agr.Npf = reader.GetValue(3).ToString();

                        agrs.Add(agr);
                    }
                    reader.Close();
                }
            }

            return agrs;
        }

        /// <summary>
        /// Получить данные для диаграмм в JSON формате.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="npf"></param>
        /// <returns></returns>
        public string GetChartJsons(DateTime d1, DateTime d2, string npf)
        {
            var agrs = GetAgreementsForCharts(d1, d2, npf);
            var units = agrs.Select(u => u.Unit).Distinct().ToList();
            var chartJsons = new List<ChartJson>();
            var count = 0;
            foreach (var e in units)
            {
                var c = new ChartJson()
                {
                    Label = e,
                    BorderColor = _colors[count],
                    Data = agrs.Where(a => a.Unit == e).Select(d => d.Data).OrderBy(o => o.x).ToArray()
                };
                chartJsons.Add(c);
                count++;
            }
            return JsonConvert.SerializeObject(chartJsons);
        }

        /// <summary>
        /// Асинхронная версия получения данных для диаграмм в JSON формате.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="npf"></param>
        /// <returns></returns>
        public async Task<string> GetChartJsonsAsync(DateTime d1, DateTime d2, string npf)
        {
            return await Task.Run(() => GetChartJsons(d1, d2, npf));
        }
    }
}