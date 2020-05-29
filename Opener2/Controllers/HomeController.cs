using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Opener2.Models;
using Opener2.Utils;
using OfficeOpenXml;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Opener2.Controllers
{
    public class HomeController : Controller
    {
        private Commander commander = new Commander();
        private ChartJson charts = new ChartJson();

        [HttpGet]
        public ActionResult Index()
        {
             return View();
        }

        [HttpPost]
        public ActionResult Index(string snils, bool dateCheck = false, bool statusCheck = false, string npf = "", string username = "")
        {
            Agreement agr;
            try
            {
                if (dateCheck)
                {
                    commander.UpdateCreatedDate(snils);
                }
                if (statusCheck)
                {
                    commander.OpenAgreement(snils);
                }
                if (!(string.IsNullOrEmpty(npf)))
                {
                    commander.ChangeNpf(snils, npf);
                }
                if (!(string.IsNullOrEmpty(username)))
                {
                    commander.ChangeAgent(snils, username);
                }
                agr = commander.Check(snils);
                ViewBag.agrList = agr.ToListForView();

                return View();
            }
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Reports()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Reports(string npf, string date1, string date2)
        {
            try
            {
                DateTime d1;
                DateTime.TryParse(date1, out d1);
                DateTime d2;
                DateTime.TryParse(date2, out d2);

                if (string.IsNullOrEmpty(npf))
                {
                    throw new Exception("Не указан фонд");
                }
                else if(npf == "halva")
                {
                    var relatedProductsList = await commander.GetRelatedProductsAsync(d1, d2);
                    string reportPath = $@"C:\inetpub\Download_Reports\halva_{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx";
                    ViewBag.reportPath = ExcelFormer.RelatedProductListToExcel(relatedProductsList, reportPath);
                }
                else
                {
                    var agreementsList = await commander.GetAgreementsAsync(npf, d1, d2);
                    string reportPath = $@"C:\inetpub\Download_Reports\{npf}_{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx";
                    ViewBag.reportPath = ExcelFormer.RelatedProductListToExcel(agreementsList, reportPath);
                }

                return Download(ViewBag.reportPath);
            } 
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
                return View("Error");
            }
        }

        public FileResult Download(string path)
        {
            var doc = System.IO.File.ReadAllBytes(path);
            var file = new System.IO.FileInfo(path);
            return File(doc, "application/vnd.ms-excel", file.Name);
        }

        public ActionResult Diagrams()
        {
            var d1 = DateTime.Now.AddDays(-30);
            var d2 = DateTime.Now;
            var dataArray = charts.GetChartJsons(d1, d2, "газ");
            ViewBag.dataArray = dataArray;
            var d1s = d1.ToString("yyyy-MM-dd");
            var d2s = d2.ToString("yyyy-MM-dd");
            ViewBag.d1 = d1s;
            ViewBag.d2 = d2s;
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ChartsData(string minDate, string maxDate, string npf)
        {
            var d1 = DateTime.Parse(minDate);
            var d2 = DateTime.Parse(maxDate);
            var dataList = await charts.GetChartJsonsAsync(d1, d2, npf);
            ViewBag.d1 = d1.ToString("yyyy-MM-dd");
            ViewBag.d2 = d2.ToString("yyyy-MM-dd");
            return Json(dataList);
        }

        public JsonResult ajaxTest()
        {
            string a = "complete";
            return Json(a);
        }
    }
}