using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace Phars
{
    internal class Program
    {
        static HttpClient _httpClient = new();
        //const string DOMAIN = "https://genshin.honeyhunterworld.com";
        //const string DOMAIN = "http://links.qatl.ru";
        //const string DOMAIN = "https://www.travelline.ru";
        //static string Domain = "https://sendpulse.com";
        static string Domain = "http://links.qatl.ru";
        static async Task Main(string[] args)
        {
            if (args.Count() == 1) Domain = args[0];
            DateTime date = DateTime.Now;
            List<string> links = new();
            Dictionary<string, int> checkedLink = new();
            Regex textHTML = new(@"text/html");
            Uri uri = new(Domain);
            links.Add(Domain);
            string url = "";
            HttpResponseMessage response = new();
            //Console.WriteLine(result);

            for (int index = 0; index < links.Count; index++)
            {
                url = links[index];
                try
                {
                    response = await _httpClient.GetAsync(url);
                    int status = (int)response.StatusCode;
                    if (status >= 200 && status < 300)
                    {
                        if (
                            response.Content.Headers.ContentType == null
                            || response.Content.Headers.ContentType.MediaType == null
                            || textHTML.IsMatch(response.Content.Headers.ContentType.MediaType)
                            )
                        {
                            string html = await response.Content.ReadAsStringAsync();
                            List<string> newLinks = new(CorrectLink(GetLinks(html), url));
                            foreach (string link in newLinks) { if (!links.Contains(link)) { links.Add(link); /*Console.WriteLine(link);*/ } }
                        }
                    }
                    checkedLink.Add(url, status);
                    Console.WriteLine($"{$"Count links: {links.Count};", -22}{$"Current link: {index + 1};", -22}{status, -4}{url}");
                }
                catch (Exception)
                {
                    checkedLink.Add(url, (int)response.StatusCode);
                }
            }

            SaveLinkToFile("CorrectLink " + uri.Host + ".txt", from pair in checkedLink where pair.Value >= 200 && pair.Value < 300 select pair.Key + " " + pair.Value, date);
            SaveLinkToFile("BadLink " + uri.Host + ".txt", from pair in checkedLink where pair.Value < 200 || pair.Value >= 300 select pair.Key + " " + pair.Value, date);
        }

        static void SaveLinkToFile(string filename, IEnumerable<string> links, DateTime currentDate)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (var line in links) sw.WriteLine(line);
                sw.WriteLine($"Проверено ссылок: {links.Count()}");
                sw.WriteLine($"Дата проверки: {currentDate}");
            }
        }

        static IEnumerable<string> GetLinks(string html)
        {
            HtmlDocument htmlDocument = new();
            HashSet<string> links = new();
            htmlDocument.LoadHtml(html);

            var nodes = GetNodes(htmlDocument, "//a[@href]");
            links = new(links.Concat(GetAttributes(nodes, "href")));

            nodes = GetNodes(htmlDocument, "//link[@href]");
            links = new(links.Concat(GetAttributes(nodes, "href")));

            nodes = GetNodes(htmlDocument, "//img[@src]");
            links = new(links.Concat(GetAttributes(nodes, "src")));

            nodes = GetNodes(htmlDocument, "//script[@src]");
            links = new(links.Concat(GetAttributes(nodes, "src")));

            return links;
        }
        static HtmlNodeCollection GetNodes(HtmlDocument htmlDocument, string XPath)
        {
            return htmlDocument.DocumentNode.SelectNodes(XPath);
        }
        static IEnumerable<string> GetAttributes(HtmlNodeCollection nodes, string attribute)
        {
            HashSet<string> links = new();
            if (nodes == null) return links;
            foreach (var node in nodes)
            {
                HtmlAttribute href = node.Attributes[attribute];
                links.Add(href.Value);
            }
            return links;
        }
        static IEnumerable<string> CorrectLink(IEnumerable<string> allLinks, string currentLink)
        {
            Regex http = new("^(https?://)");
            Regex withouthttp = new("^(//)");
            Regex tel = new(@"^(tel:)");
            Regex mail = new(@"^(mailto:)");
            Regex self = new(@"^#");
            Regex domain = new(Domain);
            HashSet<string> links = new();
            foreach (var link in allLinks)
            {
                if (!tel.IsMatch(link) && !mail.IsMatch(link) && !self.IsMatch(link) && link.Length > 0)
                    if (http.IsMatch(link) || withouthttp.IsMatch(link)) { if (domain.IsMatch(link)) links.Add(link); }
                    else if (link[0] == '/') links.Add(Domain + link);
                    else links.Add(Domain + "/" + link);
            }
            return links;
        }
    }
}