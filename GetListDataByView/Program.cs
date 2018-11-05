using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Utilities;

namespace GetListDataByView
{
    class Program
    {
        static ClientContext clientContext;

        static void Main(string[] args)
        {
            Console.Write("Site url: ");
            var siteUrl = Console.ReadLine(); 
            Console.Write("login: ");
            var login = Console.ReadLine();   
            Console.Write("password: ");
            var password = Console.ReadLine();
            var authManager = new AuthenticationManager();
            clientContext = authManager.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, login, password);
            var web = clientContext.Site.RootWeb;
            clientContext.Load(web.Lists);
            clientContext.ExecuteQuery();

            for (int i = 0; i < web.Lists.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {clientContext.Site.RootWeb.Lists[i].Title}");
            }
            Console.Write("Select list number: ");
            var listIndex = (int.Parse(Console.ReadLine()) - 1);
            var list = web.Lists[listIndex];

            clientContext.Load(list.Views);
            clientContext.ExecuteQuery();
            for (int i = 0; i < list.Views.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {list.Views[i].Title}");
            }
            Console.Write("Select list view number: ");
            var listViewIndex = (int.Parse(Console.ReadLine()) - 1);
            var listView = list.Views[listViewIndex];

            var items = GetListDataByView(list.Id, listView.Id);
            Console.WriteLine($"{items.Count} items");
            Console.ReadKey();
        }

        static ListItemCollection GetListDataByView(Guid listId, Guid listViewId)
        {
            var list = clientContext.Site.RootWeb.GetListById(listId);
            var listView = list.GetViewById(listViewId);

            CamlQuery camlQuery = new CamlQuery
            {
                ViewXml = $"<View><Query>{listView.ViewQuery}</Query></View>"
            };
            var items = list.GetItems(camlQuery);
            clientContext.Load(items);
            clientContext.ExecuteQuery();
            return items;
        }
    }
}
