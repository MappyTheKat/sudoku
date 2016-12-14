using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class HttpParser
    {
        int gridSize;
        public HttpParser(int gs)
        {
            gridSize = gs;
        }

        public string getBoardStringFromURL(string url)
        {
            Console.WriteLine("getting from url {0}", url);
            string Boardstring = "";
            HtmlWeb myweb = new HtmlWeb();
            HtmlDocument mydoc = myweb.Load(url);
            string xpath = "";
            if (gridSize == 3)
                xpath = "//table[@id='gr']//td";
            if (gridSize == 4)
                xpath = "//table[@id='grid']//td";
            //for 16 x 16
            foreach (var n in mydoc.DocumentNode.SelectNodes(xpath))
            {
                Boardstring += stringReplacer(n.InnerText) + " ";
            }

            Console.WriteLine(Boardstring);
            return Boardstring;
        }

        string stringReplacer(string str)
        {
            return str.Replace("0", "16").Replace("A", "10").Replace("B", "11").Replace("C", "12").Replace("D", "13")
                .Replace("E", "14").Replace("F", "15").Replace("&nbsp;", "0");
        }

    }
}
