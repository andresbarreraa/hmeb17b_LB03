using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FilterWebsiteData.Models;
using HtmlAgilityPack;

namespace FilterWebsiteData.Services
{
    public class HTMLFiltererService
    {
        public FilterSourceModel Fsm { get; set; }

        public HTMLFiltererService(FilterSourceModel fsm)
        {
            Fsm = fsm;
        }

        public async Task<List<ScrapeModel>> Scrape()
        {
            var scrapeList = new List<ScrapeModel>();

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(Fsm.Root + Fsm.Destination);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var detailedNodes = htmlDocument.DocumentNode.SelectNodes("//a").Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "hero-portrait-detailed");

            foreach (var detailedNode in detailedNodes)
            {
                var sm = new ScrapeModel();

                var innerHtml = new HtmlDocument();
                innerHtml.LoadHtml(detailedNode.InnerHtml);
                sm.PictureRoute = innerHtml.DocumentNode.SelectNodes("//img").Single(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "portrait")
                    .Attributes["src"].Value;
                sm.Name = innerHtml.DocumentNode.SelectNodes("//span").Single(node =>
                    node.Attributes.Contains("class") && node.Attributes["class"].Value == "portrait-title").InnerHtml;

                var infoSite = await httpClient.GetStringAsync(Fsm.Root + detailedNode.Attributes["href"].Value);
                var infoDocument = new HtmlDocument();
                infoDocument.LoadHtml(infoSite);

                sm.Description = infoDocument.DocumentNode.SelectNodes("//p").Single(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "hero-detail-description").InnerHtml;

                scrapeList.Add(sm);
            }

            return scrapeList;
        }

        public async Task<ScrapeModel> Scrape(int id)
        {
            var scrapeModel = new ScrapeModel();

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(Fsm.Root + Fsm.Destination);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var iterator = 0;
            var detailedNode = htmlDocument.DocumentNode.SelectNodes("//a").First(node =>
            {
                if (node.Attributes.Contains("class") && node.Attributes["class"].Value == "hero-portrait-detailed")
                {
                    if (iterator == id)
                    {
                        return true;
                    }

                    iterator++;
                }

                return false;
            });

            var innerHtml = new HtmlDocument();
            innerHtml.LoadHtml(detailedNode.InnerHtml);
            scrapeModel.PictureRoute = innerHtml.DocumentNode.SelectNodes("//img").Single(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "portrait")
                .Attributes["src"].Value;
            scrapeModel.Name = innerHtml.DocumentNode.SelectNodes("//span").Single(node =>
                node.Attributes.Contains("class") && node.Attributes["class"].Value == "portrait-title").InnerHtml;

            var infoSite = await httpClient.GetStringAsync(Fsm.Root + detailedNode.Attributes["href"].Value);
            var infoDocument = new HtmlDocument();
            infoDocument.LoadHtml(infoSite);

            scrapeModel.Description = infoDocument.DocumentNode.SelectNodes("//p").Single(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "hero-detail-description").InnerHtml;

            return scrapeModel;
        }
    }
}
